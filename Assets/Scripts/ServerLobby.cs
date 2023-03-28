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
using System;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Networking;

public class ServerLobby : MonoBehaviour
{

    [Header("Network Scene Manager")]
    [SerializeField] private GameObject sceneManager;

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

    [SerializeField] private TextMeshProUGUI lobbyPlayerList;
    
    //public UnityEditor.SceneAsset mapScene;

    private Lobby currentlyConnectedLobby;
    private string playerID;


    //Relay Variables
    //Tutorial being used for this
    //https://www.youtube.com/watch?v=RtBf4v0LjHU 
    private RelayHostData _hostData;
    private RelayJoinData _joinData;



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

    //This function keeps the server alive while the host is in the lobby. Lobbys will be destroyed if the heartbeat is not kept going atleast every 30 seconds.
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
                List<string> playerListString = new List<string> ();

                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                    playerListString.Add(player.Id+"\n");
                }
                lobbyPlayerList.text = string.Join("", playerListString);
                Debug.Log(lobbyPlayerList.text);
            }
        }
    }

    public async void CreateNewLobby() {
        //TODO Relay server shuts down too fast

        try {
            //The value inside CreateAllocationAsync is the external connections
            Allocation relayEntryPointAllocation = await Relay.Instance.CreateAllocationAsync(Mathf.RoundToInt(serverPlayersInput.value)-1);
            _hostData = new RelayHostData {
                key = relayEntryPointAllocation.Key,
                port = (ushort) relayEntryPointAllocation.RelayServer.Port,
                allocationID = relayEntryPointAllocation.AllocationId,
                allocationIDBytes = relayEntryPointAllocation.AllocationIdBytes,
                connectionData = relayEntryPointAllocation.ConnectionData,
                ipv4Address = relayEntryPointAllocation.RelayServer.IpV4
            };

            //Join Code
            _hostData.joinCode = await Relay.Instance.GetJoinCodeAsync(relayEntryPointAllocation.AllocationId);

            string nameOfTheLobby = serverNameInput.text;
            int maxPlayersAllowedInLobby = Mathf.RoundToInt(serverPlayersInput.value);

            CreateLobbyOptions newLobbyOptions = new CreateLobbyOptions();

            newLobbyOptions.IsPrivate = !publicServerInput;

            //Lobby Options. This is syncing up the lobby join code from Relay and Lobby. By Default Lobby uses its own join code.
            newLobbyOptions.Data = new Dictionary<string, DataObject>() {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _hostData.joinCode
                        )
                },
            };

            Lobby newLobby = await LobbyService.Instance.CreateLobbyAsync(nameOfTheLobby, maxPlayersAllowedInLobby, newLobbyOptions);

            currentlyConnectedLobby = newLobby;

            //Lobby Heartbeat
            InvokeRepeating("KeepLobbyAliveWhileHostJoined", 0f, 15f);

            Debug.Log("Lobby Name: "+ newLobby.Name + " Lobby Max Player Count: " + newLobby.MaxPlayers + " with lobby code of: " + newLobby.LobbyCode);

            //Setting Data for the Transports 
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                _hostData.ipv4Address,
                _hostData.port,
                _hostData.allocationIDBytes,
                _hostData.key,
                _hostData.connectionData
            );

        } catch (LobbyServiceException error) {

            Debug.Log(error);

        }
        DisplayLobbyScreen(true);
    }

    //This function starts hosting the server
    public void StartHostingRelayServer() {

        sceneManager.GetComponent<NetworkSceneManager>().OnNetworkSpawn();
        NetworkManager.Singleton.StartHost();
    }

    public void OnStartHost() {
        sceneManager.GetComponent<NetworkSceneManager>().OnNetworkSpawn();
    }

    public void StartGameClient() {
        if (currentlyConnectedLobby.HostId == playerID) {
            StartHostingRelayServer();
        } else {
            Debug.Log("remove the button, does nothing as client");
        }
    }

    public void ChangeMaxPlayerHandleValue() {
        sliderValueDisplay.text = Mathf.RoundToInt(serverPlayersInput.value).ToString();
    }

    public async void DisplayLobbyScreen(bool _creatingServer) {
        if (_creatingServer) {
            if (currentlyConnectedLobby != null) {
                serverMenu.gameObject.SetActive(false);
                lobbyMenu.gameObject.SetActive(true);
                InvokeRepeating("PollForLobbyUpdates", 0f, 1.2f);
                List<string> playerListString = new List<string> ();

                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                    playerListString.Add(player.Id+"\n");
                }
                lobbyPlayerList.text = string.Join("", playerListString);
                Debug.Log(lobbyPlayerList.text);
            }
        } else {
            if (currentlyConnectedLobby != null) {
                serverMenu.gameObject.SetActive(false);
                lobbyMenu.gameObject.SetActive(true);
                InvokeRepeating("PollForLobbyUpdates", 0f, 1.2f);
                List<string> playerListString = new List<string> ();

                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                    playerListString.Add(player.Id+"\n");
                }
                lobbyPlayerList.text = string.Join("", playerListString);
                Debug.Log(lobbyPlayerList.text);


                //Joining the relay allocation after joining the lobby
                JoinAllocation relayEntryPointAllocation = await Relay.Instance.JoinAllocationAsync(currentlyConnectedLobby.Data["joinCode"].Value);

                //Setting join data for the relay allocation
                _joinData = new RelayJoinData {
                    key = relayEntryPointAllocation.Key,
                    port = (ushort) relayEntryPointAllocation.RelayServer.Port,
                    allocationID = relayEntryPointAllocation.AllocationId,
                    allocationIDBytes = relayEntryPointAllocation.AllocationIdBytes,
                    connectionData = relayEntryPointAllocation.ConnectionData,
                    hostConnectionData = relayEntryPointAllocation.HostConnectionData,
                    ipv4Address = relayEntryPointAllocation.RelayServer.IpV4
                };

                //Setting Data for the Transports 

                Debug.Log("Setting Transports");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    _joinData.ipv4Address,
                    _joinData.port,
                    _joinData.allocationIDBytes,
                    _joinData.key,
                    _joinData.connectionData,
                    _joinData.hostConnectionData
                );


                //Starting Client
                NetworkManager.Singleton.StartClient();
                
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

                List<string> playerListString = new List<string> ();

                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                    playerListString.Add(player.Id+"\n");
                }
                lobbyPlayerList.text = string.Join("", playerListString);
                Debug.Log(lobbyPlayerList.text);

            } catch (LobbyServiceException error) {

                Debug.Log(error);

            }
        }
        else {

            try {

                Debug.Log("Public");
                currentlyConnectedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
                List<string> playerListString = new List<string> ();

                foreach (var player in currentlyConnectedLobby.Players) {
                    Debug.Log("Player ID: " + player.Id);
                    playerListString.Add(player.Id+"\n");
                }
                lobbyPlayerList.text = string.Join("", playerListString);
                Debug.Log(lobbyPlayerList.text);

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


//Relay Host Data

public struct RelayHostData {
    public string joinCode;
    public string ipv4Address;
    public ushort port;
    public Guid allocationID;
    public byte[] allocationIDBytes;
    public byte[] connectionData;
    public byte[] key;

}


// Relay Join Data


public struct RelayJoinData {
    public string joinCode;
    public string ipv4Address;
    public ushort port;
    public Guid allocationID;
    public byte[] allocationIDBytes;
    public byte[] connectionData;
    public byte[] hostConnectionData;
    public byte[] key;

}