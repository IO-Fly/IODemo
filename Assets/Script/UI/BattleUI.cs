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

	void Update ()
    {

		
	}

    public void addPlayer()
    {
        players = networkManager.GetPlayerList();

        Debug.Log("玩家数目" + players.Length);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30 * players.Length);

        GameObject itemPanel = GameObject.Instantiate(itemPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        itemPanel.transform.Find("OrderText").GetComponent<Text>().text = players.Length.ToString();
        itemPanel.transform.Find("NameText").GetComponent<Text>().text = players[players.Length - 1].GetComponent<Player>().GetPlayerName();
        itemPanel.transform.Find("ScaleText").GetComponent<Text>().text = players[players.Length - 1].GetComponent<Player>().GetPlayerSize().ToString();
        itemPanel.transform.SetParent(panel.transform, false);

    }
}
