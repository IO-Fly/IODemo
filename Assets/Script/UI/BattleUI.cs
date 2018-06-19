using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public GameObject itemPanelPrefab;

    private GameObject panel;
    private List<Text> nameList;
    private List<Text> sizeList;

    void Awake()
    {
        panel = this.transform.Find("Panel").gameObject;
        nameList = new List<Text>();
        sizeList = new List<Text>();
    }

	void Update ()
    {
        int playerCount = networkManager.playerList.Count;
        if (playerCount < 1)
        {
            return;
        }
        if (playerCount != nameList.Count)
        {
            Debug.LogWarning("玩家列表未更新");
            return;
        }

        sortPlayerList();
        for (int i = 0; i < playerCount; i++)
        {
            nameList[i].text = networkManager.playerList[i].GetPlayerName();
            sizeList[i].text = networkManager.playerList[i].GetPlayerSize().ToString();
        }
    }

    // 增加一个player时，增加一个排行榜的item, 并需更新 nameList、sizeList
    public void addPlayer()
    {
        int playerCount = networkManager.playerList.Count;
        if (playerCount < 1)
        {
            return;
        }
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30 * (playerCount +1));

        // 增加的player必须在networkManager.playerList[playerCount - 1]
        GameObject itemPanel = GameObject.Instantiate(itemPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        Text orderText = itemPanel.transform.Find("OrderText").GetComponent<Text>();
        Text nameText = itemPanel.transform.Find("NameText").GetComponent<Text>();
        Text scaleText = itemPanel.transform.Find("ScaleText").GetComponent<Text>();

        orderText.text = (playerCount).ToString();
        nameText.text = networkManager.playerList[playerCount - 1].GetPlayerName();
        scaleText.text = networkManager.playerList[playerCount - 1].GetPlayerSize().ToString();
        itemPanel.transform.SetParent(panel.transform, false);

        // 维护Text列表
        nameList.Add(nameText);
        sizeList.Add(scaleText);
    }

    // 当一个player死亡时，减少一个排行榜的item
    public void removePlayer()
    {
        int playerCount = networkManager.playerList.Count;

        this.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30 * (playerCount + 1));
        Destroy(nameList[playerCount].transform.parent.gameObject);
        nameList.RemoveAt(playerCount);
        sizeList.RemoveAt(playerCount);

    }

    // 插入排序
    private void sortPlayerList()
    {
        int playerCount = networkManager.playerList.Count;

        for(int i = 1; i < playerCount; i++)
        {
            int j = i - 1;
            Player current = networkManager.playerList[i];
            while(j > -1 && networkManager.playerList[j].GetPlayerSize() < current.GetPlayerSize())
            {
                networkManager.playerList[j + 1] = networkManager.playerList[j];
                j--;
            }
            networkManager.playerList[j + 1] = current;
        }
    }


}
