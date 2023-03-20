using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_player_health : MonoBehaviour
{
    [SerializeField] private int health;
    int damageTaken = 0;


    public void UpdateHeath(int damageTaken) {

        health -= damageTaken;

        Debug.Log("target health is: " + health);

        if (health <= 0) {
            Destroy(this.gameObject);
        }
    }
}
