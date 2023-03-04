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

    private async void CreateLobby() {
        try {

            string nameOfTheLobby = serverNameInput.text;
            int maxPlayersAllowedInLobby = Mathf.RoundToInt(serverPlayersInput.value);

            Lobby newLobby = await LobbyService.Instance.CreateLobbyAsync(nameOfTheLobby, maxPlayersAllowedInLobby);

            Debug.Log("Lobby Name: "+ newLobby.Name + " Lobby Max Player Count: " + newLobby.MaxPlayers);

        } catch (LobbyServiceException error) {
            Debug.Log(error);
        }

    }

}
