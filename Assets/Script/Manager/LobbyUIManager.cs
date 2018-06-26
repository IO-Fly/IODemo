using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour {

    public static string playerName;//在游戏过程中保留玩家名称

    public InputField inputName;
    public Button startGame;
    public Button matching;
    public Dropdown dropdown;
    public GameObject[] characterPrefab;
    public Camera LobbyCamera;

    void Awake()
    {
        startGame.gameObject.SetActive(true);
        inputName.gameObject.SetActive(true);
        matching.gameObject.SetActive(false);
        dropdown.gameObject.SetActive(false);

        //播放大厅背景音乐
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlayLobbyBackground();

    }

    // Use this for initialization
    void Start()
    {

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
        GameObject playerTemp = GameObject.Find(dropdown.options[dropdown.value].text);
        LobbyCamera.GetComponent<LobbyCameraController>().setPlayer(playerTemp);
    }

    public void OnStartGame()
    {
        if(inputName != null)
        {
            //房间唯一ID，相同ID的用户不会加入同一个房间
            PhotonNetwork.AuthValues.UserId = inputName.text;
            LobbyUIManager.playerName = inputName.text;
            matching.gameObject.SetActive(true);
            dropdown.gameObject.SetActive(true);
            startGame.gameObject.SetActive(false);
            inputName.gameObject.SetActive(false);
        }
    }


}
