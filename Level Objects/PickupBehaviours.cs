using UnityEngine;
using System.Collections;

public class PickupBehaviours : MonoBehaviour
{
	bool rising = true;
	Vector3 bottomPosition, topPosition;

	void Awake()
	{
		bottomPosition = transform.position;
		topPosition = transform.position + (Vector3.up*0.5f);
		StartCoroutine(RiseAndFall(rising));
	}

	IEnumerator RiseAndFall(bool rising)
	{
		float startTime = Time.time;
		Vector3 placeholderPosition = transform.position;
		if(rising)
		{
			while(topPosition.y-transform.position.y > 0.1f)
			{
				placeholderPosition = Vector3.Lerp(transform.position, topPosition, (Time.time-startTime)*Time.deltaTime);
				transform.position = placeholderPosition;
				yield return null;
			}
			rising = false;
		}

		else
		{
			while(transform.position.y-bottomPosition.y > 0.1f)
			{
				placeholderPosition = Vector3.Lerp(transform.position, bottomPosition, (Time.time-startTime)*Time.deltaTime);
				transform.position = placeholderPosition;
				yield return null;
			}
			rising = true;
		}

		StartCoroutine(RiseAndFall(rising));
	}
}
