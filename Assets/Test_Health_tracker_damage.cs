using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Test_Health_tracker_damage : MonoBehaviour
{
   [SerializeField] private GameObject[] myText;


    public void updateUI(int DamageTaken, string PointTag)
    {
        foreach (var item in myText)
        {
            if (item.name == PointTag.ToLower())
            {
                item.GetComponentInChildren<TextMeshProUGUI>().text = DamageTaken.ToString();
            }
        }

    }

}