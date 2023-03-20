using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_player_health : MonoBehaviour
{
    [SerializeField] private int health;
    int damageTaken = 0;
    [SerializeField] Test_Health_tracker_damage test_Health_Tracker_Damage;
    [SerializeField] Slider healthSlider;
    private void Awake()
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    public void UpdateHeath(int damageTaken, string myTag) {

        test_Health_Tracker_Damage.updateUI(damageTaken, myTag);

        healthSlider.value = health -= damageTaken;
       
        Debug.Log("target health is: " + health);

        if (health <= 0) {
            Destroy(this.gameObject);
        }
    }
}
