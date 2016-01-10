using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

    RoomInfo[] gamesList;
    public GameObject lobbyUI;

    public bool networkDebugging, singlePlayerTesting;
    public static bool networkDebug;

	public string SceneToLoad;

    public string selectedGameText;

    LobbyUIManager lobbyUIManager;

    IEnumerator waitForPlayer;

    Text waitingText;

	/*---------------------------------------------------- AWAKE & START ----------------------------------------------------*/

    void Awake()
    {
		networkDebug = networkDebugging;
        lobbyUIManager = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>();
    }

    void Start ()
    {
        if(!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v4.2");

        if (!GameObject.FindWithTag("AudioManager"))
            Instantiate(Resources.Load("AudioManager"));

        else
            GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().Lobby();

        waitingText = GameObject.Find("Panel_WaitingForPlayer").transform.GetChild(0).GetChild(0).GetComponent<Text>();
    }

	/*---------------------------------------------------- UPDATE ----------------------------------------------------*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject inputField = GameObject.Find("InputField_CreateGameName");

            if(inputField)
                CreateRoom(inputField);
        }
    }

	/*---------------------------------------------------- PHOTON EVENTS ----------------------------------------------------*/

    void OnConnectedToPhoton()
    {
        if(networkDebug)
            Debug.Log("Connected to Photon");   
    }

    void OnConnectedToMaster()
    {
        if (networkDebug)
            Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
    }

    void OnFailedToConnectToPhoton()
    {
        if (networkDebug)
            Debug.Log("Failed to connect to Photon");
    }

    void OnDisconnectedFromPhoton()
    {
        if (networkDebug)
            Debug.Log("Disconnected From Photon");
    }

    void OnJoinedLobby()
    {
        if (networkDebug)
            Debug.Log("Joined Lobby");
    }

    void OnLeftLobby()
    {
        if (networkDebug)
            Debug.Log("Left Lobby");
    }

    void OnJoinedRoom()
    {
        if (networkDebug)
            Debug.Log("Joined Room");

        waitForPlayer = WaitForOtherPlayer();
        StartCoroutine(waitForPlayer);
    }

    void OnLeftRoom()
    {
        if (networkDebug)
            Debug.Log("Left Room");
    }

	void OnReceivedRoomListUpdate()
	{
		lobbyUI.GetComponent<LobbyUIManager>().PopulateGamesList();
	}

	/*---------------------------------------------------- CREATE ROOM ----------------------------------------------------*/

    public void CreateRoom(GameObject inputField)
    {
        if (inputField.GetComponent<InputField>().text != null)
        {
            string gameName = inputField.GetComponent<InputField>().text;
            RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 2 };
            PhotonNetwork.CreateRoom(gameName, roomOptions, TypedLobby.Default);
            
            lobbyUIManager.WaitingForPlayer();
        }
    }

	/*---------------------------------------------------- JOIN ROOM ----------------------------------------------------*/

    public void JoinRoom()
    {
        if(!string.IsNullOrEmpty(selectedGameText))
            PhotonNetwork.JoinRoom(selectedGameText);
        
        lobbyUIManager.WaitingForPlayer();
    }

	/*---------------------------------------------------- WAIT FOR OTHER PLAYER ----------------------------------------------------*/

    IEnumerator WaitForOtherPlayer()
    {
        if (!singlePlayerTesting)
        {
            while (PhotonNetwork.room.playerCount != 2)
            {
                if (networkDebug)
                    Debug.Log("Game created. Waiting for second player");

                yield return new WaitForSeconds(1f);
            }

            waitingText.text = "Joining Game";
            Application.LoadLevel(SceneToLoad);
        }


        else
        {
            waitingText.text = "Joining Game";
            Application.LoadLevel(SceneToLoad);
        }
    }

	/*---------------------------------------------------- CANCEL GAME ----------------------------------------------------*/

    public void CancelGame()
    {
        PhotonNetwork.LeaveRoom();

        if(waitForPlayer != null)
            StopCoroutine(waitForPlayer);

        lobbyUIManager.CancelGame();
    }
}
