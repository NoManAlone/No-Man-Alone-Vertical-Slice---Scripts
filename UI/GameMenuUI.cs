using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMenuUI : MonoBehaviour
{
    public Transform menuPanel;

	public bool menuOpen;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(menuPanel.gameObject.activeSelf)           
                CloseMenu();
            
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
		menuOpen = true;
		Cursor.visible = true;
        menuPanel.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
		menuOpen = false;

		if(!GameObject.Find("GameManager").GetComponent<GameManager>().myPlayer.GetComponent<PlayerControl>().usingConsole)
			Cursor.visible = false;

		menuPanel.gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        PhotonNetwork.Disconnect();
        Application.LoadLevel("Lobby");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
