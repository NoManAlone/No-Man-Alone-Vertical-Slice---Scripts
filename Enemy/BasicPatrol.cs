using UnityEngine;
using System.Collections;

public class BasicPatrol : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

    bool movingRight, patrolling, grounded;
    float moveSpeed = 8;

    bool roomStartPositionSet;

    Rigidbody2D enemyRB;
    RaycastHit2D hit;

    LayerMask collisions;
    Collider2D[] wallOverlaps;
    Collider2D[] groundOverlaps;
    
    BoxCollider2D wallCheckCollider, groundCheckCollider, centerCollider;

    bool collidingWall;

	/*---------------------------------------------------- AWAKE ----------------------------------------------------*/

    void Awake()
    {
		Initialisation();   
    }

	/*---------------------------------------------------- UPDATE ----------------------------------------------------*/

    void Update()
    {
		// Check if on the ground.
		GroundCheck();

		// If on the ground and patrolling, check for obstructions.
        if (patrolling && grounded)
		{
			WallCheck();
			EdgeCheck();
		}

		// After all checks, keep patrolling.
        if (patrolling)
			Patrol();
    }

	/*---------------------------------------------------- GROUND CHECK ----------------------------------------------------*/

	void GroundCheck()
	{
		// Look for any colliders overlapping the ground check collider on the enemy object.
		groundOverlaps = Physics2D.OverlapAreaAll(new Vector2(groundCheckCollider.bounds.min.x, groundCheckCollider.bounds.min.y),
		                                          new Vector2(groundCheckCollider.bounds.max.x, groundCheckCollider.bounds.max.y), collisions);

		// If any overlapping colliders are found, the enemy is grounded.
		if(groundOverlaps.Length > 0)
			grounded = true;
		
		else
			grounded = false;
	}

	/*---------------------------------------------------- WALL CHECK ----------------------------------------------------*/

	void WallCheck()
	{
		// Look for any colliders overlapping the wall check collider on the enemy object.
		wallOverlaps = Physics2D.OverlapAreaAll(new Vector2(wallCheckCollider.bounds.min.x, wallCheckCollider.bounds.min.y),
		                                        new Vector2(wallCheckCollider.bounds.max.x, wallCheckCollider.bounds.max.y), collisions);

		// If any overlapping colliders are found, change the enemy's patrol direction.
		if (wallOverlaps.Length > 0)
			ChangeDirection();
	}

	/*---------------------------------------------------- EDGE CHECK ----------------------------------------------------*/

	void EdgeCheck()
	{
		// Raycast diagonally down in the direction the enemy is moving, to look for platform edges.
		if(movingRight)
		{
			hit = Physics2D.Raycast(centerCollider.bounds.center, new Vector2(1, -.5f), 5f, collisions);
			
			if (GameManager.enemyDebug)
				Debug.DrawRay(centerCollider.bounds.center, new Vector2(1, -.5f) * 5f, Color.blue);
		}
		else
		{
			hit = Physics2D.Raycast(centerCollider.bounds.center, new Vector2(-1, -.5f), 5f, collisions);
			
			if (GameManager.enemyDebug)
				Debug.DrawRay(centerCollider.bounds.center, new Vector2(-1, -.5f) * 5f, Color.blue);
		}

		// If no platform is found, the enemy has reached an edge, and changes direction.
		if(!hit)
		{
			if(GameManager.enemyDebug)
				print("Detected Edge - Change Direction");
			
			ChangeDirection();
		}
	}

	/*---------------------------------------------------- PATROL ----------------------------------------------------*/

	void Patrol()
	{
		// Set movement speed based on direction.
		if(grounded)
		{
			if (movingRight)
				enemyRB.velocity = new Vector2(moveSpeed, enemyRB.velocity.y);
			
			else
				enemyRB.velocity = new Vector2(-moveSpeed, enemyRB.velocity.y);
		}

		// Stop moving if airborne (such as when been blown upwards by a fan).
		else
			enemyRB.velocity = new Vector2(0, enemyRB.velocity.y);
	}

	/*---------------------------------------------------- CHANGE DIRECTION ----------------------------------------------------*/

    void ChangeDirection()
    {
		// Changes the bool that decides movement direction.
		movingRight = !movingRight;

		// Change scale of object to flip the sprite.
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1);
    }

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

	void Initialisation()
	{
		// Only calculate movement on the master client to prevent sync loss.
		if(PhotonNetwork.isMasterClient)
			this.enabled = true;

		enemyRB = GetComponent<Rigidbody2D>();
		
		patrolling = true;
		
		collisions = LayerMask.GetMask("Wall", "Door", "Platform", "InteriorDoor");
		
		centerCollider = transform.FindChild("Physical Colliders").GetComponent<BoxCollider2D>();
		
		wallCheckCollider = transform.FindChild("WallCheck").GetComponent<BoxCollider2D>();
		groundCheckCollider = transform.FindChild("GroundCheck").GetComponent<BoxCollider2D>();
	}
}
