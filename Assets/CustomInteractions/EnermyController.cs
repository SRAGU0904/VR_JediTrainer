using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public ParticleSystem deathEffect;

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

            Destroy(gameObject);
        }
    }
}