using UnityEngine;

public class TargetingSystem : MonoBehaviour {
	public GameObject laser;
	public Transform firePoint;
	public GameObject target;
	public float minDelay;
	public float maxDelay;
	public float maxDistanceAway = 1f;

	void Start() {
		ScheduleNextShot();
	}

	void Update() {
		transform.rotation = Quaternion.LookRotation(HeadDirection());
	}

	void ScheduleNextShot() {
		Invoke("Shoot", Random.Range(minDelay, maxDelay));
	}

	void Shoot() {
		Quaternion rotation = Quaternion.LookRotation(ShootDirection(true));
		GameObject projectile = Instantiate(laser, firePoint.position, rotation);
		Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
		ScheduleNextShot();
	}

	Vector3 HeadDirection() {
		if (target == null) return Vector3.forward;

		CharacterController characterController = target.GetComponent<CharacterController>();
		Vector3 headPosition = characterController.transform.position + Vector3.up * (characterController.height / 2 + 0.25f);
		return (headPosition - firePoint.position).normalized;
	}
	Vector3 ShootDirection(bool randomness) {
		if (target == null) return Vector3.forward;

		CharacterController characterController = target.GetComponent<CharacterController>();
		Vector3 headPosition = characterController.transform.position + Vector3.up * (characterController.height / 2 + 0.25f);
		Vector3 headVelocity = characterController.velocity;

		float projectileSpeed = 10f; 
		Vector3 toTarget = headPosition - firePoint.position;

		float timeToTarget = toTarget.magnitude / projectileSpeed;
		Vector3 predictedPosition = headPosition + headVelocity * timeToTarget;

		if (randomness) {
			predictedPosition += characterController.transform.forward * Random.Range(-1 * maxDistanceAway, maxDistanceAway);
		}

		return (predictedPosition - firePoint.position).normalized;
	}
}