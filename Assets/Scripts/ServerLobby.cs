using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ServerLobby : MonoBehaviour
{

    [Header("Server Name Object")]
    [SerializeField] private TMP_InputField serverNameInput;

    [Header("Server Players Object")]
    [SerializeField] private Slider serverPlayersInput;

    [Header("Server Players Display Object")]
    [SerializeField] private TextMeshProUGUI sliderValueDisplay;

    [Header("Server Public Lobby Object")]
    [SerializeField] private Toggle publicServerInput;

    [Header("Server Code Object")]
    [SerializeField] private TMP_InputField serverCodeInput;

    [Header("Server Menu")]
    [SerializeField] private GameObject serverMenu;

    [Header("Lobby Menu")]
    [SerializeField] private GameObject lobbyMenu;


    private Lobby currentlyConnectedLobby;
    private string playerID;


    private async void Start() {
        await UnityServices.InitializeAsync();


        //Event listeners for user authentication
        AuthenticationEventListener();


        //Login the user Anonymously
        await AnonymousSignIn();

    }

    void AuthenticationEventListener() {

        AuthenticationService.Instance.SignedIn += () => {

            playerID = AuthenticationService.Instance.PlayerId;

            Debug.Log("logged in as user: " + AuthenticationService.Instance.PlayerId);
            Debug.Log("User Access Token: " + AuthenticationService.Instance.AccessToken);

        };

        AuthenticationService.Instance.SignInFailed += (error) => {
            Debug.LogError(error);
        };

        AuthenticationService.Instance.SignedOut += () => {
            Debug.Log("Signed Out");
        };

    }

    //Using Task instead of Void since you can't await a void function but can await a task.
    async Task AnonymousSignIn() {
        try {

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in Successful");

        } catch (AuthenticationException error) {
            Debug.LogException(error);
        }
    }

    async void KeepLobbyAliveWhileHostJoined() {
        if (currentlyConnectedLobby is not null) {
            Debug.Log("HeartBeat sent");
            await LobbyService.Instance.SendHeartbeatPingAsync(currentlyConnectedLobby.Id);
        }
    }

    async void PollForLobbyUpdates() {
        //todo leave lobby when not found
        if (currentlyConnectedLobby is not null) {
            Lobby currentlyConnectedLobbyUpdate = await LobbyService.Instance.GetLobbyAsync(currentlyConnectedLobby.Id);
            if (currentlyConnectedLobbyUpdate.Players.Count != currentlyConnectedLobby.Players.Count) {

                currentlyConnectedLobby = currentlyConnectedLobbyUpdate;
        
                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                }
            }
        }
    }

    public async void CreateNewLobby() {
        try {

            string nameOfTheLobby = serverNameInput.text;
            int maxPlayersAllowedInLobby = Mathf.RoundToInt(serverPlayersInput.value);

            CreateLobbyOptions newLobbyOptions = new CreateLobbyOptions();

            newLobbyOptions.IsPrivate = !publicServerInput;

            Lobby newLobby = await LobbyService.Instance.CreateLobbyAsync(nameOfTheLobby, maxPlayersAllowedInLobby, newLobbyOptions);

            currentlyConnectedLobby = newLobby;

            InvokeRepeating("KeepLobbyAliveWhileHostJoined", 0f, 15f);

            Debug.Log("Lobby Name: "+ newLobby.Name + " Lobby Max Player Count: " + newLobby.MaxPlayers + " with lobby code of: " + newLobby.LobbyCode);

        } catch (LobbyServiceException error) {

            Debug.Log(error);

        }
        DisplayLobbyScreen(true);
    }

    public void ChangeMaxPlayerHandleValue() {
        sliderValueDisplay.text = Mathf.RoundToInt(serverPlayersInput.value).ToString();
    }

    public void DisplayLobbyScreen(bool _creatingServer) {
        if (_creatingServer) {
            if (currentlyConnectedLobby != null) {
                serverMenu.gameObject.SetActive(false);
                lobbyMenu.gameObject.SetActive(true);
                InvokeRepeating("PollForLobbyUpdates", 0f, 1.2f);
            }
        } else {
            if (currentlyConnectedLobby != null) {
                serverMenu.gameObject.SetActive(false);
                lobbyMenu.gameObject.SetActive(true);
                InvokeRepeating("PollForLobbyUpdates", 0f, 1.2f);
            }
        }
    }

    public async void JoinLobby() {
        Debug.Log(serverCodeInput.text);
        Debug.Log(serverCodeInput.text.Length);
        if (serverCodeInput.text is not null && serverCodeInput.text.Length == 6) {

            try {

                Debug.Log("Private");
                currentlyConnectedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(serverCodeInput.text);
                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                }

            } catch (LobbyServiceException error) {

                Debug.Log(error);

            }
        }
        else {

            try {

                Debug.Log("Public");
                currentlyConnectedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                }

            } catch (LobbyServiceException error) {

                Debug.Log(error);

            }
        }
        DisplayLobbyScreen(false);
    }

    public async void LeaveLobby() {
        try {

            if (playerID == currentlyConnectedLobby.HostId) {
                await LobbyService.Instance.DeleteLobbyAsync(currentlyConnectedLobby.Id);
                currentlyConnectedLobby = null;
                CancelInvoke("KeepLobbyAliveWhileHostJoined");
            }
            else {
                await LobbyService.Instance.RemovePlayerAsync(currentlyConnectedLobby.Id, playerID);
                currentlyConnectedLobby = null;
            }

        } catch (LobbyServiceException error) {

            Debug.Log(error);

        }
        CancelInvoke("PollForLobbyUpdates");
    }

}
