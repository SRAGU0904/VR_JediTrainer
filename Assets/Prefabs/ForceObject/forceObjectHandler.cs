using UnityEngine;

public class forceObjectHandler : MonoBehaviour
{
	private int hp;
	void Start() {
		hp = 2;
	}

	void Update() {
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Laser")){
			hp -= 1;
			if(hp < 0)
				Destroy(gameObject);
		}else if (other.CompareTag("Enemy")){
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
	}
}
