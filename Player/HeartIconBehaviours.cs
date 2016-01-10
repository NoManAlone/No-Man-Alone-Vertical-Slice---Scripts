using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartIconBehaviours : MonoBehaviour
{
	Image fillImage, containerImage;

	float startingPosition = -1.4f;

	PhotonView playerAnimPhotonView;

	bool incapacitated = false, reviving = false;

	void Start()
	{
		fillImage = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();
		containerImage = transform.GetChild(1).GetChild(1).GetComponent<Image>();

		fillImage.enabled = false;
		containerImage.enabled = false;

		playerAnimPhotonView = GetComponent<PhotonView>();
	}

	public void CallStartIconAnimate()
	{
		playerAnimPhotonView.RPC("StartIconAnimate", PhotonTargets.AllBuffered);
    }

	public void CallEndIconAnimate()
	{
		playerAnimPhotonView.RPC("EndIconAnimate", PhotonTargets.AllBuffered);
	}

	public void CallSwitchReviving(bool isReviving)
	{
		playerAnimPhotonView.RPC("SwitchReviving", PhotonTargets.AllBuffered, isReviving);
	}

	[PunRPC]
	void StartIconAnimate()
	{
		incapacitated = true;
		StartCoroutine(IconAnimate());
	}

	[PunRPC]
	void EndIconAnimate()
	{
		incapacitated = false;
		reviving = false;
	}

	[PunRPC]
	void SwitchReviving(bool isReviving)
	{
		reviving = isReviving;
	}

	IEnumerator IconAnimate()
	{
		while(incapacitated)
		{
			// Fill up heart if reviving.
			if(reviving)
			{
				containerImage.enabled = true;
				fillImage.enabled = true;
				if(fillImage.GetComponent<RectTransform>().localPosition.y < 0)
					fillImage.GetComponent<RectTransform>().localPosition = new Vector2(0, fillImage.rectTransform.anchoredPosition.y+(Time.deltaTime*(1.4f/2f)));
				yield return null;
			}

			// Flash empty heart if not reviving.
			else
			{
				fillImage.rectTransform.anchoredPosition = new Vector2(0, startingPosition);
				fillImage.enabled = false;
				containerImage.enabled = true;
				yield return new WaitForSeconds(0.5f);
				containerImage.enabled = false;
                yield return new WaitForSeconds(0.5f);
			}
		}

		fillImage.enabled = false;
		containerImage.enabled = false;
	}

//	public void IconAnimate()
//	{
//		while(playerControl.incapacitated)
//		{
//			// Fill up heart if reviving.
//			if(playerControl.reviving)
//			{
//				playerAnimPhotonView.RPC("CallFillHeart", PhotonTargets.AllBuffered);
//			}
//			
//			// Flash empty heart if not reviving.
//			else
//			{
//				playerAnimPhotonView.RPC("CallFlashHeart", PhotonTargets.AllBuffered);
//			}
//		}
//		
//		playerAnimPhotonView.RPC("EndIconAnimate", PhotonTargets.AllBuffered);
//	}
//	
//	[PunRPC]
//	void CallFillHeart()
//	{
//		StartCoroutine(FillHeart());
//	}
//	
//	[PunRPC]
//	void CallFlashHeart()
//	{
//		StartCoroutine(FlashHeart());
//	}
//	
//	[PunRPC]
//	void EndIconAnimate()
//	{
//		fillImage.enabled = false;
//		containerImage.enabled = false;
//	}
//	
//	IEnumerator FillHeart()
//	{
//		containerImage.enabled = true;
//		fillImage.enabled = true;
//		if(fillImage.GetComponent<RectTransform>().localPosition.y < 0)
//			fillImage.GetComponent<RectTransform>().localPosition = new Vector2(0, fillImage.rectTransform.anchoredPosition.y+(Time.deltaTime*(1.4f/2f)));
//        yield return null;
//    }
//    
//    IEnumerator FlashHeart()
//    {
//        fillImage.rectTransform.anchoredPosition = new Vector2(0, startingPosition);
//        fillImage.enabled = false;
//        containerImage.enabled = true;
//        yield return new WaitForSeconds(0.5f);
//        containerImage.enabled = false;
//        yield return new WaitForSeconds(0.5f);
//	}
}