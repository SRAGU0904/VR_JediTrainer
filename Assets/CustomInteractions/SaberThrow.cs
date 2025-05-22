using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputData))]
public class SaberThrow : MonoBehaviour
{
    [Header("References")]
    public Transform rightControllerDirection;
    public InputActionReference throwAction;

    [Header("Throw Settings")]
    public float returnDelay = 0.9f;
    public float spinTorque = 300f;
    public float minFlyTime = 0.2f;
    public float returnAcceleration = 10f;

    private float initialThrowSpeed = 0f;
    private float returnSpeed = 0f;
    private float returnTimer = 0f;
    private float flightTimer = 0f;

    private Vector3 flyDirection;
    private Vector3 spinAxis;

    private bool isHeld = false;
    private bool canSetHeld = false;
    private bool isFlyingOut = false;
    private bool isReturning = false;
    private bool canReturn = false;

    private Rigidbody rb;
    private Collider col;
    private XRGrabInteractable interactable;
    private IXRSelectInteractor interactor;
    private InputData _inputData;

    public float requiredThrowForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        interactable = GetComponent<XRGrabInteractable>();
        _inputData = GetComponent<InputData>();

        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);

        if (interactable.isSelected)
        {
            isHeld = true;
            interactor = interactable.firstInteractorSelecting;
        }
        else
        {
            isHeld = false;
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void OnEnable()
    {
        throwAction.action.Enable();
        throwAction.action.performed += HandleInput;
    }

    void OnDisable()
    {
        throwAction.action.Disable();
        throwAction.action.performed -= HandleInput;
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (context.performed && isHeld)
        {
            Throw();
        }
    }

    private void HandleMovement()
    {
        if (isFlyingOut)
        {
            flightTimer += Time.deltaTime;
            if (flightTimer > returnDelay)
                StartReturn();
        }

        if (isReturning)
        {
            returnTimer += Time.deltaTime;
            Vector3 dir = (rightControllerDirection.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, rightControllerDirection.position);
            float boostedSpeed = returnSpeed + returnAcceleration * returnTimer;
            rb.linearVelocity = dir * boostedSpeed;

            if (distance < 0.5f)
                CompleteReturn();
        }

        if (isFlyingOut || isReturning)
        {
            rb.AddTorque(spinAxis * spinTorque * Time.deltaTime, ForceMode.Force);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        isHeld = true;
        isFlyingOut = false;
        isReturning = false;

        rb.isKinematic = true;
        rb.useGravity = false;

        AttachToController();
        Debug.Log("[BOOMERANG DEBUG] Grabbed boomerang!");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!canSetHeld)
        {
            Debug.Log("[BOOMERANG DEBUG] Ignored early release");
            return;
        }

        isHeld = false;
        Debug.Log("[BOOMERANG DEBUG] Released");
    }

    private void Throw()
    {
        _inputData._rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceVelocity, out Vector3 localVelocity);
        _inputData._rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceAngularVelocity, out Vector3 angularVel);

        Vector3 controllerVelocity = rightControllerDirection.TransformDirection(localVelocity);
        float controllerSpeed = controllerVelocity.magnitude;
        float speedThreshold = 0.3f;

        if (controllerSpeed <= speedThreshold)
        {
            Debug.Log("[BOOMERANG DEBUG] Throw cancelled: speed too low");
            return;
        }

        if (interactor != null && interactable.interactionManager != null)
        {
            interactable.interactionManager.SelectExit(interactor, interactable);
        }

        isHeld = false;
        canSetHeld = true;
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.angularDamping = 0f;

        flyDirection = rightControllerDirection.forward;
        Vector3 motionDir = localVelocity.normalized;
        float speedMultiplier = 8f;
        initialThrowSpeed = controllerSpeed * speedMultiplier;
        rb.linearVelocity = flyDirection * initialThrowSpeed;

        if (controllerSpeed > speedThreshold)
        {
            spinAxis = Vector3.Cross(flyDirection, motionDir).normalized;

            if (spinAxis == Vector3.zero || float.IsNaN(spinAxis.x))
            {
                spinAxis = rightControllerDirection.up;
            }

            rb.inertiaTensor = Vector3.one;
            rb.inertiaTensorRotation = Quaternion.identity;
            rb.angularVelocity = spinAxis * 50f;

            Debug.Log($"[DEBUG] flyDir: {flyDirection}, motionDir: {motionDir}, spinAxis: {spinAxis}");
        }

        flightTimer = 0f;
        isFlyingOut = true;
        col.enabled = true;

        canReturn = false;
        Invoke(nameof(EnableReturn), minFlyTime);
        Invoke(nameof(StartReturn), returnDelay);
    }

    private void EnableReturn()
    {
        canReturn = true;
    }

    private void StartReturn()
    {
        isReturning = true;
        isFlyingOut = false;

        returnSpeed = initialThrowSpeed * 0.8f;
        returnTimer = 0f;
    }

    private void CompleteReturn()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        AttachToController();

        if (interactor != null && interactable.interactionManager != null)
        {
            interactable.interactionManager.SelectEnter(interactor, interactable);
        }

        isReturning = false;
        isFlyingOut = false;

        isHeld = true;
        canSetHeld = false;

        Debug.Log("[BOOMERANG DEBUG] Returned to hand");
    }

    private void AttachToController()
    {
        transform.SetParent(rightControllerDirection);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public bool IsHeld()
    {
        return isHeld;
    }
}
