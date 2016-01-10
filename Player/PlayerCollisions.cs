using UnityEngine;
using System.Collections;

public class PlayerCollisions : MonoBehaviour
{
    PlayerControl playerControl;
    PlayerStats playerStats;

    void OnTriggerEnter2D(Collider2D collider)
    {
        ////Enemy
        if (collider.gameObject.tag == "EnemyCollider")
        {
            if (playerStats.health > 0)
            {
                StartCoroutine(DisableCollision());
                StartCoroutine(playerControl.Knockback());
            }
        }

        //Player Revive
        if (collider.gameObject.tag == "Player Red" || collider.gameObject.tag == "Player Blue")
        {
            if (playerStats.health <= 0 && !playerControl.reviving)
                StartCoroutine(playerControl.RevivePlayer());
        }
    }

    public IEnumerator DisableCollision()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerNoCollision");
        playerControl.invincible = true;
        yield return new WaitForSeconds(playerStats.invincibleTime);
        gameObject.layer = LayerMask.NameToLayer("PlayerCollision");
        playerControl.invincible = false;
    }

	public void Initialisation()
	{
		playerControl = transform.parent.GetComponent<PlayerControl>();
		playerStats = transform.parent.GetComponent<PlayerStats>();
	}
}
