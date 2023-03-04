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

    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("logged in as user: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateNewLobby() {
        try {

            string nameOfTheLobby = serverNameInput.text;
            int maxPlayersAllowedInLobby = Mathf.RoundToInt(serverPlayersInput.value);

            Lobby newLobby = await LobbyService.Instance.CreateLobbyAsync(nameOfTheLobby, maxPlayersAllowedInLobby);

            await LobbyService.Instance.JoinLobbyByCodeAsync(newLobby.LobbyCode);

            Debug.Log("Lobby Name: "+ newLobby.Name + " Lobby Max Player Count: " + newLobby.MaxPlayers);

        } catch (LobbyServiceException error) {
            Debug.Log(error);
        }

    }

    public async void DisplayLobbyList() {
        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync();


        foreach (Lobby lobby in response.Results) {
            Debug.Log("Found Lobby: " + lobby.Name + " with max player count of: " + lobby.MaxPlayers + " and ID of " + lobby.Id);
        }
    }

}
