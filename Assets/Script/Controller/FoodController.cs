using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnTriggerEnter(Collider other){
		Debug.Log ("食物：碰撞，将要删除");
		if (other.gameObject.tag == "player") {
			Debug.Log ("食物：删除");
			PhotonView.Destroy (this.gameObject);
		}
	}

}
