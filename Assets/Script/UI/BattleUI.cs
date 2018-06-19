using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public GameObject itemPanelPrefab;

    private GameObject panel;

    private GameObject[] players;

    void Awake()
    {
        panel = this.transform.Find("Panel").gameObject;
    }

	void FixedUpdate ()
    {
        players = networkManager.GetPlayerList();

        clearItem();
        for (int i = 0; i < players.Length; i++)
        {
            GameObject itemPanel = GameObject.Instantiate(itemPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
            itemPanel.transform.Find("OrderText").GetComponent<Text>().text = (i + 1).ToString();
            itemPanel.transform.Find("NameText").GetComponent<Text>().text = players[i].GetComponent<Player>().GetPlayerName();
            itemPanel.transform.Find("ScaleText").GetComponent<Text>().text = players[i].GetComponent<Player>().GetPlayerSize().ToString();
            itemPanel.transform.SetParent(panel.transform, false);
        }

    }

    public void addPlayer()
    {
        players = networkManager.GetPlayerList();

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30 * players.Length);

        clearItem();
        for (int i = 0; i < players.Length; i++)
        {
            GameObject itemPanel = GameObject.Instantiate(itemPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
            itemPanel.transform.Find("OrderText").GetComponent<Text>().text = (i+1).ToString();
            itemPanel.transform.Find("NameText").GetComponent<Text>().text = players[i].GetComponent<Player>().GetPlayerName();
            itemPanel.transform.Find("ScaleText").GetComponent<Text>().text = players[i].GetComponent<Player>().GetPlayerSize().ToString();
            itemPanel.transform.SetParent(panel.transform, false);
        }


    }

    private void clearItem()
    {
        for(int i = 1; i < panel.transform.childCount; i++)
        {
            Destroy(panel.transform.GetChild(i).gameObject);
        }
    }
}
