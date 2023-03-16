using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MouseControl : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    private InputAction lookAction;

    [SerializeField] private GameObject my_Body;
    CharacterController characterController;
    [SerializeField] float LRmouseSensitivity = 0.1f;
    [SerializeField] float UDmouseSensitivty = 0.1f;

    float UDRotation = 0f;
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
        Vector3 m_LRRotationInput = new Vector3(0, lookAction.ReadValue<Vector2>().x, 0);


        Vector3 m_UDRotationInput = new Vector3(lookAction.ReadValue<Vector2>().y, 0, 0);
        UDRotation -= m_UDRotationInput.x * UDmouseSensitivty;
        UDRotation = Mathf.Clamp(UDRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(UDRotation, 0f, 0f);

        my_Body.transform.Rotate(m_LRRotationInput * Time.deltaTime * LRmouseSensitivity);

        //myCamera.gameObject.transform.Rotate(new Vector3(, -70f, 40f), 0, 0), 1);
        // myCamera.gameObject.transform.localRotation = Quaternion.Euler(Mathf.Clamp((-m_YRotationInput.x * Time.deltaTime * mouseSensitivity),-70f,40f), 0f, 0f);
        //myCamera.transform.Rotate(Vector3.up * m_xRotationInput.y);

    }
}
