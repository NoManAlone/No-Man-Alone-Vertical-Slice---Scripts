using UnityEngine;
using System.Collections;

public class DialogBoxTrigger : MonoBehaviour
{
	GameManager gameManager;
	DialogBox dialogBox;

	public string textToDisplay;
	public byte iconToDisplay = 0;

	bool dialogStarted;

	public bool consoleTutorialDialog;

	void Awake()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		dialogBox = GameObject.Find("DialogBoxText").GetComponent<DialogBox>();
	}

	void OnTriggerEnter2D(Collider2D playerCheck)
	{
		if(playerCheck.gameObject == gameManager.myPlayer && !dialogStarted)
		{
			if(consoleTutorialDialog)
				gameManager.myPlayer.GetComponent<PlayerControl>().consoleTutorial = true;

			dialogStarted = true;
			dialogBox.StartDialogBox(textToDisplay, iconToDisplay);
		}
	}

	void OnTriggerExit2D(Collider2D playerCheck)
	{
		if(consoleTutorialDialog)
			gameManager.myPlayer.GetComponent<PlayerControl>().consoleTutorial = false;

		if(playerCheck.gameObject == gameManager.myPlayer && dialogStarted)
		{
			dialogStarted = false;
			dialogBox.EndDialogBox();
		}
	}
}
