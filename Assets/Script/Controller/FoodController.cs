using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : Photon.PunBehaviour {
	public GameObject foodPrefab;
	private Vector3 translation;
	public float rotationSpeed=2f;
	public float translationSpeed=0.5f;
	// Use this for initialization
	
	void Start () {
		translation = (this.transform.position - new Vector3(0,this.transform.position.y+Random.Range(-10f,10f),0)).normalized * translationSpeed;
		}
	
	// Update is called once per frame
	void Update () {
	this.gameObject.transform.Rotate(rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime);
	this.gameObject.transform.Translate(translation*Time.deltaTime,Space.World);
	if(this.gameObject.transform.position.y<-95||this.gameObject.transform.position.y>-5||this.gameObject.transform.position.x>100||this.gameObject.transform.position.x<-100||this.gameObject.transform.position.z<-100||this.gameObject.transform.position.z>100){
		if(photonView.isMine&&PhotonNetwork.isMasterClient){
		//PhotonNetwork.Destroy(this.gameObject);
		//PhotonNetwork.InstantiateSceneObject("food",new Vector3(Random.Range(-20,20),Random.Range(-95,-5),Random.Range(-20,20)), Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
		this.photonView.RPC("AddFood",PhotonTargets.MasterClient);
		this.photonView.RPC("DestroyFood",PhotonTargets.MasterClient);
		}
		}
	}
	private void OnTriggerEnter(Collider other){
		Debug.Log ("食物：碰撞，将要删除");
		if (other.gameObject.tag == "player") {
			Debug.Log ("食物：删除");
			//PhotonNetwork.Destroy (this.gameObject);
			//PhotonNetwork.InstantiateSceneObject("food", new Vector3(Random.Range(-95,95), Random.Range(-95,-5), Random.Range(-95,95)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
			this.photonView.RPC("AddFood",PhotonTargets.MasterClient);
			this.photonView.RPC("DestroyFood",PhotonTargets.MasterClient);
		}
	}
	[PunRPC]
	void DestroyFood(){
		if(photonView.isMine&&PhotonNetwork.isMasterClient)
		PhotonNetwork.Destroy(this.gameObject);
		Debug.Log("删除食物");
	}
	
	[PunRPC]
	void AddFood(){
		if(PhotonNetwork.isMasterClient)
		PhotonNetwork.InstantiateSceneObject("food",new Vector3(Random.Range(-20,20),Random.Range(-95,-5),Random.Range(-20,20)), Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)),0,null);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
		if(stream.isWriting){
			stream.SendNext(this.gameObject.transform.position);
		}
		else{
			this.gameObject.transform.position = (Vector3)stream.ReceiveNext();
		}
	}

}
