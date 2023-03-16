using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_player_health : MonoBehaviour
{
    [SerializeField] private Material[] healths;
    int count = 0;

    private void Awake()
    {
        this.GetComponent<MeshRenderer>().material = healths[count];
    }


    public void UpdateHeath()
    {
        count++;
        if (count < healths.Length)
            this.GetComponent<MeshRenderer>().material = healths[count];
        else
            Destroy(this.gameObject);
    }
}
