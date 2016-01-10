using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerManager : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	public int startingMaxPower;
	public float power;
	float rechargeRate = 0;
    public float rechargeSpeed = 3;
	public bool recharging;
    int maxPower;
    public int meterLength;
	float depletionRate;

	sbyte nextPipDown, nextPipUp;

	bool redPlayerJetpacking = false, bluePlayerJetpacking = false;

	Text powerText, maxPowerText;

	GameObject powerMeterSegment;
	Transform powerMeter;
	
	Image[] powerMeterPips, segmentOverlays;

	/*---------------------------------------------------- AWAKE & UPDATE ----------------------------------------------------*/

	void Awake()
	{
		Initialisation();
    }

	void Update()
	{
		UpdatePower();
		UpdatePips();
	}

	/*---------------------------------------------------- UPDATE POWER ----------------------------------------------------*/

	void UpdatePower()
	{
		// Check if power out of bounds.
		if(power>maxPower)
		{
			power = maxPower;
			powerText.text = power.ToString();
		}
		else if(power < 0)
		{
			power = 0;
			powerText.text = power.ToString();
		}

		// Deplete or recharge power.
		if(depletionRate>0)
		{
			power -= depletionRate*Time.deltaTime;
			powerText.text = power.ToString();
		}
		else if(power<maxPower)
		{
			power += rechargeRate*Time.deltaTime;
			powerText.text = power.ToString();
		}
	}

	/*---------------------------------------------------- UPDATE PIPS ----------------------------------------------------*/

	void UpdatePips()
	{
		if(power-1<nextPipDown && power>0)
		{
			Color placeHolderColor = powerMeterPips[nextPipDown].color;
			placeHolderColor.a = 0f;
			if(powerMeterPips[nextPipDown])
				powerMeterPips[nextPipDown].color = placeHolderColor;
			nextPipDown = System.Convert.ToSByte(Mathf.Floor(power-1));
			nextPipUp = System.Convert.ToSByte(Mathf.Floor(power));
		}
		
		if(power-1>=nextPipUp)
		{
			Color placeHolderColor = powerMeterPips[nextPipUp].color;
			placeHolderColor.a = 1f;
			if(powerMeterPips[nextPipUp])
				powerMeterPips[nextPipUp].color = placeHolderColor;
			nextPipDown = System.Convert.ToSByte(Mathf.Floor(power-1));
			nextPipUp = System.Convert.ToSByte(Mathf.Floor(power));
		}
	}

	/*---------------------------------------------------- ALTER DEPLETION RATE ----------------------------------------------------*/

	[PunRPC]
	public void AlterDepletionRate(float alterAmount)
	{
		depletionRate += alterAmount;
	}

    public void AlterMaxPower(int powerValue)
	{
        if (power < 0)
            power = 0;

		maxPower += powerValue;
		maxPowerText.text = maxPower.ToString();
	    power += powerValue;
		powerText.text = power.ToString();
        
		nextPipDown += System.Convert.ToSByte(powerValue);
		nextPipUp += System.Convert.ToSByte(powerValue);

		SetPipsAlpha();
	}

	/*---------------------------------------------------- INSTANTIATE POWER METER ----------------------------------------------------*/

	public void InstantiatePowerMeter()
	{
        for (int counter = 1; counter<meterLength-1; counter++)
		{
			GameObject thisSegment = Instantiate(powerMeterSegment);
			thisSegment.transform.SetParent(powerMeter);
			thisSegment.transform.SetSiblingIndex(counter+1);
			thisSegment.GetComponent<RectTransform>().anchoredPosition = (8+(counter*23))*Vector2.right;
			thisSegment.transform.localScale = Vector3.one;
		}
		powerMeter.GetChild(0).GetComponent<RectTransform>().anchoredPosition = (15+((meterLength-1)*23))*Vector3.right;
		
		GetPipsAndOverlays();

		Color placeHolderColor = new Color(1, 0, 0, 1);

		for(int counter = 0; counter < meterLength/2; counter++)
		{
			placeHolderColor.a = powerMeterPips[counter].color.a;
			placeHolderColor.g = (float)counter/(meterLength/2f);

			powerMeterPips[counter].color = placeHolderColor;
		}

		placeHolderColor.g = 1;

		for(int counter = meterLength/2; counter < meterLength; counter++)
		{
			placeHolderColor.a = powerMeterPips[counter].color.a;
			placeHolderColor.r = 1f-(((float)counter-(meterLength/2))/(meterLength/2));
			
			powerMeterPips[counter].color = placeHolderColor;
		}
	}

	/*---------------------------------------------------- EMPTY OUT POWER METER ----------------------------------------------------*/

	public void EmptyOutPowerMeter()
	{
		if(powerMeter.childCount>2)
		{
			for(int counter = 2; counter<meterLength; counter++)
			{
				GameObject.Destroy(powerMeter.GetChild(counter).gameObject);
			}
		}
	}

	/*---------------------------------------------------- GET PIPS AND OVERLAYS ----------------------------------------------------*/

	void GetPipsAndOverlays()
	{
		System.Array.Resize(ref powerMeterPips, meterLength);
		System.Array.Resize(ref segmentOverlays, meterLength);
		for(int counter = 1; counter < meterLength; counter++)
		{
			powerMeterPips[counter-1] = powerMeter.GetChild(counter).GetChild(1).GetComponent<Image>();
			segmentOverlays[counter-1] = powerMeter.GetChild(counter).GetChild(0).GetComponent<Image>();
		}
		powerMeterPips[meterLength-1] = powerMeter.GetChild(0).GetChild(1).GetComponent<Image>();
		segmentOverlays[meterLength-1] = powerMeter.GetChild(0).GetChild(0).GetComponent<Image>();
	}

	/*---------------------------------------------------- SET PIPS ALPHA ----------------------------------------------------*/

	void SetPipsAlpha()
	{
        if (power < 0)
            power = 0;

        Color placeHolderColor = new Color();
		for(int counter = 0; counter < power; counter++)
		{
			placeHolderColor = powerMeterPips[counter].color;
			placeHolderColor.a = 1f;
			powerMeterPips[counter].color = placeHolderColor;
		}

		for(int counter = System.Convert.ToInt32(power); counter < meterLength; counter++)
		{
			placeHolderColor = powerMeterPips[counter].color;
			placeHolderColor.a = 0f;
			powerMeterPips[counter].color = placeHolderColor;
		}
	}

	/*---------------------------------------------------- SET JETPACKING INDICATOR ----------------------------------------------------*/

	[PunRPC]
	public void SetJetpackingIndicator(bool jetpacking, bool isRedPlayer)
	{
		Color placeHolderColor;
		
		for(int counter = 0; counter < segmentOverlays.Length; counter++)
		{
			placeHolderColor = segmentOverlays[counter].color;
			
			if(jetpacking)
			{
				if(isRedPlayer)
				{
					placeHolderColor.r = 1;
					redPlayerJetpacking = true;
					if(!bluePlayerJetpacking)
						StartCoroutine(JetpackFadeEffect());
				}
				else
				{
					placeHolderColor.b = 1;
					bluePlayerJetpacking = true;
					if(!redPlayerJetpacking)
						StartCoroutine(JetpackFadeEffect());
				}
			}
			else
			{
				if(isRedPlayer)
				{
					placeHolderColor.r = 0;
					redPlayerJetpacking = false;
				}
				else
				{
					placeHolderColor.b = 0;
					bluePlayerJetpacking = false;
				}
			}

			segmentOverlays[counter].color = placeHolderColor;
		}
	}

	/*---------------------------------------------------- JETPACK FADE EFFECT ----------------------------------------------------*/

	IEnumerator JetpackFadeEffect()
	{
		Color placeHolderColor;
		float currentAlpha = 0f;
		bool rising = true;

		while(redPlayerJetpacking||bluePlayerJetpacking)
		{
			if(rising)
			{
				if(currentAlpha < 0.5f)
					currentAlpha += Time.deltaTime;
				else
					rising = false;
			}
			else
			{
				if(currentAlpha > 0f)
					currentAlpha -= Time.deltaTime;
				else
					rising = true;
			}

			foreach(Image segmentOverlay in segmentOverlays)
			{
				placeHolderColor = segmentOverlay.color;
				placeHolderColor.a = currentAlpha;
				segmentOverlay.color = placeHolderColor;
			}

			yield return null;
		}

		foreach(Image segmentOverlay in segmentOverlays)
		{
			placeHolderColor = segmentOverlay.color;
			placeHolderColor.a = 0f;
			segmentOverlay.color = placeHolderColor;
        }
    }
    
	/*---------------------------------------------------- SET RECHARGING ----------------------------------------------------*/

    [PunRPC]
	public void SetRecharging()
	{
		if(depletionRate == 0)
			recharging = true;
		else
			recharging = false;

		if(recharging)
			StartCoroutine(RechargeRatePause());
		else
			rechargeRate = 0;

	}

	/*---------------------------------------------------- RECHARGE RATE PAUSE ----------------------------------------------------*/

	IEnumerator RechargeRatePause()
	{
		rechargeRate = 0;
		float waitTimer = 0;
		while(recharging && waitTimer<1)
		{
			waitTimer += Time.deltaTime;
			yield return null;
		}
		rechargeRate = rechargeSpeed;
	}

	/*---------------------------------------------------- INITIALISATION ----------------------------------------------------*/

	void Initialisation()
	{
        maxPowerText = GameObject.Find("MaxPower Value").GetComponent<Text>();
        powerText = GameObject.Find("Power Value").GetComponent<Text>();

		maxPower = startingMaxPower;
		power = maxPower;
		meterLength = maxPower;
		
		nextPipDown = System.Convert.ToSByte(Mathf.Floor(power-1));
		nextPipUp = System.Convert.ToSByte(Mathf.Floor(power));
		
		maxPowerText.text = maxPower.ToString();
		powerText.text = power.ToString();
		
		powerMeter = GameObject.Find("Power Meter").transform;
		powerMeterSegment = Resources.Load<GameObject>("Power_Meter_Segment");
		powerMeterPips = new Image[0];

		segmentOverlays = new Image[0];
		
		InstantiatePowerMeter();
	}
}
