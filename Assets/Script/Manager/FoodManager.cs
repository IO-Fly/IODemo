using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : Photon.PunBehaviour {
	private Vector3 PositionTemp;
	private Vector3 RotationTemp;
	public int FoodCount;
	public GameObject foodPrefab;
	public GameObject[] foodInstances;
	void Awake(){
		PhotonNetwork.OnEventCall += this.OnEventRaised;
		foodInstances = new GameObject[FoodCount];
		RaiseEventOptions options = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.All;
		options.CachingOption = EventCaching.DoNotCache;
		PhotonNetwork.RaiseEvent(1,null,true, options);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnEventRaised(byte evCode, object content, int senderid){
		PhotonPlayer sender = PhotonPlayer.Find(senderid);
		switch(evCode){
			//主客户端生成食物
			case 1:
			Debug.Log("主客户端生成食物");
			Debug.Log("是否是主客户端: " +sender.IsMasterClient);//主客户端生成食物
			if(PhotonNetwork.isMasterClient&&sender.IsMasterClient){
				for(int i=0;i<FoodCount;i++){
					GameObject instance = (GameObject)Instantiate(foodPrefab, new Vector3(Random.Range(-90,90), Random.Range(-95,-5), Random.Range(-90,90)),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)));
					instance.GetComponent<FoodOverrideController>().ID = i;
					foodInstances[i] = instance;
				}
				if(PhotonNetwork.isMasterClient){
					//StartCoroutine(SyncPosition());
		}
			}

		break;
			//其他客户端根据主客户端生成食物
			case 2:
			Debug.Log("其他客户端更新食物");
			if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
				for(int i=0;i<FoodCount;i++){
					GameObject instance = (GameObject)Instantiate(foodPrefab, new Vector3(((float[])content)[i*7+0],((float[])content)[i*7+1],((float[])content)[i*7+2]),Quaternion.Euler(Random.Range(0,180),Random.Range(0,180),Random.Range(0,180)));
					instance.GetComponent<FoodOverrideController>().translation = new Vector3(((float[])content)[i*7+3],((float[])content)[i*7+4],((float[])content)[i*7+5]);
					instance.GetComponent<FoodOverrideController>().ID = (int)((float[])content)[i*7+6];
					foodInstances[i] = instance;
				}
	
			}
			break;
			//处理发起更新命令
			case 3:
			if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
				Debug.Log("其他客户端刷新食物位置");
				for(int i=0;i<FoodCount;i++){
					if(foodInstances[i]==null)
					return;
					this.foodInstances[i].transform.position = ((Vector3[])content)[i];
				}
			}
			break;
			//主客户端发起重置食物位置事件
			case 4:
			if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
				this.foodInstances[(int)((Vector3[])content)[2].x].transform.position = ((Vector3[])content)[0];
				this.foodInstances[(int)((Vector3[])content)[2].x].GetComponent<FoodOverrideController>().translation= ((Vector3[])content)[1];
			}
			break;
		}

	}

public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
	if(PhotonNetwork.isMasterClient){
		Debug.Log("主客户端发请求给加入的客户端更新食物");
		GameObject[] foodList = GameObject.FindGameObjectsWithTag("food");
		float[] foodInfomation = new float[7*foodList.Length];
		for(int i=0;i<foodList.Length;i++){
			foodInfomation[i*7+0] = foodList[i].transform.position.x;
			foodInfomation[i*7+1] = foodList[i].transform.position.y;
			foodInfomation[i*7+2] = foodList[i].transform.position.z;
			foodInfomation[i*7+3] = foodList[i].GetComponent<FoodOverrideController>().translation.x;
			foodInfomation[i*7+4] = foodList[i].GetComponent<FoodOverrideController>().translation.y;
			foodInfomation[i*7+5] = foodList[i].GetComponent<FoodOverrideController>().translation.z;
			foodInfomation[i*7+6] = foodList[i].GetComponent<FoodOverrideController>().ID;
		}
		RaiseEventOptions options = new RaiseEventOptions();
		options.TargetActors = new int[]{newPlayer.ID};
		PhotonNetwork.RaiseEvent(2,foodInfomation,true,options);


	}
}

IEnumerator SyncPosition(){
	while(true){
		//主客户端发起更新
		Debug.Log("主客户端发起更新");
		Vector3[] foodsPosition = new Vector3[FoodCount];
		for(int i=0;i<FoodCount;i++){
			Debug.Log(foodInstances[i].transform.position.x);
			foodsPosition[i].x = this.foodInstances[i].transform.position.x;
			foodsPosition[i].y = this.foodInstances[i].transform.position.y;
			foodsPosition[i].z = this.foodInstances[i].transform.position.z;
		}
		RaiseEventOptions options  = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.Others;
		options.CachingOption = EventCaching.DoNotCache;
		PhotonNetwork.RaiseEvent(3, foodsPosition,true, options);
	yield return new WaitForSeconds(0.5f);
	}
}

void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer){
	if(PhotonNetwork.isMasterClient){
		//StartCoroutine(SyncPosition());
	}
}

	
}
