using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOverrideController : MonoBehaviour {
	public int ID;
	// Use this for initialization
	public GameObject foodPrefab;
	public Vector3 translation;
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
		if(PhotonNetwork.isMasterClient){
			this.gameObject.transform.position = GetRandomVector3();
			this.translation = (this.transform.position - new Vector3(0,this.transform.position.y+Random.Range(-10f,10f),0)).normalized * translationSpeed;
			Vector3[] Data = {this.gameObject.transform.position,this.translation,new Vector3(this.ID,0,0)};
			RaiseEventOptions options = new RaiseEventOptions();
			options.Receivers = ReceiverGroup.Others;
			options.CachingOption = EventCaching.DoNotCache;
			PhotonNetwork.RaiseEvent(4,Data,true,options);
			}

		}
	}
	private void OnTriggerEnter(Collider other){
		Debug.Log ("食物：碰撞，将要删除");
		if (other.gameObject.tag == "player") {
			Debug.Log ("食物：删除");
			if(PhotonNetwork.isMasterClient){
			this.gameObject.transform.position = GetRandomVector3();
			this.translation = (this.transform.position - new Vector3(0,this.transform.position.y+Random.Range(-10f,10f),0)).normalized * translationSpeed;
			Vector3[] Data = {this.gameObject.transform.position,this.translation,new Vector3(this.ID,0,0)};
			RaiseEventOptions options = new RaiseEventOptions();
			options.Receivers = ReceiverGroup.Others;
			options.CachingOption = EventCaching.DoNotCache;
			PhotonNetwork.RaiseEvent(4,Data,true,options);
			}
		}
	}
	private  Vector3 GetRandomVector3(){
		return new Vector4(Random.Range(-20,20),Random.Range(-95,-5),Random.Range(-20,20));
	}

}
