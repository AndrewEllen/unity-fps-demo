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

    [SerializeField] private Vector2 minMaxRotationOnXAxis;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private NetworkMovement playerMovement;

    [SerializeField] private float mass;

    private PhysicsConstants physics;
    private CharacterController characterController;
    private PlayerControls playerControls;
    public Transform groundCheck;

    private float cameraAngle;
    
    public float jumpHeight = 3f;
    public float groundDis = 0.4f;
    public LayerMask groundLayerMask;
    bool isGrounded;
    Vector3 jumpVelocityVector;


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

        physics = new PhysicsConstants();

        Cursor.lockState = CursorLockMode.Locked;

    }


    void Update() {

        Vector3 movementInput = playerControls.Player.Move.ReadValue<Vector3>();
        bool playerJump = playerControls.Player.Jump.triggered;
        Vector2 playerLookInput2D = playerControls.Player.Look.ReadValue<Vector2>();

        if (IsClient && IsLocalPlayer) {
            
            playerMovement.ProcessLocalPlayerMovement(movementInput, playerLookInput2D);

        } else {
            
            playerMovement.ProcessSimulatedPlayerMovement();  

        }
        
    }

/*
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

    private void JumpPlayer(bool jumpInput) {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDis, groundLayerMask);

        if (isGrounded) {
            jumpVelocityVector.y = 0;
            if (jumpInput) {

                Debug.Log("Jumping");

                jumpVelocityVector.y = jumpHeight;
            }

        } else {
            jumpVelocityVector.y = physics.objectGravity(mass, jumpVelocityVector.y, transform.localScale.x, transform.localScale.z, true);
        }

        characterController.Move(jumpVelocityVector * Time.deltaTime);

    }

    private void MovePlayer(Vector3 movementInput) {

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.z;
        
        characterController.Move(move * playerSpeed * Time.deltaTime);


    }

    [ServerRpc]
    private void MoveServerRPC(Vector3 movementInput, Vector2 rotationInput, bool jumpInput) {

        MovePlayer(movementInput);
        JumpPlayer(jumpInput);
        RotatePlayer(rotationInput);

    }
*/

}
