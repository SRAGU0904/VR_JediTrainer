using UnityEngine;

public class Projectile : MonoBehaviour {
	public float speed = 10f;
	public float lifetime = 20f;

	void Start() {
		Destroy(gameObject, lifetime); // Destroy after a delay
	}

	void Update() {
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player")) // or check for specific component
		{
			var health = other.GetComponent<HealthBarController>();
			if (health != null)
			{
				health.TakeDamage(10);
			}
		}

		Destroy(gameObject); // Destroy the projectile
	}
}
