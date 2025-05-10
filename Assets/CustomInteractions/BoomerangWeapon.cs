using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR;

[RequireComponent(typeof(InputData))]
public class BoomerangWeapon : MonoBehaviour
{
    [Header("References")]
    public Transform rightControllerDirection;   // The direction reference from the right controller
    [Header("XR Rig Reference")]
    public Transform xrOrigin;                   // XR Rig

    [Header("Throw Settings")]
    public float returnDelay = 1f;              // Time after throw before returning
    public float spinTorque = 300f;               // Amount of spin force when flying
    public float minFlyTime = 0.2f;               // Minimum time before allowing return
    public float returnAcceleration = 10f;        // Acceleration while returning

    private float initialThrowSpeed = 0f;         // Speed at moment of throw
    private float returnSpeed = 0f;               // Speed during return
    private float returnTimer = 0f;               // Timer for return movement
    private float flightTimer = 0f;               // Timer for flying out

    private Vector3 flyDirection;                 // Current flying direction
    private Vector3 spinAxis;                     // Axis used for spinning the boomerang

    private bool isHeld = false;                  // Whether boomerang is currently held
    private bool canSetHeld = false;               // Whether we allow re-grabbing logic
    private bool isFlyingOut = false;             // Whether boomerang is flying out
    private bool isReturning = false;             // Whether boomerang is returning
    private bool canReturn = false;               // Whether returning is allowed

    // References
    private Rigidbody rb;
    private Collider col;
    private XRGrabInteractable interactable;
    private IXRSelectInteractor interactor;
    private InputData _inputData;

    // === Initialization ===
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        interactable = GetComponent<XRGrabInteractable>();
        _inputData = GetComponent<InputData>();

        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);

        // Check if starting already held
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

    // === Per-frame Update ===
    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    // === Handle input ===
    private void HandleInput()
    {
        bool isAPressed = false;
        _inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out isAPressed);

        if (isHeld && isAPressed)
        {
            Throw();
        }
    }

    // === Handle flying out or returning movement ===
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

            // Move toward the hand
            Vector3 dir = (rightControllerDirection.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, rightControllerDirection.position);
            float boostedSpeed = returnSpeed + returnAcceleration * returnTimer;

            rb.linearVelocity = dir * boostedSpeed;

            if (distance < 0.5f)
                CompleteReturn();
        }

        if (isFlyingOut || isReturning)
        {
            // Constant rotation around spin axis
            rb.AddTorque(spinAxis * spinTorque * Time.deltaTime, ForceMode.Force);
        }
    }

    // === Handle grab (select entered) ===
    private void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        isHeld = true;
        isFlyingOut = false;
        isReturning = false;

        rb.isKinematic = true;
        rb.useGravity = false;

        AttachToController();
    }

    // === Handle release (select exited) ===
    private void OnRelease(SelectExitEventArgs args)
    {
        if (!canSetHeld)
        {
            return;
        }

        isHeld = false;
    }

    // === Core throwing logic ===
    private void Throw()
{
    // Get controller velocity and angular velocity from input
    _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 localVelocity);
    _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angularVel);

    // Convert local velocity to world space
    Vector3 controllerVelocity = rightControllerDirection.TransformDirection(localVelocity);
    float controllerSpeed = controllerVelocity.magnitude;
    float speedThreshold = 0.3f;

    // Only allow throwing if speed is above threshold
    if (controllerSpeed <= speedThreshold)
    {
        Debug.Log("[BOOMERANG DEBUG] Throw cancelled: speed too low");
        return;
    }

    // Force deselection of the interactable
    if (interactor != null && interactable.interactionManager != null)
    {
        interactable.interactionManager.SelectExit(interactor, interactable);
    }

    isHeld = false;
    canSetHeld = true;
    transform.SetParent(null);

    // Prepare rigidbody for physics-based movement
    rb.isKinematic = false;
    rb.useGravity = false;
    rb.constraints = RigidbodyConstraints.None;
    rb.angularDamping = 0f;

    // Use the rightControllerDirection forward as the flying direction
    flyDirection = rightControllerDirection.forward;
    float speedMultiplier = 8f;
    initialThrowSpeed = Mathf.Pow(controllerSpeed, 1.2f) * speedMultiplier;
    rb.linearVelocity = flyDirection * initialThrowSpeed;

    if (controllerSpeed > speedThreshold)
    {
       Vector3 motionDir = controllerVelocity.normalized;

       // The axis of rotation should be perpendicular to both the flight direction and the swinging direction.
       spinAxis = Vector3.Cross(flyDirection, motionDir).normalized;

       if (spinAxis == Vector3.zero || float.IsNaN(spinAxis.x))
       {
          // When the cross product approaches zero, use the upward direction of the controller as the rotation axis.
          spinAxis = rightControllerDirection.up;
       }

       rb.inertiaTensor = Vector3.one;
       rb.inertiaTensorRotation = Quaternion.identity;
       rb.angularVelocity = spinAxis * 50f;

       //Debug
       Debug.Log($"[DEBUG] flyDir: {flyDirection}, motionDir: {motionDir}, spinAxis: {spinAxis}");
    }

    // Initialize flight state
    flightTimer = 0f;
    isFlyingOut = true;
    col.enabled = true;

    // Set up return timing
    canReturn = false;
    Invoke(nameof(EnableReturn), minFlyTime);
    Invoke(nameof(StartReturn), returnDelay);
}


    // === Enable returning after minFlyTime ===
    private void EnableReturn()
    {
        canReturn = true;
    }

    // === Start moving back to the hand ===
    private void StartReturn()
    {
        isReturning = true;
        isFlyingOut = false;
        
        returnSpeed = initialThrowSpeed * 0.8f;
        returnTimer = 0f;
        
        float returnAngularSpeed = 20f;
        rb.angularVelocity = spinAxis * returnAngularSpeed;
    }

    // === Finalize return and attach to hand ===
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
    }

    // === Attach boomerang to controller ===
    private void AttachToController()
    {
        transform.SetParent(rightControllerDirection);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}