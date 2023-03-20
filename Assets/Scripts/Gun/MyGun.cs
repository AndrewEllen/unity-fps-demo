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

    private bool _allowedToFire;

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
    void Update() {

        //Followed help on forum here https://answers.unity.com/questions/132154/how-to-limit-the-players-rate-of-fire.html
        
        if (fireAction.triggered && _allowedToFire) {
            StartCoroutine(FireWhilePressed());
        }

    }

    IEnumerator FireWhilePressed() {

        _allowedToFire = false;

        GameObject bullet = Instantiate(myBullet, myBulletSpawnTransform.position, myBulletSpawnTransform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(myBulletSpawnTransform.forward * myPower);
        bullet.GetComponent<BulletScript>().SetMyPlayer(myPlayer);

        yield return new WaitForSeconds(1f);
        _allowedToFire = true;

    }


 
}
