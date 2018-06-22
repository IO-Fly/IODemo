using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class networkManager :Photon.PunBehaviour {

    public static GameObject localPlayer;
    public static List<Player> playerList = new List<Player>();
    public GameObject foodPrefab;
	public GameObject poisonPrefab;
    public GameObject moveablePrefab;
    public int foodCount=300;
	public int poisonCount=100;
    public int moveableCount = 5;
	private int count=0;
	// Use this for initialization
    void Awake(){
		PhotonNetwork.sendRate=20;
		PhotonNetwork.sendRateOnSerialize=20;
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnLevelWasLoaded(int level){

        //播放游戏背景音乐
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlayGameBackground();

        CreatePlayer();

        //在主客户端加载场景
        if (PhotonNetwork.isMasterClient){
            //CreateFood();
			//this.InvokeRepeating("DelayFood", 1f,0.2f);
        }    	
	}

	private void CreateFood(){
		for(int i=0;i<foodCount;i++){
			PhotonNetwork.InstantiateSceneObject(foodPrefab.name, new Vector3(Random.Range(-90,90), Random.Range(-95,-5), Random.Range(-90,90)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
		}
		for(int i=0;i<poisonCount;i++){
			PhotonNetwork.InstantiateSceneObject(poisonPrefab.name, new Vector3(Random.Range(-90,90), Random.Range(-95,-5), Random.Range(-90,90)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
		}

        for (int i = 0; i < moveableCount; i++)
        {
            GameObject thisObject = PhotonNetwork.InstantiateSceneObject(moveablePrefab.name, new Vector3(Random.Range(-90, 90), Random.Range(-95, -5), Random.Range(-90, 90)), Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)), 0, null);
        }
        FoodAI.allPlayers = playerList;
    }
	private void DelayFood(){
			if(count>=200){
				this.CancelInvoke();
			}
			Debug.Log("执行次数： "+count);
			PhotonNetwork.InstantiateSceneObject(foodPrefab.name, new Vector3(Random.Range(-20,20), Random.Range(-95,-5), Random.Range(-20,20)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
			count+=1;
	}

	private void CreatePlayer(){

        string characterName = PhotonNetwork.player.NickName;
        Debug.Log(characterName);
		GameObject localPlayer = PhotonNetwork.Instantiate(characterName, new Vector3(Random.Range(-80,80),Random.Range(-80,-20),Random.Range(-80,80)),Quaternion.identity, 0);
        networkManager.localPlayer = localPlayer;//缓存本地玩家对象

        GameObject playerCamera = GameObject.Find("PlayerCamera");
        playerCamera.GetComponent<CameraController>().setPlayer(localPlayer);//将摄像机指向本地玩家

        GameObject minimapCamera = GameObject.Find("MinimapCamera");
        minimapCamera.GetComponent<MinimapCameraFllow>().setPlayer(localPlayer);//将Minimap摄像机指向本地玩家

        GameObject rootCanvas = GameObject.Find("HUDCanvas");
        GameObject skillUI = rootCanvas.transform.Find("SkillUI").gameObject;
        skillUI.GetComponent<ShowSkill>().setPlayer(localPlayer);//将指向本地玩家

    }

}
