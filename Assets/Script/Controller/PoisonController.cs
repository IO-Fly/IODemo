using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "player"){
			PhotonNetwork.Destroy(this.gameObject);
			PhotonNetwork.InstantiateSceneObject("poisonFood", new Vector3(Random.Range(-95,95), Random.Range(-95,-5), Random.Range(-95,95)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
		}
	}
}
