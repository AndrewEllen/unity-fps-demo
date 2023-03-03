using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public InputActionAsset actionAsset;
    private InputAction jumpAction;
    public InputAction moveAction;


    [SerializeField]private float MovementSpeed = 2.0f;
    public float gravity = -9.81f;
    bool isGrounded;

    CharacterController characterController;


    private void Awake()
    {
        jumpAction = actionAsset.FindAction("Jump");
        moveAction = actionAsset.FindAction("Move");
        characterController = GetComponent<CharacterController>();
        //rb = gameObject.GetComponent<Rigidbody>();
    }
    void Update()
    {
      // isGrounded = Physics.CheckSphere(groun)
        
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
