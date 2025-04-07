using UnityEngine;
using UnityEngine.XR;
using System.Collections;

[RequireComponent(typeof(InputData))]
public class ForceHandler : MonoBehaviour
{
 
    private float forceMultiplier = 250f;
    private float maxRayDistance = 500f;
    private float velocityThreshold = 0.75f;
    private float gestureCooldown = 0.25f;

    private InputData _inputData;
    private float lastForceTime = -Mathf.Infinity;
    private Rigidbody lastRBHit;

    private void Start(){
        _inputData = GetComponent<InputData>();
    }

    void Update(){
        _inputData._leftController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed);
        _inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 currentPosition);
        _inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 currentVelocity);

        if (!isTriggerPressed)
            lastRBHit = null;

        if (isTriggerPressed && 
            Time.time - lastForceTime > gestureCooldown && 
            currentVelocity.magnitude > velocityThreshold){

            if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxRayDistance))
                lastRBHit = hit.rigidbody;

           if (lastRBHit){
                float dot = Vector3.Dot(currentVelocity.normalized, transform.forward);
                lastRBHit.linearVelocity = (lastRBHit.position - currentPosition) * (dot > 0 ? -1 : 1) * (currentVelocity.magnitude - velocityThreshold) * forceMultiplier * Time.deltaTime;
           }
        }else
            lastRBHit = null;
    }
}