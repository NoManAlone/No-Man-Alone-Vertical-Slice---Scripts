using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ConsoleControl : MonoBehaviour 
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

    KeyCode powerOffAll;

    GameManager gameManager;
	PowerManager powerManager;
    ConsoleCameraControl consoleCameraControl;

    GameObject unpowerAllButton, lastSelectedRoom;
    Button button_ToggleLights, button_UnpowerRoom, button_UnpowerAll;

    public GameObject[] rooms;

    LayerMask switchableLayerMask, roomLayerMask;

	PlayerControl playerControl;
	DialogBox dialogBox;

	/*---------------------------------------------------- UPDATE ----------------------------------------------------*/

    void Update()
    {
        if(playerControl.usingConsole || GameManager.test)
        {
			//Power off all switchables hotkey
			if (Input.GetKeyDown(powerOffAll))
				UnpowerAllSwitchables();

			// Left-click event for switchable level objects.
			if (Input.GetMouseButtonDown(0))
			{
                Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
				RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0, switchableLayerMask);

                if (hit.collider != null)
                {
                    Debug.Log("Clicked " + hit.collider.name + ", Layer = " + LayerMask.LayerToName(hit.collider.gameObject.layer)
                    + ", Tag = " + hit.collider.tag);

					SwitchableToggle(hit);
                }

                else
                    print("No switchables hit");
			}

			// Right-click event for focusing rooms.
			else if(Input.GetMouseButtonDown(1))
			{
				Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
				RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0, roomLayerMask);

				if(hit.collider.tag == "Room")
					consoleCameraControl.FocusRoom(hit.collider.gameObject);
				else
					print("No room hit.");
			}
		}
	}

	/*---------------------------------------------------- SWITCHABLE TOGGLE ----------------------------------------------------*/

    void SwitchableToggle(RaycastHit2D hit)
    {
        if (hit.collider.tag == "SwitchableClickArea")
        {
            GameObject hitObject = hit.transform.parent.gameObject;

			// Stop telling the player how to use the console if they do it once.
			if(playerControl.consoleTutorial)
			{
				dialogBox.EndDialogBox();
				playerControl.consoleTutorial = false;
			}

            //If powered
            if (hitObject.GetComponent<Power>().powered)
            { 
                if(hitObject.tag == "Door")
                {
                    if(!hitObject.GetComponent<DoorBehaviours>().DoorObstructed())
                        hitObject.GetComponent<PhotonView>().RPC("TogglePowered", PhotonTargets.AllBuffered, hit.transform.parent.tag);
                }

                else
                {
                    hitObject.GetComponent<PhotonView>().RPC("TogglePowered", PhotonTargets.AllBuffered, hit.transform.parent.tag);
                }
            }

            else//If not powered
            {
                if (powerManager.power >= hit.transform.parent.GetComponent<Power>().powerCost)
                    hitObject.GetComponent<PhotonView>().RPC("TogglePowered", PhotonTargets.AllBuffered, hit.transform.parent.tag);
            }
        }   
    }

	/*---------------------------------------------------- UNPOWER ALL SWITCHABLES ----------------------------------------------------*/

    void UnpowerAllSwitchables()
    {
        foreach(GameObject room in rooms)
        {
            GameObject[] powerables = room.GetComponent<RoomProperties>().roomSwitchables;

            foreach (GameObject powerable in powerables)
            {
                if(powerable.GetComponent<Power>() && powerable.GetComponent<Power>().powered)
                    powerable.GetComponent<PhotonView>().RPC("TogglePowered", PhotonTargets.AllBuffered, powerable.tag);
            }
        }
    }

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

	public void Initialisation()
	{
		consoleCameraControl = transform.FindChild("ConsoleCamera").GetComponent<ConsoleCameraControl>();
		
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		powerManager = gameManager.gameObject.GetComponent<PowerManager>();
		
		rooms = GameObject.FindGameObjectsWithTag("Room");
		
		unpowerAllButton = GameObject.Find("Unpower All Button");
		
		button_UnpowerAll = unpowerAllButton.GetComponent<Button>();
		button_UnpowerAll.onClick.AddListener(UnpowerAllSwitchables);
		
		unpowerAllButton.GetComponent<Button>().onClick.AddListener(UnpowerAllSwitchables);
		
		switchableLayerMask = LayerMask.GetMask("ConsoleInteraction");
		roomLayerMask = LayerMask.GetMask("Room");
		
		powerOffAll = KeyCode.C;
		
		playerControl = gameManager.myPlayer.GetComponent<PlayerControl>();
		dialogBox = GameObject.Find("DialogBoxText").GetComponent<DialogBox>();
	}
}
