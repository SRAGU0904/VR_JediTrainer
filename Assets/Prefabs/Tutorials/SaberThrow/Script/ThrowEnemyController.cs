using UnityEngine;

public class ThrowEnemyController : MonoBehaviour
{
	public ParticleSystem deathEffect;
	public EnemyManager manager;

	private bool isDead = false;

	private void OnTriggerEnter(Collider other)
	{
		if (isDead) return;

		if (other.CompareTag("Lightsaber"))
		{
			isDead = true;

			if (deathEffect != null)
			{
				ParticleSystem effect = Instantiate(deathEffect, transform.position, Quaternion.identity);
				effect.Play();
				Destroy(effect.gameObject, 2f);
			}

			if (manager != null)
			{
				manager.OnEnemyKilled();
			}

			Destroy(gameObject);
		}
	}
}