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

    private float cameraAngle;


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

        if (IsLocalPlayer) {
            
            if (playerControls.Player.Move.inProgress) {

                Vector2 playerMovementInput2D = playerControls.Player.Move.ReadValue<Vector2>();
                Vector3 playerMovement3D = playerMovementInput2D.x * cameraTransform.right + playerMovementInput2D.y * cameraTransform.forward;

                playerMovement3D.y = 0;

                characterController.Move(playerMovement3D * playerSpeed * Time.deltaTime);

            }

            if (playerControls.Player.Look.inProgress) {

                Vector2 playerLookInput2D = playerControls.Player.Look.ReadValue<Vector2>();
                
                transform.RotateAround(transform.position, transform.up, playerLookInput2D.x * playerRotationSpeed * Time.deltaTime);
                
                RotateCamera(playerLookInput2D.y);
                
            }

        }
        
    }

    private void RotateCamera(float inputYAxis) {

        cameraAngle = Vector3.SignedAngle(transform.forward, cameraTransform.forward, cameraTransform.right);

        float cameraRotationAmount = inputYAxis * playerRotationSpeed * Time.deltaTime;
        float newCameraAngle = cameraAngle - cameraRotationAmount;

        if (newCameraAngle <= minMaxRotationOnXAxis.x && newCameraAngle >= minMaxRotationOnXAxis.y) {

            cameraTransform.RotateAround(cameraTransform.position, cameraTransform.right, -inputYAxis * playerRotationSpeed * Time.deltaTime);

        }

    }

}
