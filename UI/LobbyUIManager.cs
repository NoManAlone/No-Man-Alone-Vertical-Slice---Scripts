using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LobbyUIManager : MonoBehaviour
{
	/*---------------------------------------------------- DECLARATIONS ----------------------------------------------------*/

    public GameObject panelCreateJoin, panelWaiting;

    public GameObject createGamePanel;
    public Transform contentPanelGamesList;

    RoomInfo[] gamesList;

    public List<GameObject> availableGames = new List<GameObject>();

    Button buttonJoinGame, buttonCreateGame;

	/*---------------------------------------------------- AWAKE & UPDATE ----------------------------------------------------*/

    void Awake()
    {
        buttonJoinGame = GameObject.Find("Button_JoinGame").GetComponent<Button>();
        buttonCreateGame = GameObject.Find("Button_CreateGame").GetComponent<Button>();
    }
	
	void Update ()
    {        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
			CancelCreateGame();
        }

        if (PhotonNetwork.insideLobby)
            buttonCreateGame.interactable = true;

        else
            buttonCreateGame.interactable = false;

        if (PhotonNetwork.GetRoomList().Length <= 0)
            buttonJoinGame.interactable = false;

        else
            buttonJoinGame.interactable = true;

    }

	/*---------------------------------------------------- CANCEL CREATE GAME ----------------------------------------------------*/

	public void CancelCreateGame()
	{
		if (createGamePanel.activeSelf)
			createGamePanel.SetActive(false);
	}

	/*---------------------------------------------------- SHOW AND CLOSE POP-UP ----------------------------------------------------*/

    public void ShowPopup(GameObject panel)
    {
        panel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(panel.transform.GetChild(0).gameObject, null);
    }
	
    public void ClosePopup(GameObject panel)
    {
        panel.transform.GetChild(0).GetComponent<InputField>().interactable = false;
        panel.SetActive(false);
    }

	/*---------------------------------------------------- ENABLE & DISABLE BUTTON INTERACTION ----------------------------------------------------*/

    public void EnableButtonInteraction(GameObject button)
    {
        button.GetComponent<Button>().interactable = true;
        EventSystem.current.SetSelectedGameObject(null);
    }
	
    public void DisableButtonInteraction(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
    }

	/*---------------------------------------------------- WAITING FOR PLAYER ----------------------------------------------------*/

    public void WaitingForPlayer()
    {
        createGamePanel.SetActive(false);
        StartCoroutine(CanvasFadeOut(panelCreateJoin));
        StartCoroutine(CanvasFadeIn(panelWaiting));
    }

	/*---------------------------------------------------- CANCEL GAME ----------------------------------------------------*/

    public void CancelGame()
    {
        StartCoroutine(CanvasFadeOut(panelWaiting));
        StartCoroutine(CanvasFadeIn(panelCreateJoin)); 
    }

	/*---------------------------------------------------- CANVAS FADE-IN AND FADE-OUT ----------------------------------------------------*/

    public IEnumerator CanvasFadeIn(GameObject go)
    {
        while (go.GetComponent<CanvasGroup>().alpha < 1f)
        {
            go.GetComponent<CanvasGroup>().alpha += 8 * Time.deltaTime;
            yield return null;
        }
        
        go.GetComponent<CanvasGroup>().interactable = true;
        go.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public IEnumerator CanvasFadeOut(GameObject go)
    {
        while (go.GetComponent<CanvasGroup>().alpha > 0)
        {
            go.GetComponent<CanvasGroup>().alpha -= 8 * Time.deltaTime;
            yield return null;
        }

        go.GetComponent<CanvasGroup>().interactable = false;
        go.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

	/*---------------------------------------------------- POPULATE GAMES LIST ----------------------------------------------------*/

    public void PopulateGamesList()
    {

        foreach (Transform child in contentPanelGamesList)
        {
            GameObject.Destroy(child.gameObject);
            availableGames.Clear();
        }

        gamesList = PhotonNetwork.GetRoomList();

        foreach (RoomInfo game in gamesList)
        {
            GameObject newGameButton = Instantiate(Resources.Load("GamesListButton")) as GameObject;
            newGameButton.transform.SetParent(contentPanelGamesList);
            newGameButton.transform.localScale = new Vector3(1, 1, 1);
            newGameButton.transform.FindChild("Text").GetComponent<Text>().text = game.name;

            availableGames.Add(newGameButton);
        }
    }

	/*---------------------------------------------------- QUIT GAME ----------------------------------------------------*/

    public void QuitGame()
    {
        Application.Quit();
    }
}
