using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : Photon.PunBehaviour {
	public GameObject foodPrefab;
	private Vector3 translation;
	public float rotationSpeed=2f;
	public float translationSpeed=0.05f;
	// Use this for initialization
	void Start () {
		translation = (this.transform.position - new Vector3(0,this.transform.position.y+Random.Range(-1f,1f),0))*translationSpeed;
	}
	
	// Update is called once per frame
	void Update () {
	this.gameObject.transform.Rotate(rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime);
	this.gameObject.transform.Translate(translation*Time.deltaTime,Space.World);
	if(this.gameObject.transform.position.y<-95||this.gameObject.transform.position.y>-5||this.gameObject.transform.position.x>100||this.gameObject.transform.position.x<-100||this.gameObject.transform.position.z<-100||this.gameObject.transform.position.z>100){
		PhotonNetwork.Destroy(this.gameObject);
		PhotonNetwork.Instantiate("food",new Vector3(Random.Range(-20,20),Random.Range(-95,-5),Random.Range(-20,20)), Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);

		}
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
