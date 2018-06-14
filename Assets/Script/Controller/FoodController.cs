using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : Photon.PunBehaviour {
	public GameObject foodPrefab;
	public float rotationSpeed=2f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	this.gameObject.transform.Rotate(rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime);	
	}
	private void OnTriggerEnter(Collider other){
		Debug.Log ("食物：碰撞，将要删除");
		if (other.gameObject.tag == "player") {
			Debug.Log ("食物：删除");
			PhotonNetwork.Destroy (this.gameObject);
			PhotonNetwork.Instantiate("food", new Vector3(Random.Range(-95,95), Random.Range(-95,-5), Random.Range(-95,95)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
		}
	}

}
