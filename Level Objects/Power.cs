using UnityEngine;
using System.Collections;

public class Power : MonoBehaviour 
{
	public bool powered;
	public int powerCost;

	[PunRPC]
	public void TogglePowered(string powerableType)
	{
        switch (powerableType)
		{
			case "Door":
                TogglePower();
                GetComponent<DoorBehaviours>().SetPowered(powered);
				break;

			case "Fan":
                TogglePower();
                GetComponent<FanBehaviours>().SetPowered(powered);
                break;
		}
	}

    void TogglePower()
    {
        if (powered)
            powered = false;

        else
            powered = true;
    }

    public void SetPowered(bool power)
    {
        powered = power;
    }
}
