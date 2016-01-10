using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	public static bool test;
    public bool testingPlayer1;

	public bool networkDebugging, enemyDebugging, playerInvincible;
	public static bool networkDebug, enemyDebug, playerInvinc;

    public bool bothPlayersJoined;

	GameObject spawn;

	public GameObject myPlayer;
    public GameObject otherPlayer;

    public GameObject myPlayerCurrentRoom, myPlayerNewRoom, otherPlayerCurrentRoom, otherPlayerNewRoom;

	public static bool playerRedIncapacitated, playerBlueIncapacitated;

	/*---------------------------------------------------- AWAKE & START ----------------------------------------------------*/

    void Awake () 
	{
        // Only show spawn position sprite in editor.
        if (GameObject.Find("Spawn Position"))
        {
            spawn = GameObject.Find("Spawn Position");
            spawn.SetActive(false);
        }

        else
        {
            Debug.LogWarning("No Spawn Position found! Creating one at position (0,0)");
            spawn  = Instantiate(Resources.Load("Spawn Position", typeof(GameObject))) as GameObject;
            spawn.GetComponent<SpriteRenderer>().enabled = false;
        }

        // Global static variables, (secondary variables for setting in inspector).
        networkDebug = networkDebugging;
        enemyDebug = enemyDebugging;
        playerInvinc = playerInvincible;
    }

    void Start()
    {
        //If not joining from lobby. This is for testing purposes
        if (!PhotonNetwork.connected)
        {
            Debug.Log("Testing Mode; Connecting to Photon");

            test = true;

            //Connect to photon
            PhotonNetwork.ConnectUsingSettings("v4.2");
        }

        //Joining from lobby
        else
        {
            if (PhotonNetwork.isMasterClient)
                SpawnPlayer("Player Red", spawn.transform.position);

            else
                SpawnPlayer("Player Blue", spawn.transform.position + new Vector3(-2, 0, 0));

            StartCoroutine(WaitForOtherPlayer());
        }   
    }

	/*---------------------------------------------------- UPDATE ----------------------------------------------------*/

    void Update()
    {
        if (bothPlayersJoined)
        {
            Debug.Log("Player Count = " + PhotonNetwork.room.playerCount);

            if (PhotonNetwork.room.playerCount != 2)
            {
                Debug.Log("Player Disconnected");
                PhotonNetwork.LeaveRoom();
                Application.LoadLevel("Lobby");

            }  
        }
    }

	/*---------------------------------------------------- PHOTON EVENTS ----------------------------------------------------*/

    void OnConnectedToPhoton()
    {
        if (test)
            Debug.Log("Connected to Photon");
    }

    void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    void OnJoinedLobby()
    {
        if (test)
        {
            Debug.Log("Joined Lobby");

            RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 2 };
            PhotonNetwork.JoinOrCreateRoom("Game", roomOptions, TypedLobby.Default);
        }  
    }

    void OnJoinedRoom()
    {
        if (test)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if(testingPlayer1)
                    SpawnPlayer("Player Red", spawn.transform.position);

                else
                    SpawnPlayer("Player Blue", spawn.transform.position);
            } 

            else
                SpawnPlayer("Player Blue", spawn.transform.position + new Vector3(-5, 0, 0));
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("Player Disconnected " + player.name);
    }

	/*---------------------------------------------------- SPAWN PLAYER ----------------------------------------------------*/

    void SpawnPlayer(string playerName, Vector3 spawnPosition)
    {
        GameObject player = PhotonNetwork.Instantiate(playerName, spawnPosition, Quaternion.identity, 0);
        myPlayer = player;

        PhotonNetwork.player.name = playerName;

        //Enable player scripts
        player.AddComponent<PlayerControl>();
        player.transform.FindChild("CollisionCheck").gameObject.AddComponent<PlayerCollisions>();
        player.AddComponent<ConsoleControl>();
        player.AddComponent<PlayerStats>();
        player.GetComponent<AudioListener>().enabled = true;
        player.transform.FindChild("Cameras").gameObject.SetActive(true);
        player.transform.FindChild("PlayerVisibility").gameObject.SetActive(true);

        // Initialise player scripts manually, to avoid null references.
        player.GetComponent<PlayerControl>().Initialisation();
        player.GetComponent<ConsoleControl>().Initialisation();
        player.transform.FindChild("CollisionCheck").gameObject.GetComponent<PlayerCollisions>().Initialisation();
        player.GetComponent<PlayerStats>().Initialisation();
        player.transform.FindChild("ConsoleCamera").GetComponent<ConsoleCameraControl>().Initialisation();
    }

	/*---------------------------------------------------- WAIT FOR OTHER PLAYER ----------------------------------------------------*/

    IEnumerator WaitForOtherPlayer()
    {
        while (!bothPlayersJoined)
        {
            if(networkDebug)
                print("waiting for other player");

            if (PhotonNetwork.room.playerCount == 2)
            {
                while (otherPlayer == null)
                {
                    SetOtherPlayer();
                    yield return new WaitForSeconds(0.5f);
                }   

                bothPlayersJoined = true;
            }

            yield return null;
        }
    }

	/*---------------------------------------------------- SET OTHER PLAYER ----------------------------------------------------*/

    void SetOtherPlayer()
    {
        if (myPlayer.tag == "Player Red")
        {
            if (GameObject.FindGameObjectWithTag("Player Blue"))
                otherPlayer = GameObject.FindGameObjectWithTag("Player Blue");
        }

        else if (myPlayer.tag == "Player Blue")
        {
            if (GameObject.FindGameObjectWithTag("Player Red"))
                otherPlayer = GameObject.FindGameObjectWithTag("Player Red");
        }
    }

	/*---------------------------------------------------- SET PLAYERS INCAPACITATED ----------------------------------------------------*/

	[PunRPC]
	public void SetPlayersIncapacitated(bool isPlayerRed, bool isIncapacitated)
	{
		if(isPlayerRed)
			playerRedIncapacitated = isIncapacitated;

		else
			playerBlueIncapacitated = isIncapacitated;

		if(playerRedIncapacitated && playerBlueIncapacitated)
			Application.LoadLevel(Application.loadedLevel);
	}

	/*---------------------------------------------------- LEVEL COMPLETE ----------------------------------------------------*/

    public void LevelComplete(LevelEndTeleporter levelEndTeleporter)
    {
        myPlayer.transform.FindChild("CollisionCheck").gameObject.layer = LayerMask.NameToLayer("PlayerNoCollision");

        myPlayer.GetComponent<PlayerControl>().enabled = false;
        myPlayer.GetComponent<PhotonTransformView>().enabled = false;
        myPlayer.transform.FindChild("Player Animation").GetComponent<PhotonAnimatorView>().enabled = false;

        if (otherPlayer)
        {
            otherPlayer.transform.FindChild("CollisionCheck").gameObject.layer = LayerMask.NameToLayer("PlayerNoCollision");

            otherPlayer.GetComponent<PhotonTransformView>().enabled = false;
            otherPlayer.transform.FindChild("Player Animation").GetComponent<PhotonAnimatorView>().enabled = false;
        }

        StartCoroutine(levelEndTeleporter.LevelEndAnimation(myPlayer, otherPlayer));
    }
}
