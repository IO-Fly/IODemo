using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public Dropdown dropdown;
    public GameObject[] characterPrefab;

    private void Start()
    {
        // 播放大厅背景音乐
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlayLobbyBackground();

        // 初始化角色名字下拉框
        SetCharacterNameList();
    }

    // 初始化角色名字
    private void SetCharacterNameList()
    {
        List<string> optionList = new List<string>();
        for (int i = 0; i < characterPrefab.Length; i++)
        {
            optionList.Add(characterPrefab[i].name);
        }
        if (characterPrefab.Length > 0)
        {
            dropdown.AddOptions(optionList);
            dropdown.captionText.text = optionList[0];//初始选择角色
            PhotonNetwork.player.NickName = dropdown.options[dropdown.value].text;
        }
    }

    // 开始匹配
    public void OnStartMatching()
    {
        PhotonNetwork.player.NickName = dropdown.options[dropdown.value].text;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("0.0.1");
        }
    }
}
