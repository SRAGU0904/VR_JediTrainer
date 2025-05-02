using UnityEngine;

public class TargetingSystem : MonoBehaviour {
    public GameObject laser;
    public Transform firePoint;
    public GameObject target;
    public float minDelay;
    public float maxDelay;

    void Start() {
        ScheduleNextShot();
    }

    void Update() {
        transform.rotation = Quaternion.LookRotation(ComputeDirection());
    }

    void ScheduleNextShot() {
        Invoke("Shoot", Random.Range(minDelay, maxDelay));
    }

    void Shoot() {
        Quaternion rotation = Quaternion.LookRotation(ComputeDirection());
        Instantiate(laser, firePoint.position, rotation);
        ScheduleNextShot();
    }

    Vector3 ComputeDirection() {
        if (target == null) return Vector3.up;

        CharacterController characterController = target.GetComponent<CharacterController>();
        Vector3 headPosition = characterController.transform.position + Vector3.up * (characterController.height / 2 + 0.25f);
        return (headPosition - firePoint.position).normalized;
    }
}
