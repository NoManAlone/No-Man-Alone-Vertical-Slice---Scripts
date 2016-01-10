using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomPlayerTriggers : MonoBehaviour
{
	Image visibilityOverlay;
	PhotonView photonView;

	LevelDetails levelDetails;

	bool redPlayerPresent = false, bluePlayerPresent = false;

	void Awake()
	{
		visibilityOverlay = transform.parent.FindChild("Visibility Overlay").GetComponent<Image>();
		visibilityOverlay.enabled = true;

		photonView = GetComponent<PhotonView>();

		levelDetails = GameObject.Find("Level").GetComponent<LevelDetails>();
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if(collider.gameObject.tag.Contains("Player"))
		{
			if(collider.gameObject.tag.Contains("Red"))
				redPlayerPresent = true;
			else
				bluePlayerPresent = true;

			if(visibilityOverlay.enabled)
			{
				photonView.RPC("RevealRoom", PhotonTargets.AllBuffered);
			}
		}
	}
	
	void OnTriggerExit2D(Collider2D collider)
	{
		if(collider.gameObject.tag.Contains("Player"))
		{
			if(collider.gameObject.tag.Contains("Red"))
				redPlayerPresent = false;
			else
				bluePlayerPresent = false;

			if(!redPlayerPresent && !bluePlayerPresent)
			{
				UnpowerRoom(transform.parent.gameObject);
			}
		}
	}

	void UnpowerRoom(GameObject room)
	{
		GameObject[] powerables = room.GetComponent<RoomProperties>().roomSwitchables;
		
		foreach (GameObject powerable in powerables)
		{
			if(powerable.GetComponent<Power>() && powerable.GetComponent<Power>().powered)
				powerable.GetComponent<PhotonView>().RPC("TogglePowered", PhotonTargets.AllBuffered, powerable.tag);
		}
	}

	[PunRPC]
	void RevealRoom()
	{
		visibilityOverlay.enabled = false;
		levelDetails.AddFoundRoom(transform.parent.gameObject);
	}
}
