using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GamesListButton : MonoBehaviour
{
    LobbyManager networkManager;   
    Button button;

    List<GameObject> availableGames;

    void Awake()
    {
        networkManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        availableGames = GameObject.Find("LobbyUIManager").GetComponent<LobbyUIManager>().availableGames;

        button = GetComponent<Button>();
        button.onClick.AddListener(() => { GetGame();});
    }

    public void GetGame()
    {
        networkManager.selectedGameText = button.transform.FindChild("Text").GetComponent<Text>().text;
        button.GetComponent<Image>().color = Color.white;

        foreach (GameObject game in availableGames)
        {
            if(game != this.gameObject)
                game.GetComponent<Image>().color = Color.clear;
        }  
    }
}
