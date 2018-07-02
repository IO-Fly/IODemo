using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class networkManager :Photon.PunBehaviour {

    public static GameObject localPlayer;
    public static List<Player> playerList = new List<Player>();

	// Use this for initialization
    void Awake(){
		PhotonNetwork.sendRate=20;
		PhotonNetwork.sendRateOnSerialize=20;
	}

	// Update is called once per frame
	private void OnLevelWasLoaded(int level){

        //播放游戏背景音乐
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlayGameBackground();

        //创建玩家
        CreatePlayer(); 
	}

	private void CreatePlayer(){

        string characterName = PhotonNetwork.player.NickName;
        Debug.Log(characterName);
		GameObject localPlayer = PhotonNetwork.Instantiate(characterName, new Vector3(Random.Range(-80,80),Random.Range(-80,-20),Random.Range(-80,80)),Quaternion.identity, 0);
        networkManager.localPlayer = localPlayer;//缓存本地玩家对象

        GameObject playerCamera = GameObject.Find("PlayerCamera");
        playerCamera.GetComponent<CameraController>().setPlayer(localPlayer);//将摄像机指向本地玩家

        GameObject minimapCamera = GameObject.Find("MinimapCamera");
        minimapCamera.GetComponent<MinimapCameraFllow>().SetPlayer(localPlayer);//将Minimap摄像机指向本地玩家

        GameObject rootCanvas = GameObject.Find("HUDCanvas");
        GameObject skillUI = rootCanvas.transform.Find("SkillUI").gameObject;
        skillUI.GetComponent<SkillUI>().setPlayer(localPlayer);//将指向本地玩家
        GameObject.Find("SUIMONO_Module").GetComponent<Suimono.Core.SuimonoModule>().setTrack = localPlayer.transform;

    }
}
