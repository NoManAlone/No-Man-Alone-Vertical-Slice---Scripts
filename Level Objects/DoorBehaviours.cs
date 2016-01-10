using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DoorBehaviours : MonoBehaviour 
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	AudioSource sfx;
    AudioClip sfxDoorOpening, sfxDoorClosing;
	
	PowerManager powerManager;

	bool powered;
    int powerCost;
    public float doorOpenSpeed = 8;

    public Sprite consoleView_Powered, consoleView_Unpowered;
    SpriteRenderer consoleViewImage;

    Bounds doorBounds;

    Transform door, doorImage;

    Transform doorTop, doorBottom;
    Transform doorImageTop, doorImageBottom;

    bool doorOpening, doorClosing;

    //Vertical Doors
    Vector3 doorOpenPosition, doorClosePosition;

    //Horizontal Doors
    Vector3 doorTopOpenPosition, doorBottomOpenPosition, doorTopClosePosition, doorBottomClosePosition;

	/*---------------------------------------------------- AWAKE & START ----------------------------------------------------*/

    void Awake()
	{
		Initialisation();
    }
	
	void Start()
	{
        if (powered && powerManager.power >= powerCost)
        {
            OpenDoor();
            consoleViewImage.sprite = consoleView_Powered;
        }

        else
        {
            consoleViewImage.sprite = consoleView_Unpowered;
        }
	}

	/*---------------------------------------------------- SET POWERED ----------------------------------------------------*/

    public void SetPowered(bool powered)
    {
        if (powered)
            StartCoroutine(OpenDoor());

        else
            StartCoroutine(CloseDoor());
    }

	/*---------------------------------------------------- OPEN DOOR ----------------------------------------------------*/

    IEnumerator OpenDoor()
    {
        doorClosing = false;
        doorOpening = true;

        sfx.clip = sfxDoorOpening;
        sfx.Play();

        powered = true;
        powerManager.AlterMaxPower(-powerCost);
        consoleViewImage.sprite = consoleView_Powered;

        if (name.Contains("V"))
        {
			if (gameObject.name.Contains("Interior"))
				door.gameObject.SetActive(false);

            while (doorOpening)
            {
                door.position = Vector3.MoveTowards(door.position, doorOpenPosition, Time.deltaTime * doorOpenSpeed);
                doorImage.position = Vector3.MoveTowards(doorImage.position, new Vector2(doorOpenPosition.x, doorOpenPosition.y + 4), Time.deltaTime * doorOpenSpeed);

                if (door.position == doorOpenPosition)
                    doorOpening = false;

                yield return null;
            }
        }

        else
        {
            while (doorOpening)
            {
                doorTop.position = Vector3.MoveTowards(doorTop.position, doorTopOpenPosition, Time.deltaTime * doorOpenSpeed);
                doorBottom.position = Vector3.MoveTowards(doorBottom.position, doorBottomOpenPosition, Time.deltaTime * doorOpenSpeed);

                if (transform.rotation.z == 0)
                {
                    doorImageTop.position = Vector3.MoveTowards(doorImageTop.position, doorTopOpenPosition + new Vector3(0, 2, 0), Time.deltaTime * doorOpenSpeed);
                    doorImageBottom.position = Vector3.MoveTowards(doorImageBottom.position, doorBottomOpenPosition - new Vector3(0, 2, 0), Time.deltaTime * doorOpenSpeed);
                }
                else
                {
                    doorImageTop.position = Vector3.MoveTowards(doorImageTop.position, doorTopOpenPosition - new Vector3(2, 0, 0), Time.deltaTime * doorOpenSpeed);
                    doorImageBottom.position = Vector3.MoveTowards(doorImageBottom.position, doorBottomOpenPosition + new Vector3(2, 0, 0), Time.deltaTime * doorOpenSpeed);
                }

                if (doorTop.position == doorTopOpenPosition && doorBottom.position == doorBottomOpenPosition)
                    doorOpening = false;

                yield return null;
            }
        }
    }

	/*---------------------------------------------------- CLOSE DOOR ----------------------------------------------------*/

    IEnumerator CloseDoor()
    {
        doorOpening = false;
        doorClosing = true;

        sfx.clip = sfxDoorClosing;
        sfx.Play();

        powered = false;
        powerManager.AlterMaxPower(powerCost);
        consoleViewImage.sprite = consoleView_Unpowered;

        if(name.Contains("V"))
        {
            while (doorClosing)
            {
                door.position = Vector3.MoveTowards(door.position, doorClosePosition, Time.deltaTime * doorOpenSpeed);
                doorImage.position = Vector3.MoveTowards(doorImage.position, new Vector2(doorClosePosition.x, doorClosePosition.y ), Time.deltaTime * doorOpenSpeed);

                if (door.position == doorClosePosition && doorImage.position == doorClosePosition)
                    doorClosing = false;

                yield return null;
            }

			if (gameObject.name.Contains("Interior"))
				door.gameObject.SetActive(true);
        }

        else
        {
            while (doorClosing)
            {
                doorTop.position = Vector3.MoveTowards(doorTop.position, doorTopClosePosition, Time.deltaTime * doorOpenSpeed);
                doorBottom.transform.position = Vector3.MoveTowards(doorBottom.position, doorBottomClosePosition, Time.deltaTime * doorOpenSpeed);

                if (transform.rotation.z == 0)
                {
                    doorImageTop.position = Vector3.MoveTowards(doorImageTop.position, doorTopClosePosition + new Vector3(0, 2 * gameObject.transform.localScale.y, 0), Time.deltaTime * doorOpenSpeed);
                    doorImageBottom.position = Vector3.MoveTowards(doorImageBottom.position, doorBottomClosePosition - new Vector3(0, 2 * gameObject.transform.localScale.y, 0), Time.deltaTime * doorOpenSpeed);
                }
                else
                {
                    doorImageTop.position = Vector3.MoveTowards(doorImageTop.position, doorTopClosePosition - new Vector3(2 * gameObject.transform.localScale.y, 0, 0), Time.deltaTime * doorOpenSpeed);
                    doorImageBottom.position = Vector3.MoveTowards(doorImageBottom.position, doorBottomClosePosition + new Vector3(2 * gameObject.transform.localScale.y, 0, 0), Time.deltaTime * doorOpenSpeed);
                }

                if (doorTop.position == doorTopClosePosition && doorBottom.position == doorBottomClosePosition)
                    doorClosing = false;

                yield return null;
            }
        }
    }

	/*---------------------------------------------------- DOOR OBSTRUCTED ----------------------------------------------------*/

    public bool DoorObstructed()
    {
        bool obstructed = false;

        Vector2 doorBottomLeftCorner = new Vector2(doorBounds.min.x, doorBounds.min.y);
        Vector2 doorTopRightCorner = new Vector2(doorBounds.max.x, doorBounds.max.y);

        int playerLayer = LayerMask.NameToLayer("Player");
        int playerMask = 1 << playerLayer;

        if (Physics2D.OverlapArea(doorBottomLeftCorner, doorTopRightCorner, playerMask))
            obstructed = true;

        return obstructed;
    }

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

	void Initialisation()
	{
		if (name.Contains("V"))
		{
			door = transform.FindChild("Door");
			doorImage = transform.FindChild("Door Mask").FindChild("DoorImage");
		}
		
		else
		{
			doorTop = transform.FindChild("Door_Top");
			doorBottom = transform.FindChild("Door_Bottom");
			doorImageTop = transform.FindChild("Door Mask").FindChild("Door Top");
			doorImageBottom = transform.FindChild("Door Mask").FindChild("Door Bottom");
		}
		
		consoleViewImage = transform.Find("ConsoleView").GetComponent<SpriteRenderer>();
		
		if (GameObject.Find("GameManager") != null)
			powerManager = GameObject.Find("GameManager").GetComponent<PowerManager>();
		
		else
			Debug.LogWarning("No GameManager found! Is there a GameManager object in the scene?");
		
		powered = GetComponent<Power>().powered;
		powerCost = GetComponent<Power>().powerCost;
		
		//Audio
		sfx = GetComponent<AudioSource>();
		sfxDoorOpening = Resources.Load("DoorOpening") as AudioClip;
		sfxDoorClosing = Resources.Load("DoorClosing") as AudioClip;
		
		
		//Set Door open and close positions
		if (name.Contains("V"))
		{
			doorOpenPosition = new Vector3(door.position.x, door.position.y + 8, 0);
			doorClosePosition = door.position;
		}
		
		else
		{
			if (transform.rotation.z == 0)
			{
				doorTopOpenPosition = new Vector3(doorTop.position.x, doorTop.position.y + 4 * gameObject.transform.localScale.y * doorTop.transform.localScale.y, 0);
				doorBottomOpenPosition = new Vector3(doorBottom.position.x, doorBottom.transform.position.y - 4 * gameObject.transform.localScale.y * doorTop.transform.localScale.y, 0);
				doorTopClosePosition = doorTop.transform.position;
				doorBottomClosePosition = doorBottom.position;
			}
			
			else
			{
				doorTopOpenPosition = new Vector3(doorTop.transform.position.x - 4 * gameObject.transform.localScale.y * doorTop.localScale.y, doorTop.position.y, 0);
				doorBottomOpenPosition = new Vector3(doorBottom.position.x + 4 * gameObject.transform.localScale.y * doorTop.localScale.y, doorBottom.position.y, 0);
				doorTopClosePosition = doorTop.position;
				doorBottomClosePosition = doorBottom.position;
			}
		}
	}
}
