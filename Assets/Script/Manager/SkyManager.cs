using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class SkyManager : Photon.PunBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other){
		Debug.Log("离开海面");
		if(other.gameObject.tag == "player"&&other.gameObject.transform.position.y>-1){
			other.gameObject.GetComponent<PlayerController>().fly=true;
			other.gameObject.GetComponent<PlayerController>().height = 50f/other.gameObject.transform.localScale.x	;
		}

	}
	void OnTriggerExit(Collider other){
		Debug.Log("进入海底");
		if(other.gameObject.tag == "player"&&other.gameObject.transform.position.y<=0){
			other.gameObject.GetComponent<PlayerController>().fly = false;
			other.gameObject.GetComponent<PlayerController>().drop = false;
		}
	}
}
