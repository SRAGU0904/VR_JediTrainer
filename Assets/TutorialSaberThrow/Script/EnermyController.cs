using UnityEngine;

public class EnermyController : MonoBehaviour
{
	[Header("Explosion Effect")]
	public ParticleSystem deathEffect;

	private bool isDead = false;

	private void OnTriggerEnter(Collider other)
	{
		if (isDead) return;

		if (other.CompareTag("Lightsaber"))
		{
			isDead = true;
			Debug.Log($"[ENEMY] {gameObject.name} was hit and is being destroyed.");

			// Play explosion effect
			if (deathEffect != null)
			{
				ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
				effect.Play();
				Destroy(effect.gameObject, 2f);
			}

			CheckIfLastEnemy();

			// Destroy the enemy
			Destroy(gameObject);
		}
	}

	private void CheckIfLastEnemy()
	{
		Transform parent = transform.parent;
		if (parent == null) return;

		// Count remaining active enemies under the same group
		int aliveCount = 0;
		foreach (Transform child in parent)
		{
			if (child != transform && child.gameObject.activeInHierarchy)
				aliveCount++;
		}

		if (aliveCount == 0)
		{
			Debug.Log("[ENEMY] All enemies defeated in this group, triggering path progression.");
			FindObjectOfType<PlayerPathController>()?.OnEnemyGroupCleared();
		}
	}
}