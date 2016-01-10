using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomProperties : MonoBehaviour
{
    public int roomNum;

    public Bounds roomAreaBounds;
    public Vector2 topLeftCorner, bottomRightCorner;

    public GameObject[] roomSwitchables;
	
    GameManager gameManager;

    void Awake ()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        roomAreaBounds = GetComponent<Collider2D>().bounds;
        topLeftCorner = new Vector2(roomAreaBounds.min.x - 0.1f, roomAreaBounds.min.y - 0.1f);
        bottomRightCorner = new Vector2(roomAreaBounds.max.x + 0.1f, roomAreaBounds.max.y + 0.1f);

        GetRoomSwitchables();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        SetPlayerNewRoom(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        SetPlayerCurrentRoom(collider);
    }

	void GetRoomSwitchables()
	{
		Power[] doors = new Power[0];
		if(transform.FindChild("Doors"))
			doors = transform.FindChild("Doors").GetComponentsInChildren<Power>(true);

		Power[] otherSwitchables = GetComponentsInChildren<Power>(true);

		roomSwitchables = new GameObject[doors.Length + otherSwitchables.Length];

		for(byte counter = 0; counter < doors.Length; counter++)
			roomSwitchables[counter] = doors[counter].gameObject;
		for(byte counter = 0; counter < otherSwitchables.Length; counter++)
			roomSwitchables[doors.Length + counter] = otherSwitchables[counter].gameObject;
	}

    void SetPlayerNewRoom(Collider2D collider)
    {
        if (collider.gameObject.tag.Contains("Player"))
        {
            if (collider.gameObject == gameManager.myPlayer)
            {
                gameManager.myPlayerNewRoom = gameObject;

                if(!gameManager.myPlayerCurrentRoom)
                    gameManager.myPlayerCurrentRoom = gameManager.myPlayerNewRoom;
            }

            else if (collider.gameObject == gameManager.otherPlayer)
            {
                gameManager.otherPlayerNewRoom = gameObject;

                if(!gameManager.otherPlayerCurrentRoom)
                    gameManager.otherPlayerCurrentRoom = gameManager.otherPlayerNewRoom;
            }
        }
    }

    void SetPlayerCurrentRoom(Collider2D collider)
    {
        if (collider.gameObject.tag.Contains("Player"))
        {
            if (collider.gameObject == gameManager.myPlayer)
                gameManager.myPlayerCurrentRoom = gameManager.myPlayerNewRoom;
           
            else if (collider.gameObject == gameManager.otherPlayer)
                gameManager.otherPlayerCurrentRoom = gameManager.otherPlayerNewRoom;
        }
    }
}
