using UnityEngine;

public class TIEFighterPatrol : MonoBehaviour
{
	[Header("Waypoints (in rectangular order)")]
	public Transform[] waypoints;  // The patrol path points (set in clockwise or counterclockwise order)

	[Header("Movement Settings")]
	public float moveSpeed = 3f;      // Movement speed
	public float rotateSpeed = 3f;    // Rotation speed
	public bool rotationEnabled = true;

	private int currentIndex = 0;     // Current waypoint index
	private float patrolHeight;       // Fixed height to maintain

	void Start()
	{
		// Ensure at least 2 waypoints are set
		if (waypoints.Length < 2)
		{
			enabled = false;
			return;
		}

		// Store the initial Y height of the object
		patrolHeight = transform.position.y;
	}

	void Update()
	{
		Transform target = waypoints[currentIndex];

		// Get horizontal direction toward the target ---
		Vector3 direction = (target.position - transform.position);
		direction.y = 0;  // Ignore vertical difference to keep level

		// Move toward the target point ---
		Vector3 moveDir = direction.normalized;
		transform.position += moveDir * moveSpeed * Time.deltaTime;

		// Keep the Y position constant (patrol height)
		transform.position = new Vector3(transform.position.x, patrolHeight, transform.position.z);

		// Smoothly rotate toward the direction of movement ---
		if (rotationEnabled && moveDir != Vector3.zero)
		{
			Quaternion lookRot = Quaternion.LookRotation(moveDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
		}

		// Switch to the next waypoint when close enough ---
		if (direction.magnitude < 0.5f)
		{
			currentIndex = (currentIndex + 1) % waypoints.Length;
		}
	}
}