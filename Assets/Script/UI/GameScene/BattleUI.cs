using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public GameObject itemPanelPrefab;

    private GameObject panel;
    private List<Text> nameList;
    private List<Text> sizeList;
    //private List<GameObject> itemList;
    public Color normalColor;
    public Color highlightColor; 

    void Awake()
    {
        panel = this.transform.Find("Panel").gameObject;
        nameList = new List<Text>();
        sizeList = new List<Text>();
        //itemList = new List<GameObject>();

        
        if(normalColor == Color.clear)
        {
            normalColor = new Color(255, 255, 255, 100);
        }
        if(highlightColor == Color.clear)
        {
            highlightColor = new Color(255, 138, 0, 255);
        }
    }


    //private int curFrame = 0;
    //private int maxFrame = 10;
    //public void updateSeveralFrame()
    //{
    //    curFrame = 0;
    //}

    // 排名发生变化时更新，只更新几帧
    void Update ()
    {
        //if (curFrame++ > maxFrame)
        //{
        //    return;
        //}
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

        SortPlayerList();
        for (int i = 0; i < playerCount; i++)
        {
            nameList[i].text = networkManager.playerList[i].GetPlayerName();
            sizeList[i].text = networkManager.playerList[i].GetPlayerSize().ToString("0.0");

            Image curItemImage = nameList[i].transform.parent.gameObject.GetComponent<Image>();
            if ( networkManager.playerList[i].gameObject == networkManager.localPlayer )
            {
                curItemImage.color = highlightColor;
                //itemList[i].GetComponent<Image>().color = highlightColor;
            }
            else
            {
                curItemImage.color = normalColor;
                //itemList[i].GetComponent<Image>().color = normalColor;
            }
        }
    }

    // 增加一个player时，增加一个排行榜的item, 并需更新 nameList、sizeList
    public void AddPlayer()
    {
        int playerCount = networkManager.playerList.Count;
        if (playerCount < 1)
        {
            return;
        }
        this.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, 30 * (playerCount +1));

        // 增加的player必须在networkManager.playerList[playerCount - 1]
        GameObject itemPanel = GameObject.Instantiate(itemPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        Text orderText = itemPanel.transform.Find("OrderText").GetComponent<Text>();
        Text nameText = itemPanel.transform.Find("NameText").GetComponent<Text>();
        Text scaleText = itemPanel.transform.Find("ScaleText").GetComponent<Text>();

        orderText.text = (nameList.Count+1).ToString();
        nameText.text = networkManager.playerList[playerCount - 1].GetPlayerName();
        scaleText.text = networkManager.playerList[playerCount - 1].GetPlayerSize().ToString("0.0");
        itemPanel.transform.SetParent(panel.transform, false);

        // 维护Text列表
        nameList.Add(nameText);
        sizeList.Add(scaleText);
        //itemList.Add(itemPanel);

        // 更新name，由于player的创建是异步的，需要时间
        // updateSeveralFrame();
    }

    // 当一个player死亡时，减少一个排行榜的item
    public void RemovePlayer()
    {
        if (this == null)
        {
            return;
        }
        int playerCount = networkManager.playerList.Count;

        // 删除排行榜最后一个，为了不用维护 orderList，重新更新即可
        this.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, 30 * (playerCount + 1));
        Destroy(nameList[playerCount].transform.parent.gameObject);
        nameList.RemoveAt(playerCount);
        sizeList.RemoveAt(playerCount);
        //itemList.RemoveAt(playerCount);
    }

    // 插入排序
    private void SortPlayerList()
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
