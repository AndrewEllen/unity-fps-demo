using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    private Rigidbody rb;
    private float bulletSpeed = 5f;

    private AudioSource myPlayerAudio;

    private string[] playerModelTagList = {"body","head","enemy"};

    private int damageMultiplier;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += rb.velocity * Time.deltaTime;

        if (transform.position.y < -10 || transform.position.x > 5000 | transform.position.z > 5000)
        {
            Destroy(this.gameObject);
        }
    }

    private int getDamageMultipliers(string bodyTag) {
        int damageMultiplierValue = 1;

        if (bodyTag == "head") {
            damageMultiplierValue = 3;
        } 
        else if (bodyTag == "body") {
            damageMultiplierValue = 1;
        }

        return damageMultiplierValue;
    }


    private void OnCollisionEnter(Collision collision)
    {

        try {
            if (playerModelTagList.Contains(collision.gameObject.tag.ToLower())) {

                damageMultiplier = getDamageMultipliers(collision.gameObject.tag.ToLower());

                Debug.Log("hello");

                //the default damage value. Could be changed depending on the gun/bullet
                int damageValue = 1;

                Debug.Log(damageMultiplier);

                collision.gameObject.GetComponentInParent<Test_player_health>().UpdateHeath(damageValue * damageMultiplier);
                myPlayerAudio.Play();
            }
        } catch {

        }
        /// take health from collsion if player
        /// partical effect if want
        /// inform player who shot bullet that it hit
        /// destory bullet
        Destroy(this.gameObject);
    }


    public void SetMyPlayer(GameObject myPlayer)
    { 
        myPlayerAudio = myPlayer.GetComponent<AudioSource>();
    }
}
