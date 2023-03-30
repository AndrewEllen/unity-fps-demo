using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//Tutorial:
//https://www.youtube.com/watch?v=leL6MdkJEaE

public class NetworkMovement : NetworkBehaviour {

    [SerializeField] CharacterController playerCharacterController;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerRotationSpeed;
    [SerializeField] private Transform cameraSocketObject;
    [SerializeField] private GameObject virtualCamera;

    private Transform virtualCameraTransform;
    private int tick = 0;
    private float tickRate = 1f/60f;
    private float tickDeltaTime;

    private const int BUFFER_SIZE = 1024;
    private InputState[] inputStates = new InputState[BUFFER_SIZE];
    private TransformState[] transformStates = new TransformState[BUFFER_SIZE];

    public NetworkVariable<TransformState> serverTransformState = new NetworkVariable<TransformState>();
    public TransformState previousTransformState;
    
    private void OnEnable() {
        
        serverTransformState.OnValueChanged += OnServerStateChanged;

    }

    public override void OnNetworkSpawn() {

        base.OnNetworkSpawn();
        virtualCameraTransform = virtualCamera.transform;

    }

    //Player Reconcilliation will go in here later
    private void OnServerStateChanged(TransformState previousValue, TransformState newValue) {

        previousTransformState = previousValue;

    }

    public void ProcessLocalPlayerMovement(Vector3 movementInput, Vector2 cameraInput) {

        tickDeltaTime += Time.deltaTime;
        if (tickDeltaTime > tickRate) {

            int bufferIndex = tick % BUFFER_SIZE;

            if (!IsServer) {

                MovePlayerServerRPC(tick, movementInput, cameraInput);
                MovePlayer(movementInput);
                RotatePlayer(cameraInput);

            } else {

                MovePlayer(movementInput);
                RotatePlayer(cameraInput);

                TransformState state = new TransformState() {

                    tickState = tick,
                    positionState = transform.position,
                    rotationState = transform.rotation,
                    hasStartedMovingState = true

                };

                previousTransformState = serverTransformState.Value;
                serverTransformState.Value = state;

            }

            InputState inputState = new InputState() {
                
                tickState = tick,
                movementInputState = movementInput,
                cameraInputState = cameraInput,

            };

            TransformState transformState = new TransformState() {

                tickState = tick,
                positionState = transform.position,
                rotationState = transform.rotation,
                hasStartedMovingState = true

            };

            inputStates[bufferIndex] = inputState;
            transformStates[bufferIndex] = transformState;

            tickDeltaTime -= tickRate;
            tick++;
            
        }

    }

    public void ProcessSimulatedPlayerMovement() {

        tickDeltaTime += Time.deltaTime;
        
        if (tickDeltaTime > tickRate) {
            
            if (serverTransformState.Value.hasStartedMovingState) {

                transform.position = serverTransformState.Value.positionState;
                transform.rotation = serverTransformState.Value.rotationState;

            }

            tickDeltaTime -= tickRate;
            tick++;

        }

    }

    private void MovePlayer(Vector3 movementInput) {

        Vector3 playerMovement = virtualCameraTransform.right * movementInput.x + virtualCameraTransform.forward * movementInput.z;

        if (!playerCharacterController.isGrounded) {
            playerMovement.y = -9.81f;
        }
        
        playerCharacterController.Move(playerMovement * playerSpeed * tickRate);

    }

    private void RotatePlayer(Vector2 cameraInput) {
        
        virtualCameraTransform.RotateAround(virtualCameraTransform.position, virtualCameraTransform.right, -cameraInput.y * playerRotationSpeed * tickRate);
        transform.RotateAround(transform.position, transform.up, cameraInput.x * playerRotationSpeed * tickRate);

    }

    [ServerRpc]
    private void MovePlayerServerRPC(int tick, Vector3 movementInput, Vector2 cameraInput) {

        //Can put testing here for server packets lost. Do this by testing previous tick number

        MovePlayer(movementInput);
        RotatePlayer(cameraInput);

        TransformState state = new TransformState() {

            tickState = tick,
            positionState = transform.position,
            rotationState = transform.rotation,
            hasStartedMovingState = true,

        };

        previousTransformState = serverTransformState.Value;
        serverTransformState.Value = state;

    }

}
