using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
	public int health = 3;
	public int maxHealth = 3;
	public float runSpeed = 10;
    public float invincibleTime = 2;
	
	public Image heart1, heart2, heart3;
	Color transparent = new Color(1,1,1,0), opaque = new Color(1,1,1,1);
	
	GameManager gameManager;
	PlayerControl playerControl;
	
	public void AlterHealth(int value)
	{
		health += value;
		
		if(health == 0)
		{
			playerControl.PlayerIncapacitate();
		}
		
		if(gameObject == gameManager.myPlayer)
		{
			switch(health)
			{
			case 0:
				heart1.color = transparent;
				heart2.color = transparent;
				heart3.color = transparent;
				break;
			case 1:
				heart1.color = opaque;
				heart2.color = transparent;
				heart3.color = transparent;
				break;
			case 2:
				heart1.color = opaque;
				heart2.color = opaque;
				heart3.color = transparent;
				break;
			case 3:
				heart1.color = opaque;
				heart2.color = opaque;
				heart3.color = opaque;
				break;
			default:
				heart1.color = transparent;
				heart2.color = transparent;
				heart3.color = transparent;
				break;
			}
		}
	}

	public void Initialisation()
	{
		if (GameObject.Find("NormalViewUI"))
		{
			heart1 = GameObject.Find("Heart_Filled").GetComponent<Image>();
			heart2 = GameObject.Find("Heart_Filled (1)").GetComponent<Image>();
			heart3 = GameObject.Find("Heart_Filled (2)").GetComponent<Image>();
		}
		
		else
			Debug.LogWarning("Could not find Heart Meter! Is there a NormalViewUI in the scene? ");
		
		if (GameObject.Find("GameManager"))
			gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		playerControl = GetComponent<PlayerControl>();
	}
}
