using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonServer : MonoBehaviourPunCallbacks
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    [SerializeField]
    private CoinSpawner coinSpawner;

    private void Start()
    {
        // iniciar textos a 0
        player1Text.text = "0";
        player2Text.text = "0";
        ConnectToServer();
    }

    private static void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        PhotonNetwork.JoinOrCreateRoom(
            "DefaultRoom",
            new RoomOptions { MaxPlayers = 2 },
            TypedLobby.Default
        );
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("Failed to join room: " + message);
        CreateNewRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room. Total Players: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (PhotonNetwork.CurrentRoom.PlayerCount > 2)
        {
            Debug.Log("Room is full, creating a new room...");
            PhotonNetwork.LeaveRoom();
            CreateNewRoom();
            return;
        }

        InstantiatePlayer();
        coinSpawner.Start();
    }

    private void CreateNewRoom()
    {
        string newRoomName = "Room_" + Random.Range(1000, 9999); // Nombre aleatorio
        PhotonNetwork.CreateRoom(
            newRoomName,
            new RoomOptions { MaxPlayers = 2 },
            TypedLobby.Default
        );
    }

    private void InstantiatePlayer()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.LogError("No hay una sala activa. No se puede instanciar el jugador.");
            return;
        }

        GameObject selectedPrefab = PhotonNetwork.CurrentRoom.PlayerCount == 1 ? player1Prefab : player2Prefab;
        Transform spawnTransform = PhotonNetwork.CurrentRoom.PlayerCount == 1 ? spawnPoint1 : spawnPoint2;

        if (!PlayerPrefs.HasKey("Player1Score"))
        {
            PlayerPrefs.SetInt("Player1Score", 0);
            PlayerPrefs.Save();
        }

        if (!PlayerPrefs.HasKey("Player2Score"))
        {
            PlayerPrefs.SetInt("Player2Score", 0);
            PlayerPrefs.Save();
        }

        if (selectedPrefab == null)
        {
            Debug.LogError("No se encontró el prefab para el jugador correspondiente.");
            return;
        }

        PhotonNetwork.Instantiate(
            selectedPrefab.name,
            spawnTransform.position,
            Quaternion.identity
        );
    }
}
