using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnect : Photon.PunBehaviour {
	public int maxPlayerPerRoom=2;
	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings("0.0.1");
		PhotonNetwork.AuthValues = new AuthenticationValues(Random.Range(0,10000).ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void StartMatching(){
		RoomOptions roomOptions = new RoomOptions{IsVisible = true, MaxPlayers = 2};
		PhotonNetwork.JoinOrCreateRoom(null,roomOptions,null);
	}

	public override void OnCreatedRoom(){
		Debug.Log("create room");
	}
	public override void OnJoinedRoom()
	{
		Debug.Log("joined room");
		PhotonNetwork.automaticallySyncScene = true;
	}
	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
		int number = PhotonNetwork.playerList.Length;
		Debug.Log(number + "players now in the room");
		if (number == maxPlayerPerRoom){
			PhotonNetwork.LoadLevel("GameScene");
		}
	}
}
