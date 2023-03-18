using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletScript : MonoBehaviour
{
    private Rigidbody rb;
    private float bulletSpeed = 5f;

    private AudioSource myPlayerAudio;

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


    private void OnCollisionEnter(Collision collision)
    {
        /// take health from collsion if player
        /// partical effect if want
        /// inform player who shot bullet that it hit
        /// destory bullet
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("hello");
            collision.gameObject.GetComponent<Test_player_health>().UpdateHeath();
            myPlayerAudio.Play();
        }

        Destroy(this.gameObject);
    }


    public void SetMyPlayer(GameObject myPlayer)
    { 
        myPlayerAudio = myPlayer.GetComponent<AudioSource>();
    }
}
