using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MyGun : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    private InputAction fireAction;

    public GameObject myBullet;
    [SerializeField] public BulletScriptableObject bulletInfo;
    [SerializeField] private Transform myBulletSpawnTransform;
    [SerializeField] private float myPower;
    [SerializeField] GameObject myPlayer;
    private void Awake()
    {
        fireAction = actionAsset.FindAction("Fire");
        myBullet = bulletInfo.myBullet;
    }


    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (fireAction.triggered)
        {
            
            GameObject bullet = Instantiate(myBullet, myBulletSpawnTransform.position, myBulletSpawnTransform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(myBulletSpawnTransform.forward * myPower);
            bullet.GetComponent<BulletScript>().SetMyPlayer(myPlayer);
        }

       
    }


 
}
