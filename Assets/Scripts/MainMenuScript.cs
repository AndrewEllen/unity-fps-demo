using UnityEngine;


public class MainMenuScript : MonoBehaviour
{

    [Header("Menu's")]
    public GameObject[] menuList;

    public void DisplayMainMenu() {
        menuList[0].gameObject.SetActive(true);
        menuList[1].gameObject.SetActive(false);
        menuList[2].gameObject.SetActive(false);
    }

    public void DisplayServersMenu() {
        menuList[0].gameObject.SetActive(false);
        menuList[1].gameObject.SetActive(true);
    }

    public void DisplaySettingsMenu() {
        menuList[0].gameObject.SetActive(false);
        menuList[2].gameObject.SetActive(true);   
    }

    public void ExitGame() {
        Debug.Log("Hello");
        Application.Quit();
    }
}
