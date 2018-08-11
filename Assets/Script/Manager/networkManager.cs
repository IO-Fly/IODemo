using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class networkManager :Photon.PunBehaviour {

    public static GameObject localPlayer;
    public static List<Player> playerList = new List<Player>();
    Dictionary<int, string> characterAIDict = new Dictionary<int, string>();
    public int PlayerAINum = 2;

    // Use this for initialization
    void Awake(){

		PhotonNetwork.sendRate=20;
		PhotonNetwork.sendRateOnSerialize=20;
        characterAIDict[0] = "Kun_Copy_withAI";
        characterAIDict[1] = "Kun_Size_withAI";
        characterAIDict[2] = "Kun_Hide_withAI";
        characterAIDict[3] = "Kun_Speed_withAI";

        //CreatePlayer();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) { 
        
        //播放游戏背景音乐
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlayGameBackground();

        //创建玩家
        CreatePlayer();

        //激活非主客户端的玩家
        if (!PhotonNetwork.isMasterClient)
        {
            foreach (Player player in playerList)
            {
                player.gameObject.SetActive(true);
            }
        }

        //创建AI玩家
        if (PhotonNetwork.isMasterClient && SceneManager.GetActiveScene().name != "GameScene")
        {
            CreatePlayerAI();
        }
        

    }


	private void CreatePlayer(){

        string characterName = PhotonNetwork.player.NickName;
        Debug.Log(characterName);
		GameObject localPlayer = PhotonNetwork.Instantiate(characterName, FoodManager.GetInitPosition(),Quaternion.identity, 0);
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


    private void CreatePlayerAI()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        for(int i = 0; i < PlayerAINum; i++)
        {
            int randomIndex = Random.Range(0, 4);
            PhotonNetwork.InstantiateSceneObject(characterAIDict[randomIndex], FoodManager.GetInitPosition(), Quaternion.identity, 0, null);
        }  

    }
}
