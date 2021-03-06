﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOverrideController : MonoBehaviour {
	public int ID;
	// Use this for initialization
	public GameObject foodPrefab;
	public Vector3 translation;
	public float rotationSpeed=2f;
	public float translationSpeed=0.5f;

    //记录哪个玩家生成的
    public int playerID;
	
	void Start () {
        //translation = (this.transform.position - new Vector3(0,this.transform.position.y+Random.Range(-10f,10f),0)).normalized * translationSpeed;
        //translation = FoodManager.GetInitSpherePos(1.0f).normalized * translationSpeed;
	}
	
	// Update is called once per frame
	void Update () {

	    this.gameObject.transform.Rotate(rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime,rotationSpeed*Time.deltaTime);
	    this.gameObject.transform.Translate(translation*Time.deltaTime,Space.World);
    
        if (!FoodManager.IsInBoundary(this.gameObject.transform.position) && PhotonNetwork.isMasterClient)
        {
            Vector3 tempPosition = FoodManager.GetInitPosition();
            Vector3 temptranslation = (this.transform.position - new Vector3(0, this.transform.position.y + Random.Range(-10f, 10f), 0)).normalized * translationSpeed;
            Vector3[] Data = { tempPosition, temptranslation, new Vector3(this.ID, 0, 0) };
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.All;
            options.CachingOption = EventCaching.DoNotCache;
            PhotonNetwork.RaiseEvent((byte)FoodManager.Event.RESET_FOOD, Data, true, options);

        }
	}
	private void OnTriggerEnter(Collider other){
		Debug.Log ("食物：碰撞，将要删除");
		if (other.gameObject.tag == "player" || other.gameObject.tag == "playerCopy") {
			Debug.Log ("食物：删除");
			if(other.GetComponent<Player>().photonView.isMine){


                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                options.CachingOption = EventCaching.DoNotCache;
                if (this.tag == "food")
                {
                    Vector3 tempPosition = FoodManager.GetInitPosition();
                    Vector3 temptranslation = (this.transform.position - new Vector3(0, this.transform.position.y + Random.Range(-10f, 10f), 0)).normalized * translationSpeed;
                    Vector3[] Data = { tempPosition, temptranslation, new Vector3(this.ID, 0, 0) };
                    PhotonNetwork.RaiseEvent((byte)FoodManager.Event.RESET_FOOD, Data, true, options);
                }
                else if(this.tag == "playerFood")
                {
                    Debug.Log("玩家食物：删除");
                    int[] Data = {this.ID};
                    PhotonNetwork.RaiseEvent((byte)FoodManager.Event.RESET_PLAYER_FOOD, Data, true, options);
                }
			   
			   
                //触发玩家吃到食物事件
                other.gameObject.GetComponent<Player>().photonView.RPC("EatFood", PhotonTargets.AllViaServer);
            }
        }
	}

}
