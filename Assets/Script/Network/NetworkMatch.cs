using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkMatch : Photon.PunBehaviour
{

    public static string playerName;    // 在游戏过程中保留当前玩家名称
    public int maxPlayerPerRoom = 2;    // 单个房间最多玩家数目
    public static string sceneName;     // 匹配的场景

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount < maxPlayerPerRoom)
        {
            //Debug.LogWarning("调用一次");
            GetComponent<LobbyUI>().OnMatchUI();
        }
    }

    //public override void OnCreatedRoom()
    //{
    //    Debug.Log("create room");
    //}

    // 加入房间成功
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("joined room");
        
    //}

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        RoomOptions roomOptions = new RoomOptions { IsVisible = true, MaxPlayers = (byte)maxPlayerPerRoom, PublishUserId = true };
        Debug.LogWarning("随便输出");
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }
        else
        {
            Debug.Log("服务器连接断开");
        }
        
    }

    bool isLoadScene = false;
    void Update()
    {
        if (isLoadScene == false && PhotonNetwork.inRoom && PhotonNetwork.room.PlayerCount == maxPlayerPerRoom)
        {
            // 关闭房间
            PhotonNetwork.room.IsOpen = false;

            // 开始加载游戏场景
            isLoadScene = true;
            GetComponent<SceneLoader>().LoadScene(NetworkMatch.sceneName);  
        }
    }

}
