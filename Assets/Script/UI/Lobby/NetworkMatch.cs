using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkMatch : Photon.PunBehaviour
{
    public static string playerName;    // 在游戏过程中保留当前玩家名称
    public int maxPlayerPerRoom = 2;    // 单个房间最多玩家数目

    AsyncOperation asyncOperation;

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinRandomRoom();
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

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }
        else
        {
            Debug.Log("服务器连接断开");
        }
        
    }
 

    void Update()
    {

        if (PhotonNetwork.inRoom && PhotonNetwork.room.PlayerCount == maxPlayerPerRoom
            && asyncOperation == null)
        {
            asyncOperation = PhotonNetwork.LoadLevelAsync("GameScene");   
            asyncOperation.allowSceneActivation = false;
            Debug.Log("正在进入游戏");
        }
         
        if(asyncOperation != null)
        {
            Debug.LogWarning("Progress: " + asyncOperation.progress);
            if(asyncOperation.progress >= 0.90f)
            {
                asyncOperation.allowSceneActivation = true;
                Debug.Log("进入游戏");
            }
        }

    }


}
