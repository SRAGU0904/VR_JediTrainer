using UnityEngine;

public class ForceObjectHandler : MonoBehaviour
{
	public ParticleSystem explosionCollider;
	private int hp;
	void Start() {
		hp = 2;
	}

	void Update() {
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Laser")){
			hp -= 1;
			if(hp == 0)
				explodeForceObject();
		}else if (other.CompareTag("Enemy")){
			Destroy(other.gameObject);
			explodeForceObject();
		}
	}

	void explodeForceObject() {
		ParticleSystem effect = Instantiate(explosionCollider, transform.position, Quaternion.identity);
		effect.Play();
		Destroy(effect.gameObject, 2f);
		Destroy(gameObject);
	}
}
