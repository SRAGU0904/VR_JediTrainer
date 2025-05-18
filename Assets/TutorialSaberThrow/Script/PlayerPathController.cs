using UnityEngine;

public class PlayerPathController : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform[] waypoints;            // Movement points the player should follow
    public float moveSpeed = 5f;             // Speed of movement between waypoints
    public float stopDistance = 0.5f;        // Distance threshold to consider arrival at a waypoint

    [Header("Combat Phase Settings")]
    public GameObject[] enemyGroups;         // Array of enemy groups per waypoint

    private int currentIndex = 0;            // Index of current waypoint
    private bool isFighting = false;         // Whether the player is in a combat phase

    void Update()
    {
        // Stop if all waypoints are completed
        if (currentIndex >= waypoints.Length)
        {
            Debug.Log("[PATH] All waypoints completed.");
            return;
        }

        // If player is fighting and presses G, skip this enemy group
        if (isFighting && Input.GetKeyDown(KeyCode.G))
        {
	        Debug.Log($"[PATH] G key pressed — clearing all enemies at point {currentIndex}.");

	        if (enemyGroups.Length > currentIndex && enemyGroups[currentIndex] != null)
	        {
		        foreach (Transform child in enemyGroups[currentIndex].transform)
		        {
			        Destroy(child.gameObject);  // Simulate enemy defeat
		        }
	        }

	        // Let Update() handle the transition naturally next frame
	        return;
        }


        // If fight is over because all enemies are dead, proceed
        if (isFighting && IsEnemyGroupCleared(currentIndex))
        {
            OnEnemyGroupCleared();
            return;
        }

        // If still fighting, wait
        if (isFighting)
        {
            Debug.Log($"[PATH] Currently fighting at point {currentIndex}");
            return;
        }

        // Move the player toward the current waypoint
        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);

        // If player has arrived at the waypoint
        
        if (distance < stopDistance)
        {
	        transform.position = target.position;  // snap to exact point

	        bool isLast = (currentIndex == waypoints.Length - 1);
	        bool cleared = IsEnemyGroupCleared(currentIndex);

	        if (!cleared)
	        {
		        StartFightPhase(currentIndex);  // Begin combat
	        }
	        else if (!isLast)
	        {
		        ProceedToNextPoint();  // Auto-advance if already cleared
	        }
        }
    }

    // Start the combat phase at the given point
    void StartFightPhase(int index)
    {
        isFighting = true;

        if (enemyGroups.Length > index && enemyGroups[index] != null)
        {
            enemyGroups[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"[PATH] No enemy group assigned for index {index}");
        }
    }

    // Called when enemies are cleared OR when skipped via G key
    public void OnEnemyGroupCleared()
    {
        Debug.Log($"[PATH] Enemy group at {currentIndex} cleared or skipped. Proceeding.");
        ProceedToNextPoint();
    }

    // Advance to the next waypoint
    private void ProceedToNextPoint()
    {
        isFighting = false;
        currentIndex++;
        Debug.Log($"[STATE] At waypoint {currentIndex}: Position = {waypoints[currentIndex].position}");

    }

    // Check if current enemy group has no active children left
    private bool IsEnemyGroupCleared(int index)
    {
        if (enemyGroups.Length <= index || enemyGroups[index] == null)
        {
            Debug.Log($"[CHECK] No enemy group at point {index} — treating as cleared.");
            return true;
        }

        int count = enemyGroups[index].transform.childCount;
        Debug.Log($"[CHECK] Enemy group '{enemyGroups[index].name}' has {count} children.");

        return count == 0;
    }
}
