using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public GameObject itemPanelPrefab;

    private GameObject panel;

	void Start ()
    {


        //this.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 30*3);
        //panel = this.transform.Find("Panel").gameObject;

        //for (int i = 0; i < 3; i++)
        //{
        //    GameObject itemPanel = GameObject.Instantiate(itemPanelPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        //    itemPanel.transform.Find("OrderText").GetComponent<Text>().text = (i + 1).ToString();
        //    itemPanel.transform.Find("NameText").GetComponent<Text>().text = "zd" + (i + 1).ToString();
        //    itemPanel.transform.Find("ScaleText").GetComponent<Text>().text = "z" + (i + 1).ToString();
        //    itemPanel.transform.SetParent(panel.transform, false);
        //}

    }
	

	void Update ()
    {

		
	}

    public void UpdatePlayerCount()
    {

    }
}
