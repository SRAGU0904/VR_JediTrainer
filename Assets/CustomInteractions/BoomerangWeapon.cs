using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR;
using System.Collections;

[RequireComponent(typeof(InputData))]
public class BoomerangWeapon : MonoBehaviour
{
    [Header("References")]
    public Transform rightControllerDirection;

    [Header("Throw Settings")]
    public float throwForce = 20f;
    public float accelerationTime = 1.0f;
    private float returnDelay = 2f;
    private float spinTorque = 5000f;
    private float minFlyTime = 0.2f;

    private Rigidbody rb;
    private Collider col;
    private XRGrabInteractable interactable;
    private IXRSelectInteractor interactor;

    private bool isHeld = false;
    private bool isFlyingOut = false;
    private bool isReturning = false;
    private bool canReturn = false;

    private Vector3 flyDirection;
    private float flightTimer = 0f;

        private InputData _inputData;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        interactable = GetComponent<XRGrabInteractable>();

        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);

         _inputData = GetComponent<InputData>();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        _inputData._rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool isTriggerPressed);

        if (isHeld && isTriggerPressed)
        {
            Throw();
        }
    }

    private void HandleMovement()
    {
        if (isFlyingOut)
        {
            flightTimer += Time.deltaTime;
            float t = Mathf.Clamp01(flightTimer / accelerationTime);
            rb.linearVelocity = flyDirection * (throwForce * t);
        }

        if (isReturning)
        {
            Vector3 dir = (rightControllerDirection.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, rightControllerDirection.position);
            float boost = Mathf.Lerp(throwForce, throwForce * 1.5f, 1f - Mathf.Clamp01(distance / 5f));
            rb.linearVelocity = dir * boost;

            if (distance < 0.5f)
            {
                CompleteReturn();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isFlyingOut && !isReturning && canReturn)
        {
            StartReturn();
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
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    private void Throw()
    {
        if (interactor != null && interactable.interactionManager != null)
        {
            interactable.interactionManager.SelectExit(interactor, interactable);
        }

        isHeld = false;
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.angularDamping = 0f;

        flyDirection = rightControllerDirection.forward;
        flightTimer = 0f;
        isFlyingOut = true;
        col.enabled = true;

        canReturn = false;
        Invoke(nameof(EnableReturn), minFlyTime);

        rb.AddTorque(transform.right * spinTorque, ForceMode.Impulse);

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
    }

    private void AttachToController()
    {
        transform.SetParent(rightControllerDirection);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
