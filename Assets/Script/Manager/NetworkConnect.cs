using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnect : Photon.PunBehaviour {
	public int maxPlayerPerRoom=2;
    // Use this for initialization

    void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues()
        {
            AuthType = CustomAuthenticationType.Custom
        };

    }

	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {	

	}
	public void StartMatching(){

        if(PhotonNetwork.connected){
			PhotonNetwork.JoinRandomRoom();
		}
		else {
            PhotonNetwork.ConnectUsingSettings("0.0.1");
        }
        
	}

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom(){
		Debug.Log("create room");
	}
	public override void OnJoinedRoom()
	{
		Debug.Log("joined room");
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
        RoomOptions roomOptions = new RoomOptions { IsVisible = true, MaxPlayers = (byte)maxPlayerPerRoom, PublishUserId = true };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }
}
