using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;
using static ForceHandler;
using UnityEditor.Experimental.GraphView;

[RequireComponent(typeof(InputData))]
public class ForceHandler : MonoBehaviour{
    private InputData _inputData;

    public Transform xrOrigin;
    public LayerMask interactableLayer;
	public enum HandType {
		LeftHand,
		RightHand,
	}
	public HandType handType;

	private float coneAngle = 15f;
    private float pushForceMultiplier = 6f;
    private float pullSpeed = 5f;
    private float maxForceDistance = 15f;
    private float minForcePower = 1.5f;
    private float minPlaneDistanceFromHand = 0.75f;
    private float maxPlaneDistanceFromHand = 10f;

    private List<Rigidbody> heldObjects = new List<Rigidbody>();
    private bool isTriggerPressed;
    private Vector3 handVelocity;
    private Vector3 globalHandPosition;
	private Vector3 currentlocalHandPosition;
	private Vector3 previouslocalHandPosition;

	private Vector3 handForward;
    private Vector2 joyStickAxis;
    private float planeDistanceFromHand;
    private float forceAmount;

    private void Start(){
        _inputData = GetComponent<InputData>();
		Vector3 localPos;
		if (handType == HandType.LeftHand){
			_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out localPos);
		}else{
			_inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out localPos);
		}
		
        previouslocalHandPosition = xrOrigin.TransformPoint(localPos);
    }

    void Update(){
		if (handType == HandType.LeftHand){
			_inputData._leftController.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed);
			_inputData._leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out joyStickAxis);
			_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out currentlocalHandPosition);
		}else{
			_inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed);
			_inputData._rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out joyStickAxis);
			_inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out currentlocalHandPosition);
		}

		globalHandPosition = xrOrigin.TransformPoint(currentlocalHandPosition);
        handVelocity = (currentlocalHandPosition - previouslocalHandPosition) / Time.deltaTime;
        previouslocalHandPosition = currentlocalHandPosition;

        handForward = transform.forward.normalized;
        forceAmount = Vector3.Dot(handVelocity, handForward);

        if (isTriggerPressed){
            if(Mathf.Abs(forceAmount) >= minForcePower){
				SendHacticFeedback();
				UseTheForce();
            }
            ForcePullHeldObjects();
        }else{
            ReleaseHeldObjects();
        }
    }

    void UseTheForce(){
        Collider[] hits = Physics.OverlapSphere(globalHandPosition, maxForceDistance, interactableLayer);

        foreach (Collider hit in hits){

            Vector3 dirToObj = (hit.transform.position - globalHandPosition).normalized;
            if (Vector3.Angle(handForward, dirToObj) < coneAngle){
				if (forceAmount >= minForcePower && heldObjects.Count == 0)
					ForcePush(hit);
				else
					AddObjectToHeldObjects(hit);
            }
        }
    }
    void ForcePush(Collider hit){
        Rigidbody rb = hit.attachedRigidbody;
        if(rb != null && rb.useGravity){
            Vector3 forceDir = (hit.transform.position - globalHandPosition).normalized;
            rb.AddForce(forceDir * handVelocity.magnitude * pushForceMultiplier, ForceMode.Impulse);
        }
    }

    void AddObjectToHeldObjects(Collider hit) { 
        Rigidbody rb = hit.attachedRigidbody;
        if (rb != null && !heldObjects.Contains(rb)){
            rb.useGravity = false;
            heldObjects.Add(rb);
        }
    }

    Vector2 GetObjectPlaneOffset(int index){
        if (heldObjects.Count <= 1)
            return Vector2.zero;

        float radius = 0.3f;
        float angleStep = 360f / heldObjects.Count;
        float angle = angleStep * index;
        float rad = angle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
    }

    void updatePlaneDistanceFromHand(){
        if (joyStickAxis.y > 0.1 && planeDistanceFromHand < maxPlaneDistanceFromHand ||
            joyStickAxis.y < -0.1 && planeDistanceFromHand > minPlaneDistanceFromHand)
            planeDistanceFromHand += joyStickAxis.y * 3f * Time.deltaTime;
    }

    void ForcePullHeldObjects(){
        updatePlaneDistanceFromHand();
        Vector3 planeOrigin = globalHandPosition + handForward * planeDistanceFromHand;
        Vector3 handRight = transform.right;
        Vector3 handUp = transform.up;

		if(heldObjects.Count > 0)
			SendHacticFeedback();

		for (int i = heldObjects.Count - 1; i >= 0; i--) {
            Rigidbody rb = heldObjects[i];
            if (rb != null){
				if (forceAmount > minForcePower) {
					rb.AddForce(handForward * handVelocity.magnitude * pushForceMultiplier, ForceMode.Impulse);
					heldObjects.RemoveAt(i);
					StartCoroutine(ReenableGravityAfterDelay(rb, 1.0f));
				}else{
					Vector2 planeOffset = GetObjectPlaneOffset(i);
					Vector3 localOffset = handRight * planeOffset.x + handUp * planeOffset.y;
					Vector3 targetWorldPos = planeOrigin + localOffset;
					Vector3 toTarget = targetWorldPos - rb.position;

					rb.linearVelocity = toTarget * pullSpeed;
				}
            }
        }
    }

    void ReleaseHeldObjects(){
        planeDistanceFromHand = minPlaneDistanceFromHand;
        foreach (var rb in heldObjects){
            if (rb != null){
                rb.useGravity = true;
            }
        }
        heldObjects.Clear();
    }

	void SendHacticFeedback() {
		InputDevice controller;
		if (handType == HandType.LeftHand)
			controller = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		else
			controller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

		if (controller.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse) {
			uint channel = 0; 
			float amplitude = 0.25f; 
			float duration = 0.1f;
			controller.SendHapticImpulse(channel, amplitude, duration);
		}
	}

	IEnumerator ReenableGravityAfterDelay(Rigidbody rb, float delay){
		yield return new WaitForSeconds(delay);
		if (rb != null) {
			rb.useGravity = true;
		}
	}
}