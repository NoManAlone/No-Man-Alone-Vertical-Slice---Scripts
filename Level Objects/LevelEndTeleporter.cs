using UnityEngine;
using System.Collections;

public class LevelEndTeleporter : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	public bool player1colliding, player2colliding;
	GameManager gameManager;
	PhotonView photonView;

	public string nextScene;

	Animator teleporterAnimator;

	/*---------------------------------------------------- AWAKE & UPDATE ----------------------------------------------------*/

	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		photonView = GetComponent<PhotonView>();

		teleporterAnimator = GetComponent<Animator>();
	}

	void Update()
	{
		if(player1colliding && player2colliding && PhotonNetwork.isMasterClient)
		{
			// Called by the master client to ensure the function is called on both players.
			// Otherwise frame differences between the players can cause one player to call LevelComplete without the other.
			photonView.RPC("CallLevelComplete", PhotonTargets.AllBuffered);
		}
	}

	/*---------------------------------------------------- ON TRIGGER ENTER AND EXIT ----------------------------------------------------*/

	void OnTriggerEnter2D(Collider2D collisionCheck)
	{
		if(collisionCheck.gameObject.tag == "Player Red")
			player1colliding = true;
		else if(collisionCheck.gameObject.tag == "Player Blue")
			player2colliding = true;
	}
	
	void OnTriggerExit2D(Collider2D collisionCheck)
	{
		if(collisionCheck.gameObject.tag == "Player Red")
			player1colliding = false;
		else if(collisionCheck.gameObject.tag == "Player Blue")
			player2colliding = false;
	}

	/*---------------------------------------------------- CALL LEVEL COMPLETE ----------------------------------------------------*/

	[PunRPC]
	void CallLevelComplete()
	{
        player1colliding = false;
        player2colliding = false;
        gameManager.LevelComplete(this);
    }

	/*---------------------------------------------------- LEVEL END ANIMATION ----------------------------------------------------*/

	public IEnumerator LevelEndAnimation(GameObject myPlayer, GameObject otherPlayer)
	{
		Animator myPlayerAnimator, otherPlayerAnimator;

		myPlayerAnimator = myPlayer.transform.FindChild("Player Animation").GetComponent<Animator>();
		otherPlayerAnimator = otherPlayer.transform.FindChild("Player Animation").GetComponent<Animator>();
        
		myPlayerAnimator.SetBool("Grounded", true);
		myPlayerAnimator.SetBool("UsingConsole", false);
		myPlayerAnimator.SetBool("Incapacitated", false);
		otherPlayerAnimator.SetBool("Grounded", true);
		otherPlayerAnimator.SetBool("UsingConsole", false);
		otherPlayerAnimator.SetBool("Incapacitated", false);

		myPlayerAnimator.SetBool("Moving", false);
		otherPlayerAnimator.SetBool("Moving", false);

		teleporterAnimator.SetBool("Teleporting", true);
		yield return new WaitForSeconds(1);
        teleporterAnimator.SetBool("Teleporting", false);
		
		Application.LoadLevel(nextScene);
	}
}
