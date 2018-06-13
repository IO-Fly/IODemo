using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : Photon.PunBehaviour {

    public GameObject[] characterPrefab;
    public Dropdown dropdown;

    // Use this for initialization
    void Start () {

        List<string> optionList = new List<string>();
        for (int i = 0; i < characterPrefab.Length; i++)
        {
            optionList.Add(characterPrefab[i].name);
        }

        //初始化下拉框
        if (characterPrefab.Length > 0)
        {
            dropdown.AddOptions(optionList);
            dropdown.captionText.text = optionList[0];//初始选择角色
            PhotonNetwork.player.NickName = dropdown.options[dropdown.value].text;
        }
		
	}
	
    public void OnSelectCharacter()
    {
        PhotonNetwork.player.NickName = dropdown.options[dropdown.value].text;
        Debug.Log("选择角色： " + PhotonNetwork.player.NickName);
    }
    
}
