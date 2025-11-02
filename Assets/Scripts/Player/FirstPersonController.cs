using Unity.Netcode;
using UnityEngine;

public class FirstPersonController : NetworkBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [Range(0.1f, 30f)]
    [SerializeField] private float mouseSensitivity = 25.0f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerMovement playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;

    private void Start()
    {

        if (!IsOwner)
        {
            return;
        }
        mainCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.jumpTriggered)
                currentMovement.y = jumpForce;
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector2 input = playerInputHandler.MovementInput;
        bool sprint = playerInputHandler.SprintTriggered;

        Vector3 inputDirection = new Vector3(input.x, 0f, input.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        currentMovement.x = worldDirection.x * walkSpeed * (sprint ? sprintMultiplier : 1);
        currentMovement.z = worldDirection.z * walkSpeed * (sprint ? sprintMultiplier : 1);

        HandleJumping();
        characterController.Move(currentMovement * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = playerInputHandler.RotationInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = playerInputHandler.RotationInput.y * mouseSensitivity * Time.deltaTime;

        transform.Rotate(0, mouseX, 0);

        verticalRotation = Mathf.Clamp(verticalRotation - mouseY, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
