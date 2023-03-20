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

    private string[] playerModelTagList = {"enemy","head","neck","uppertorso","lowertorso","arm","hand","leg","foot"};

    private int damageMultiplier;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
            damageMultiplierValue = 10;
        } 
        else if (bodyTag == "neck") {
            damageMultiplierValue = 9;
        }
        else if (bodyTag == "uppertorso") {
            damageMultiplierValue = 5;
        }
        else if (bodyTag == "lowertorso") {
            damageMultiplierValue = 4;
        }
        else if (bodyTag == "arm") {
            damageMultiplierValue = 3;
        }
        else if (bodyTag == "hand") {
            damageMultiplierValue = 1;
        }
        else if (bodyTag == "leg") {
            damageMultiplierValue = 3;
        }
        else if (bodyTag == "foot") {
            damageMultiplierValue = 1;
        }

        return damageMultiplierValue;
    }


    private void OnCollisionEnter(Collision collision)
    {
        try {
            if (playerModelTagList.Contains(collision.gameObject.tag.ToLower())) {

                damageMultiplier = getDamageMultipliers(collision.gameObject.tag.ToLower());

                //the default damage value. Could be changed depending on the gun/bullet
                int damageValue = 1;

                collision.gameObject.GetComponentInParent<Test_player_health>().UpdateHeath(damageValue * damageMultiplier, collision.gameObject.tag.ToString());
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
