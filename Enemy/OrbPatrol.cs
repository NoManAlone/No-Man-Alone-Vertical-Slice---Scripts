using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbPatrol : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	public List<Vector2> patrolPointsList;
	public bool loopedPatrol, movingClockwise;
	float speed = 8;

	Vector3[] patrolPoints;
	sbyte nextPatrolPoint;
	bool bouncingUp = false, bouncingDown = false, bouncingLeft = false, bouncingRight = false;

	Vector3 previousPosition;
	LayerMask doorLayerMask;

	public bool knockedOffCourse = false;
	bool patrolling = true;

	/*---------------------------------------------------- AWAKE & START ----------------------------------------------------*/

	void Awake()
	{
		Initialisation();
	}

	void Start()
	{
		StartCoroutine(Patrol());
	}

	/*---------------------------------------------------- PATROL ----------------------------------------------------*/

	IEnumerator Patrol()
	{
		// First, increment to the next patrol point on the route.
		SetNextPatrolPoint();

		// Move towards the next patrol point until arrival.
		while(transform.localPosition != patrolPoints[nextPatrolPoint])
		{
			// If on the route, keep going towards next point.
			if(!knockedOffCourse)
			{
				previousPosition = transform.localPosition;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, patrolPoints[nextPatrolPoint], Time.deltaTime*speed);
				yield return null;
			}

			// If not on the patrol route (because of external forces), return to route before continuing.
			else
			{
				knockedOffCourse = false;
				yield return StartCoroutine(Return(previousPosition));

				// Tells the rest of the script that the enemy has arrived back onto their patrol route.
				patrolling = true;
			}
		}

		// After arrival, renew patrol routine for next patrol point.
		StartCoroutine(Patrol());
	}

	/*---------------------------------------------------- RETURN ----------------------------------------------------*/

	IEnumerator Return(Vector3 returnPoint)
	{
		// Tells rest of the script that the enemy is no longer on their patrol route.
		patrolling = false;

		// Move towards the last location on the route the enemy was at, until arrival.
		while(transform.localPosition != returnPoint)
		{
			// If the enemy is not colliding with a door, keep moving.
			if(!bouncingUp && !bouncingDown && !bouncingLeft && !bouncingRight)
			{
				// If on the return route, keep moving.
				if(!knockedOffCourse)
				{
					previousPosition = transform.localPosition;
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, returnPoint, Time.deltaTime*speed);
					yield return null;
				}

				// If knocked off the return route, initiate another Return routine back to this return.
				else
				{
					knockedOffCourse = false;
					yield return StartCoroutine(Return(previousPosition));
				}
			}

			// If a door was collided with during return routine, bounce off it.
			else
				yield return StartCoroutine(Bounce());
		}
	}

	/*---------------------------------------------------- DOOR COLLISION ----------------------------------------------------*/

	// Used when colliding with a door during Return routine.
	public void DoorCollision(Collider2D doorCollider)
	{
		// If a door is hit during patrol, the enemy changes direction.
		if(patrolling)
		{
			movingClockwise = !movingClockwise;
			SetNextPatrolPoint();
		}

		// If the enemy is trying to return to their patrol route when they hit a door,
		// they bounce back from it before trying to move on again.
		else
		{
			// Get the direction of the door detected by the collider.
			RaycastHit2D upHit, downHit, leftHit, rightHit;
			upHit = Physics2D.Raycast(transform.position, Vector2.up, 3f, doorLayerMask);
			downHit = Physics2D.Raycast(transform.position, Vector2.down, 3f, doorLayerMask);
			leftHit = Physics2D.Raycast(transform.position, Vector2.left, 3f, doorLayerMask);
			rightHit = Physics2D.Raycast(transform.position, Vector2.right, 3f, doorLayerMask);
			
			// Tell the Return routine which way to bounce based on direction of the door.
			if(upHit.collider)
				bouncingDown = true;
			else if(downHit.collider)
				bouncingUp = true;
			else if(leftHit.collider)
				bouncingRight = true;
			else if(rightHit.collider)
				bouncingLeft = true;
		}
	}

	/*---------------------------------------------------- BOUNCE ----------------------------------------------------*/

	IEnumerator Bounce()
	{
		Vector3 startPosition = transform.localPosition;
		
		// Equation for speed has the enemy move faster at first,
		// reducing zero gradually to give the motion a bouncing effect.
		for(float timer = 0.75f; timer > 0; timer -= Time.deltaTime)
		{
			if(!knockedOffCourse)
			{
				previousPosition = transform.localPosition;

				if(bouncingUp)
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition + Vector3.up*speed, Time.deltaTime*speed*timer*1.3f);
				else if(bouncingDown)
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition + Vector3.down*speed, Time.deltaTime*speed*timer*1.3f);
				else if(bouncingLeft)
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition + Vector3.left*speed, Time.deltaTime*speed*timer*1.3f);
				else if(bouncingRight)
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition + Vector3.right*speed, Time.deltaTime*speed*timer*1.3f);

				yield return null;
			}
			else
			{
				knockedOffCourse = false;

				// If knocked off course mid-bounce, stops the bounce and begins another Return.
				timer = 0;
				bouncingUp = false;
				bouncingDown = false;
				bouncingLeft = false;
				bouncingRight = false;

				yield return Return(previousPosition);
			}
		}
		
		// Tell the routine to stop bouncing once finished.
		bouncingUp = false;
		bouncingDown = false;
		bouncingLeft = false;
		bouncingRight = false;
	}
	
	/*---------------------------------------------------- SET NEXT PATROL POINT ----------------------------------------------------*/

	void SetNextPatrolPoint()
	{
		if(movingClockwise)
		{
			nextPatrolPoint++;
			if(nextPatrolPoint > patrolPoints.Length-1)
			{
				if(loopedPatrol)
					nextPatrolPoint = 0;
				else
				{
					movingClockwise = false;
					nextPatrolPoint-=2;
				}
			}
		}
		else
		{
			nextPatrolPoint--;
			if(nextPatrolPoint < 0)
			{
				if(loopedPatrol)
					nextPatrolPoint = System.Convert.ToSByte(patrolPoints.Length-1);
				else
				{
					movingClockwise = true;
					nextPatrolPoint+=2;
				}
			}
		}
	}

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

	void Initialisation()
	{
		// Only calculate movement on the master client to prevent sync loss.
		if(PhotonNetwork.isMasterClient)
			this.enabled = true;
		else
			transform.FindChild("CollisionCheck").gameObject.SetActive(false);

		doorLayerMask = LayerMask.GetMask("Door", "InteriorDoor");

		// Convert the list of patrol points (used in the editor for setting up the route) into an array for faster processing.
		patrolPoints = new Vector3[patrolPointsList.Count];
		for(byte counter = 0; counter < patrolPoints.Length; counter++)
			patrolPoints[counter] = new Vector3(patrolPointsList[counter].x, patrolPointsList[counter].y, 0);

		// Move the enemy to the start position of their patrol route.
		transform.localPosition = patrolPoints[0];

		// Set previousPosition so that the first calling of CheckOnRoute() doesn't throw a null exception.
		previousPosition = transform.localPosition;
	}
}