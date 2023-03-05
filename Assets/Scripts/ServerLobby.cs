using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private TextMeshProUGUI serverNameInput;

    [Header("Server Players Object")]
    [SerializeField] private Slider serverPlayersInput;

    [Header("Server Players Display Object")]
    [SerializeField] private TextMeshProUGUI sliderValueDisplay;

    [Header("Server Public Lobby Object")]
    [SerializeField] private Toggle publicServerInput;

    [Header("Server Name Object")]
    [SerializeField] private TextMeshProUGUI serverCodeInput;

    private Lobby currentlyConnectedLobby;
    private string playerID;


    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            playerID = AuthenticationService.Instance.PlayerId;
            Debug.Log("logged in as user: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    async void KeepLobbyAliveWhileHostJoined() {
        if (currentlyConnectedLobby is not null) {
            Debug.Log("HeartBeat sent");
            await LobbyService.Instance.SendHeartbeatPingAsync(currentlyConnectedLobby.Id);
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

    }

    public void ChangeMaxPlayerHandleValue() {
        sliderValueDisplay.text = Mathf.RoundToInt(serverPlayersInput.value).ToString();
    }

    public async void JoinLobby() {
        Debug.Log(serverCodeInput.text);
        Debug.Log(serverCodeInput.text.Length);
        if (serverCodeInput.text is not null && serverCodeInput.text.Length == 7) {

            try {

                Debug.Log("Private");
                currentlyConnectedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(serverCodeInput.text);
                Debug.Log(currentlyConnectedLobby.Players);

            } catch (LobbyServiceException error) {

                Debug.Log(error);

            }
        }
        else {

            try {

                Debug.Log("Public");
                currentlyConnectedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                Debug.Log(currentlyConnectedLobby.Players);

            } catch (LobbyServiceException error) {

                Debug.Log(error);

            }
        }
    }

    public async void LeaveLobby() {
        try {

            if (playerID == currentlyConnectedLobby.HostId) {
                await LobbyService.Instance.DeleteLobbyAsync(currentlyConnectedLobby.Id);
            }
            else {
                await LobbyService.Instance.RemovePlayerAsync(currentlyConnectedLobby.Id, playerID);
            }

        } catch (LobbyServiceException error) {

            Debug.Log(error);

        }
    }

}
