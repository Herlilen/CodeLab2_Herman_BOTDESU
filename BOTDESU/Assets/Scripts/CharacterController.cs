using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CharacterController : MonoBehaviour
{
    public float smallMoveSpeed = 3f;
    public float largeMoveSpeed = 7f;
    private float currentMoveSpeed;
    private Rigidbody rb;
    private PlayerInput inputActions;
    private Vector2 moveInput;
    public Transform cameraTransform; // Reference to the camera's transform
    public Transform virtualCameraTransform; // Reference to the virtual camera's transform
    public Vector3 smallScale = new Vector3(1f, 1f, 1f); // Small scale
    public Vector3 largeScale = new Vector3(2f, 2f, 2f); // Large scale
    public float cameraDistanceMultiplier = 2f; // Multiplier to increase the camera distance

    // References to Cinemachine rigs
    public CinemachineFreeLook cinemachineFreeLook;
    public float topRigHeight = 10f;
    public float midRigHeight = 5f;
    public float bottomRigHeight = 2f;
    public float topRigWidth = 2f;
    public float midRigWidth = 1.5f;
    public float bottomRigWidth = 1f;

    // Original settings (now public and editable)
    public Vector3 originalScale;
    public float originalMoveSpeed;
    public float originalTopRigHeight;
    public float originalMidRigHeight;
    public float originalBottomRigHeight;
    public float originalTopRigWidth;
    public float originalMidRigWidth;
    public float originalBottomRigWidth;
    public Vector3 originalCameraPosition;

    public float scaleDuration = 0.5f; // Variable to control the scale time
    public float cameraAdjustDuration = 0.5f; // Variable to control the camera adjustment time

    private bool isLargeState = false;
    private Coroutine scaleCoroutine;
    private Coroutine cameraCoroutine;

    private Animator animator; // Reference to the Animator component

    public float smallMaxSpeed = 10f; // Maximum speed cap for small scale
    public float largeMaxSpeed = 15f; // Maximum speed cap for large scale

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInput();
        animator = GetComponent<Animator>(); // Initialize the Animator component
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.HenShin.performed += OnHenShin;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.HenShin.performed -= OnHenShin;
        inputActions.Player.Disable();
    }

    void Start()
    {
        // Initialize current settings with original settings
        currentMoveSpeed = originalMoveSpeed;
        ResetCinemachineRigs();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        // Calculate the magnitude of the moveInput vector and pass it to the animator
        float moveMagnitude = moveInput.magnitude;
        animator.SetFloat("MoveMagnitude", moveMagnitude);
    }

    void OnHenShin(InputAction.CallbackContext context)
    {
        isLargeState = !isLargeState;
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(SmoothScale(isLargeState ? largeScale : smallScale));
        currentMoveSpeed = isLargeState ? largeMoveSpeed : smallMoveSpeed;
        if (isLargeState)
        {
            AdjustCameraDistance();
            if (cameraCoroutine != null)
            {
                StopCoroutine(cameraCoroutine);
            }

            cameraCoroutine = StartCoroutine(SmoothAdjustCinemachineRigs(topRigHeight, midRigHeight, bottomRigHeight,
                topRigWidth, midRigWidth, bottomRigWidth));
        }
        else
        {
            if (cameraCoroutine != null)
            {
                StopCoroutine(cameraCoroutine);
            }

            cameraCoroutine = StartCoroutine(SmoothAdjustCinemachineRigs(originalTopRigHeight, originalMidRigHeight,
                originalBottomRigHeight, originalTopRigWidth, originalMidRigWidth, originalBottomRigWidth));
        }
    }

    IEnumerator SmoothScale(Vector3 targetScale)
    {
        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / scaleDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    IEnumerator SmoothAdjustCinemachineRigs(float targetTopHeight, float targetMidHeight, float targetBottomHeight,
        float targetTopWidth, float targetMidWidth, float targetBottomWidth)
    {
        float initialTopHeight = cinemachineFreeLook.m_Orbits[0].m_Height;
        float initialMidHeight = cinemachineFreeLook.m_Orbits[1].m_Height;
        float initialBottomHeight = cinemachineFreeLook.m_Orbits[2].m_Height;
        float initialTopWidth = cinemachineFreeLook.m_Orbits[0].m_Radius;
        float initialMidWidth = cinemachineFreeLook.m_Orbits[1].m_Radius;
        float initialBottomWidth = cinemachineFreeLook.m_Orbits[2].m_Radius;
        float elapsed = 0f;

        while (elapsed < cameraAdjustDuration)
        {
            cinemachineFreeLook.m_Orbits[0].m_Height =
                Mathf.Lerp(initialTopHeight, targetTopHeight, elapsed / cameraAdjustDuration);
            cinemachineFreeLook.m_Orbits[1].m_Height =
                Mathf.Lerp(initialMidHeight, targetMidHeight, elapsed / cameraAdjustDuration);
            cinemachineFreeLook.m_Orbits[2].m_Height =
                Mathf.Lerp(initialBottomHeight, targetBottomHeight, elapsed / cameraAdjustDuration);
            cinemachineFreeLook.m_Orbits[0].m_Radius =
                Mathf.Lerp(initialTopWidth, targetTopWidth, elapsed / cameraAdjustDuration);
            cinemachineFreeLook.m_Orbits[1].m_Radius =
                Mathf.Lerp(initialMidWidth, targetMidWidth, elapsed / cameraAdjustDuration);
            cinemachineFreeLook.m_Orbits[2].m_Radius =
                Mathf.Lerp(initialBottomWidth, targetBottomWidth, elapsed / cameraAdjustDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cinemachineFreeLook.m_Orbits[0].m_Height = targetTopHeight;
        cinemachineFreeLook.m_Orbits[1].m_Height = targetMidHeight;
        cinemachineFreeLook.m_Orbits[2].m_Height = targetBottomHeight;
        cinemachineFreeLook.m_Orbits[0].m_Radius = targetTopWidth;
        cinemachineFreeLook.m_Orbits[1].m_Radius = targetMidWidth;
        cinemachineFreeLook.m_Orbits[2].m_Radius = targetBottomWidth;
    }

    void AdjustCameraDistance()
    {
        Vector3 directionToCamera = virtualCameraTransform.position - transform.position;
        virtualCameraTransform.position = transform.position + directionToCamera.normalized * cameraDistanceMultiplier;
    }

    void ResetCinemachineRigs()
    {
        if (cinemachineFreeLook != null)
        {
            cinemachineFreeLook.m_Orbits[0].m_Height = originalTopRigHeight;
            cinemachineFreeLook.m_Orbits[0].m_Radius = originalTopRigWidth;
            cinemachineFreeLook.m_Orbits[1].m_Height = originalMidRigHeight;
            cinemachineFreeLook.m_Orbits[1].m_Radius = originalMidRigWidth;
            cinemachineFreeLook.m_Orbits[2].m_Height = originalBottomRigHeight;
            cinemachineFreeLook.m_Orbits[2].m_Radius = originalBottomRigWidth;
        }
    }

    void FixedUpdate()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Keep the movement horizontal
        Quaternion cameraRotation = Quaternion.LookRotation(cameraForward);
        Vector3 adjustedMove = cameraRotation * move * currentMoveSpeed;

        rb.AddForce(adjustedMove, ForceMode.Impulse);

        // Cap the speed based on the current state
        float currentMaxSpeed = isLargeState ? largeMaxSpeed : smallMaxSpeed;
        if (rb.velocity.magnitude > currentMaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(adjustedMove);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation,
                    Time.deltaTime * 10f); // Adjust the 10f to control the rotation speed
        }
    }
}