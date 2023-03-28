using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;


// Tutorial Followed
// https://www.youtube.com/watch?v=2rYjg5N4YZc

[RequireComponent(typeof(CharacterController))]

public class PlayerControllerScript : NetworkBehaviour
{

    [SerializeField] private float playerSpeed;

    [SerializeField] private float playerRotationSpeed;

    [SerializeField] private Vector2 minMaxRotationOnXAxis;

    [SerializeField] private Transform cameraTransform;

    private CharacterController characterController;
    private PlayerControls playerControls;
    public Transform groundCheck;

    private float cameraAngle;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDis = 0.4f;
    public LayerMask groundLayerMask;
    bool isGrounded;
    Vector3 velocity;
    [SerializeField] private float MovementSpeed = 2.0f;











    public override void OnNetworkSpawn() {

        CinemachineVirtualCamera playerCamera = cameraTransform.gameObject.GetComponent<CinemachineVirtualCamera>();

        if(IsOwner) {
            playerCamera.Priority = 1;
        } else {
            playerCamera.Priority = 0;
        }

    }


    void Start() {
        
        characterController = GetComponent<CharacterController>();

        playerControls = new PlayerControls();
        playerControls.Enable();

        Cursor.lockState = CursorLockMode.Locked;

    }


    void Update() {

        Vector3 movementInput = playerControls.Player.Move.ReadValue<Vector3>();
        Vector2 playerLookInput2D = playerControls.Player.Look.ReadValue<Vector2>();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDis, groundLayerMask);

        if (IsServer && IsLocalPlayer) {
            
            MoveServerRPC(movementInput, playerLookInput2D);
            RotatePlayer(playerLookInput2D);  
            RotateCameraOnClient(playerLookInput2D);


        } else if (IsLocalPlayer) {
            
            MovePlayer(movementInput);
            RotatePlayer(playerLookInput2D);           

        }

        if (isGrounded & velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
    }

    private void RotatePlayer(Vector2 rotationInput) {

        transform.RotateAround(transform.position, transform.up, rotationInput.x * playerRotationSpeed * Time.deltaTime);

    }

    private void RotateCameraOnClient(Vector2 rotationInput) {

        cameraAngle = Vector3.SignedAngle(transform.forward, cameraTransform.forward, cameraTransform.right);

        float cameraRotationAmount = rotationInput.y * playerRotationSpeed * Time.deltaTime;
        float newCameraAngle = cameraAngle - cameraRotationAmount;

        if (newCameraAngle <= minMaxRotationOnXAxis.x && newCameraAngle >= minMaxRotationOnXAxis.y) {

            cameraTransform.RotateAround(cameraTransform.position, cameraTransform.right, -rotationInput.y * playerRotationSpeed * Time.deltaTime);

        }

    }

    private void MovePlayer(Vector3 movementInput) {

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.z;
        
        characterController.Move(move * MovementSpeed * Time.deltaTime);

        if (playerControls.Player.Jump.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

    }

    [ServerRpc]
    private void MoveServerRPC(Vector3 movementInput, Vector2 rotationInput) {

        MovePlayer(movementInput);
        RotatePlayer(rotationInput);

    }

}
