using UnityEngine;


public class MainMenuScript : MonoBehaviour
{

    [Header("Menu's")]
    public GameObject[] menuList;

    public void DisplayMenu(GameObject menusToSwitch) {
        menusToSwitch.gameObject.SetActive(!menusToSwitch.gameObject.activeSelf);
    }

    public void ExitGame() {
        Debug.Log("Hello");
        Application.Quit();
    }
}
