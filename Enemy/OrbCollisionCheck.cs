using UnityEngine;
using System.Collections;

public class OrbCollisionCheck : MonoBehaviour
{
	// This is a supplemental script to OrbPatrol.cs, see that for further details.

	OrbPatrol orbPatrol;

	AreaEffector2D[] fansFound;
	GameObject[] doorsFound;

	/*---------------------------------------------------- AWAKE ----------------------------------------------------*/

	void Awake()
	{
		orbPatrol = transform.parent.GetComponent<OrbPatrol>();

		doorsFound = new GameObject[0];
		fansFound = new AreaEffector2D[0];
	}

	/*---------------------------------------------------- ON TRIGGER ENTER & EXIT 2D ----------------------------------------------------*/

	void OnTriggerEnter2D(Collider2D collider)
	{
		// Checks for doors, so that the patrol script can be notified to bounce off them.
		DoorCheck(collider, true);
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		// Manages the doorsFound array to keep it neat and tidy.
		DoorCheck(collider, false);
	}

	/*---------------------------------------------------- ON TRIGGER STAY 2D ----------------------------------------------------*/

	void OnTriggerStay2D(Collider2D collider)
	{
		// Checks for fans that blow the enemy off-course,
		// so that the patrol script can be notified to start a Return routine.
		FanCheck(collider);
	}

	/*---------------------------------------------------- DOOR CHECK ----------------------------------------------------*/

	void DoorCheck(Collider2D collider, bool entering)
	{
		if(LayerMask.LayerToName(collider.gameObject.layer).Contains("Door"))
		{
			if(entering)
			{
				GameObject doorParent = collider.transform.parent.gameObject;

				bool alreadyFound = false;

				foreach(GameObject doorFound in doorsFound)
				{
					if(doorParent == doorFound)
						alreadyFound = true;
				}

				if(!alreadyFound)
				{
					System.Array.Resize(ref doorsFound, doorsFound.Length+1);
					doorsFound[doorsFound.Length-1] = doorParent;
					orbPatrol.DoorCollision(collider);
				}
			}

			else
			{
				GameObject doorParent = collider.transform.parent.gameObject;
				
				for(byte counter = 0; counter < doorsFound.Length; counter++)
				{
					if(doorParent == doorsFound[counter])
					{
						// Removes the fan from the array, while tidying up the array size.
						for(int counter2 = counter+1; counter2 < doorsFound.Length; counter2++)
							doorsFound[counter2-1] = doorsFound[counter2];
						System.Array.Resize(ref doorsFound, doorsFound.Length-1);
					}
				}
			}
		}
	}

	/*---------------------------------------------------- FAN CHECK ----------------------------------------------------*/

	void FanCheck(Collider2D collider)
	{
		// If the collider found is a fan effector, enact the event.
		if(collider.GetComponent<AreaEffector2D>() && collider.GetComponent<AreaEffector2D>().enabled)
		{
			AreaEffector2D fan = collider.GetComponent<AreaEffector2D>();

			// Checks if this fan has already been accounted for,
			// so as not to tell the patrol script to throw more than one Return routine.
			bool alreadyFound = false;
			foreach(AreaEffector2D fanFound in fansFound)
			{
				if(fan == fanFound)
					alreadyFound = true;
			}

			// If the fan collision is really new, tell the patrol script to call Return, and record this fan as used.
			if(!alreadyFound)
			{
				System.Array.Resize(ref fansFound, fansFound.Length+1);
				fansFound[fansFound.Length-1] = fan;
				orbPatrol.knockedOffCourse = true;
			}
		}

		// If the fan is switched off, it is removed from the array of found fans,
		// so that it may trigger the event again later.
		else if(collider.GetComponent<AreaEffector2D>() && !collider.GetComponent<AreaEffector2D>().enabled)
		{
			AreaEffector2D fan = collider.GetComponent<AreaEffector2D>();
			
			for(byte counter = 0; counter < fansFound.Length; counter++)
			{
				if(fan == fansFound[counter])
				{
					// Removes the fan from the array, while tidying up the array size.
					for(int counter2 = counter+1; counter2 < fansFound.Length; counter2++)
						fansFound[counter2-1] = fansFound[counter2];
					System.Array.Resize(ref fansFound, fansFound.Length-1);
				}
			}
		}
	}
}
