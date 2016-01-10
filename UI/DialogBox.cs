using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DialogBox : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

	RectTransform rectTransform;

	Text dialogText;

	IEnumerator dialogBoxRoutine;

	GameObject wasdKeys, eKey, mouseButton;

	GameObject selectedIcon;

	/*---------------------------------------------------- AWAKE ----------------------------------------------------*/

	void Awake()
	{
		rectTransform = transform.parent.GetComponent<RectTransform>();
		dialogText = GetComponent<Text>();

		wasdKeys = transform.parent.FindChild("TutorialIconWASD").gameObject;
		eKey = transform.parent.FindChild("TutorialIconE").gameObject;
		mouseButton = transform.parent.FindChild("TutorialIconMouse").gameObject;
	}

	/*---------------------------------------------------- START AND END DIALOG BOX ----------------------------------------------------*/

	public void StartDialogBox(string textToDisplay, byte imageUsed = 0)
	{
		StopAllCoroutines();

		dialogBoxRoutine = DisplayDialogBox(textToDisplay, imageUsed);
		StartCoroutine(dialogBoxRoutine);
	}

	public void EndDialogBox()
	{
		StopAllCoroutines();

		dialogBoxRoutine = HideDialogBox();
		StartCoroutine(dialogBoxRoutine);
	}

	/*---------------------------------------------------- TRANSITION DIALOG BOX ----------------------------------------------------*/

	public void CallTransitionDialogBox(string textToDisplay, byte imageUsed)
	{
		StopAllCoroutines();
		StartCoroutine(TransitionDialogBox(textToDisplay, imageUsed));
	}

	IEnumerator TransitionDialogBox(string textToDisplay, byte imageUsed)
	{
		dialogBoxRoutine = HideDialogBox();
		yield return StartCoroutine(dialogBoxRoutine);
		dialogBoxRoutine = DisplayDialogBox(textToDisplay, imageUsed);
		yield return StartCoroutine(dialogBoxRoutine);
	}

	/*---------------------------------------------------- DISPLAY DIALOG BOX ----------------------------------------------------*/

	IEnumerator DisplayDialogBox(string textToDisplay, byte imageUsed = 0)
	{
		dialogText.text = ""; // Clear last text before starting next dialog.
		yield return StartCoroutine(MoveDialogBox(true));

		if(selectedIcon)
			yield return StartCoroutine(FadeIcon(false));

		switch(imageUsed)
		{
		case 0:
			break;
		case 1:
			selectedIcon = wasdKeys;
			break;
		case 2:
			selectedIcon = eKey;
			break;
		case 3:
			selectedIcon = mouseButton;
			break;
		default:
			break;
		}

		if(selectedIcon)
			yield return StartCoroutine(FadeIcon(true));
			
		yield return StartCoroutine(ScrollText(textToDisplay));
	}

	/*---------------------------------------------------- HIDE DIALOG BOX ----------------------------------------------------*/

	IEnumerator HideDialogBox()
	{
		if(selectedIcon)
			yield return StartCoroutine(FadeIcon(false));

		yield return StartCoroutine(MoveDialogBox(false));
	}

	/*---------------------------------------------------- SCROLL TEXT ----------------------------------------------------*/

	IEnumerator ScrollText(string textToDisplay)
	{
		for(int counter = 0; counter < textToDisplay.Length+1; counter++)
		{
			dialogText.text = textToDisplay.Substring(0, counter);
			yield return new WaitForSeconds(0.03f);
		}
	}

	/*---------------------------------------------------- MOVE DIALOG BOX ----------------------------------------------------*/

	IEnumerator MoveDialogBox(bool goingUp)
	{
		if(goingUp)
		{
			while(rectTransform.anchoredPosition.y < 0)
			{
				rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y+5);
				yield return null;
			}
		}

		else
		{
			while(rectTransform.anchoredPosition.y > -180)
			{
				rectTransform.anchoredPosition = new Vector2(0, rectTransform.anchoredPosition.y-5);
				yield return null;
			}
		}
	}

	/*---------------------------------------------------- FADE ICON ----------------------------------------------------*/

	IEnumerator FadeIcon(bool fadingIn)
	{
		Image iconImage = selectedIcon.GetComponent<Image>();
		Color colorPlaceholder = iconImage.color;

		if(fadingIn)
		{
			colorPlaceholder.a = 0;

			while(colorPlaceholder.a < 1)
			{
				colorPlaceholder.a += Time.deltaTime*3;
				iconImage.color = colorPlaceholder;
				yield return null;
			}

			iconImage.color = Color.white;
		}

		else
		{
			colorPlaceholder.a = 1;

			while(colorPlaceholder.a > 0)
			{
				colorPlaceholder.a -= Time.deltaTime*3;
				iconImage.color = colorPlaceholder;
				yield return null;
			}

			iconImage.color = new Color(1, 1, 1, 0);

			selectedIcon = null;
		}
	}
}
