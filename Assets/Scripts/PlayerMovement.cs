using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputActionAsset actionAsset;
    private InputAction jumpAction;
    public InputAction moveAction;
    GameObject jumpTarget;

    [SerializeField]private float MovementSpeed = 2.0f;
    Vector3 velocity;

    public float gravity = -9.81f;

    CharacterController characterController;

    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDis = 0.4f;
    public LayerMask groundLayerMask;
    bool isGrounded;


    private void Awake()
    {
        jumpAction = actionAsset.FindAction("Jump");
        moveAction = actionAsset.FindAction("Move");
        characterController = GetComponent<CharacterController>();
       
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDis, groundLayerMask);

        if (isGrounded & velocity.y < 0)
        { 
            velocity.y = -2f;
        }

        characterController.Move(moveAction.ReadValue<Vector3>() * MovementSpeed * Time.deltaTime);

        if (jumpAction.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

       
    }

    private void OnEnable()
    {
        moveAction.Enable();
        actionAsset.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        actionAsset.Disable();
    }


}
