using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnect : Photon.PunBehaviour {
	public int maxPlayerPerRoom=1;
	// Use this for initialization
	void Start () {
       
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.AuthValues = new AuthenticationValues(Random.Range(0, 10000).ToString());
        PhotonNetwork.ConnectUsingSettings("0.0.1");
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
	public void StartMatching(){
        
        PhotonNetwork.JoinRandomRoom();
	}

	public override void OnCreatedRoom(){
		Debug.Log("create room");
	}
	public override void OnJoinedRoom()
	{
		Debug.Log("joined room");
		PhotonNetwork.automaticallySyncScene = true;

		PhotonNetwork.LoadLevel("GameScene");

	}
	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		int number = PhotonNetwork.playerList.Length;
		Debug.Log(number + "players now in the room");
		if (number == maxPlayerPerRoom){
			PhotonNetwork.LoadLevel("GameScene");
		}
	}

    //加入房间失败，自己创建房间
    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        RoomOptions roomOptions = new RoomOptions { IsVisible = true, MaxPlayers = 2 , PublishUserId = true };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }
}
