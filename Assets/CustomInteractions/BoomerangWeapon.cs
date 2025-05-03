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
    public float returnDelay = 1.5f;              // Time after throw before returning
    public float spinTorque = 300f;               // Amount of spin force when flying
    public float minFlyTime = 0.2f;               // Minimum time before allowing return
    public float throwDelay = 0.07f;              // Delay after button press before actual throw
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
    private bool isThrowScheduled = false;        // Whether a delayed throw has been scheduled

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
            Debug.Log("[BOOMERANG DEBUG] Start: Boomerang is initially held");
        }
        else
        {
            isHeld = false;
            Debug.Log("[BOOMERANG DEBUG] Start: Boomerang not held");
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

        if (isHeld && isAPressed && !isThrowScheduled)
        {
            isThrowScheduled = true;
            Invoke(nameof(DelayedThrow), throwDelay);
            Debug.Log("[BOOMERANG DEBUG] Throw scheduled after delay");
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

    // === Called after scheduled throw delay ===
    private void DelayedThrow()
    {
        isThrowScheduled = false;

        if (isHeld)
            Throw();
        else
            Debug.Log("[BOOMERANG DEBUG] Throw canceled: not held anymore");
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
        Debug.Log("[BOOMERANG DEBUG] Grabbed boomerang!");
    }

    // === Handle release (select exited) ===
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

    // === Core throwing logic ===
    private void Throw()
    {
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

        // Read controller velocity
        _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 localVelocity);
        _inputData._rightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angularVel);

        Vector3 controllerVelocity = rightControllerDirection.TransformDirection(localVelocity);

        float controllerSpeed = controllerVelocity.magnitude;
        float speedMultiplier = 8f;
        float minSpeed = 5f;

        // Decide throw direction and speed
        if (controllerVelocity.magnitude > 0.1f)
        {
            flyDirection = controllerVelocity.normalized;
            initialThrowSpeed = Mathf.Pow(controllerSpeed, 1.2f) * speedMultiplier;
        }
        else
        {
            flyDirection = rightControllerDirection.forward;
            initialThrowSpeed = minSpeed;
        }

        rb.linearVelocity = flyDirection * initialThrowSpeed;

        spinAxis = flyDirection;

        // Initial spin
        rb.AddTorque(spinAxis * spinTorque, ForceMode.Impulse);

        flightTimer = 0f;
        isFlyingOut = true;
        col.enabled = true;

        canReturn = false;
        Invoke(nameof(EnableReturn), minFlyTime);
        Invoke(nameof(StartReturn), returnDelay);

        Debug.Log($"[BOOMERANG DEBUG] Thrown. Direction: {flyDirection}, Speed: {initialThrowSpeed}");
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

        Debug.Log("[BOOMERANG DEBUG] Returned to hand");
    }

    // === Attach boomerang to controller ===
    private void AttachToController()
    {
        transform.SetParent(rightControllerDirection);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
