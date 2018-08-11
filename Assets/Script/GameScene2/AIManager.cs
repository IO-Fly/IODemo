using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : Photon.PunBehaviour {

	public GameObject playAI;
	// Use this for initialization
	void Start () {
		/*if(PhotonNetwork.isMasterClient)
		PhotonNetwork.InstantiateSceneObject(playAI.name, new Vector3(0,-100,0),Quaternion.identity,0,null);
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
