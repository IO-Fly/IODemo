using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetUp : MonoBehaviour {
	public Camera myCamera;
	public Behaviour[] stuffNeedDisable;
	PhotonView pv;
	// Use this for initialization
	void Start () {
		pv = GetComponent<PhotonView>();
		if (!pv.isMine){
			myCamera.enabled = false;
			for(int i = 0; i<stuffNeedDisable.Length;i++){
				stuffNeedDisable[i].enabled = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
