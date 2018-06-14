using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class networkManager :Photon.PunBehaviour {

    public static GameObject localPlayer;
    public GameObject foodPrefab;
    public int foodCount=300;
	private int count=0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnLevelWasLoaded(int level){
		CreatePlayer();

        //在主客户端加载场景
        if (PhotonNetwork.isMasterClient){
            //CreateFood();
			this.InvokeRepeating("DelayFood", 1f,0.2f);
        }    	
	}

	private void CreateFood(){
		for(int i=0;i<foodCount;i++){
			PhotonNetwork.InstantiateSceneObject(foodPrefab.name, new Vector3(Random.Range(-10,10), Random.Range(-95,-5), Random.Range(-10,10)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
		}
	}
	private void DelayFood(){
			PhotonNetwork.InstantiateSceneObject(foodPrefab.name, new Vector3(Random.Range(-20,20), Random.Range(-95,-5), Random.Range(-20,20)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
			count+=1;
			if(count==300)
				this.CancelInvoke();
	}

	private void CreatePlayer(){

        string characterName = PhotonNetwork.player.NickName;
        Debug.Log(characterName);
		GameObject localPlayer = PhotonNetwork.Instantiate(characterName, new Vector3(Random.Range(-80,80),Random.Range(-80,-20),Random.Range(-80,80)),Quaternion.identity, 0);
        networkManager.localPlayer = localPlayer;//缓存本地玩家对象
        localPlayer.GetComponent<Player>().SetPlayerName(LobbyUIManager.playerName);//设置玩家名字

        GameObject playerCamera = GameObject.Find("PlayerCamera");
        playerCamera.GetComponent<CameraController>().setPlayer(localPlayer);//将摄像机指向本地玩家

        GameObject minimapCamera = GameObject.Find("MinimapCamera");
        minimapCamera.GetComponent<MinimapCameraFllow>().setPlayer(localPlayer);//将摄像机指向本地玩家
    }
}
