using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MouseControl : MonoBehaviour
{
    [SerializeField]private InputActionAsset actionAsset;
    private InputAction lookAction;

    [SerializeField ]private GameObject my_Body;
    CharacterController characterController;
    [SerializeField]float mouseSensitivity = 0.1f;

    private Camera myCamera;
    // Start is called before the first frame update
    private void Awake()
    {
        lookAction = actionAsset.FindAction("Look");
        characterController = GetComponent<CharacterController>();
       
    }
        void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 m_Input = new Vector3(0, lookAction.ReadValue<Vector2>().x, 0);
        my_Body.transform.Rotate(m_Input * Time.deltaTime * mouseSensitivity);

        //myCamera.gameObject.transform.Rotate(new Vector3(m_Input.y * Time.deltaTime * mouseSensitivity, 0, 0));

    }
}
