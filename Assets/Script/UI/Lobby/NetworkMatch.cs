using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkMatch : Photon.PunBehaviour
{
    public static string playerName;    // 在游戏过程中保留当前玩家名称
    public int maxPlayerPerRoom = 2;    // 单个房间最多玩家数目

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("create room");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined room");
        //PhotonNetwork.LoadLevel("GameScene");
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        int number = PhotonNetwork.room.PlayerCount;
        Debug.Log(number + "players now in the room");
        
        if (number == maxPlayerPerRoom)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    //加入房间失败，自己创建房间
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        RoomOptions roomOptions = new RoomOptions { IsVisible = true, MaxPlayers = (byte)maxPlayerPerRoom, PublishUserId = true };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }
}
