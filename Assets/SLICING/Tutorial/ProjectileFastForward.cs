using Unity.VisualScripting;
using UnityEngine;

public class ProjectileFastForward : Projectile {
	public float keepDistance = 1f;
	public float farAwayMultiplier = 5f;
	public float baseSpeed = 1f;
	private Transform player;

	void Start() {
		// Assume single player
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	protected void Update() {
		if ((transform.position - player.position).magnitude > keepDistance) {
			speed = baseSpeed * farAwayMultiplier;
		}
		else {
			speed = baseSpeed;
		}
		base.Update();
	}

}
