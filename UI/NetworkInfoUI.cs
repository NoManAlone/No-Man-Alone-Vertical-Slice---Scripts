using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkInfoUI : MonoBehaviour
{

    public GameObject networkInfoPanel, networkInfoContent;
    Text playerName, playerPing;


    void Awake()
    {
        GameObject playerInfo = Instantiate(Resources.Load("PlayerNetworkInfo")) as GameObject;
        playerInfo.transform.SetParent(networkInfoContent.transform);
        playerInfo.transform.localScale = new Vector3(1, 1, 1);

        playerName = playerInfo.transform.FindChild("Name").GetComponent<Text>();
        playerPing = playerInfo.transform.FindChild("Ping").GetComponent<Text>();
    }

    void Update ()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (!networkInfoPanel.activeSelf)
                networkInfoPanel.SetActive(true);

            GetPlayerInfo();
        }

        else
        {
            if (networkInfoPanel.activeSelf)
                networkInfoPanel.SetActive(false);
        }
	}

    void GetPlayerInfo()
    {
        playerName.text = PhotonNetwork.player.name;
        playerPing.text = PhotonNetwork.GetPing().ToString();
    }
}
