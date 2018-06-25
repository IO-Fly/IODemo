using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : Photon.PunBehaviour {

	private Vector3 PositionTemp;
	private Vector3 RotationTemp;
	public int FoodCount;
	public GameObject foodPrefab;
	public GameObject[] foodInstances;

    //食物AI
    public int FoodAICount;
	private int[] foodFlushLock;
    public GameObject foodAIPrefab;
    public GameObject[] foodAIInstances;


	void Awake(){
		foodFlushLock = new int[FoodCount];
		PhotonNetwork.OnEventCall += this.OnEventRaised;
		foodInstances = new GameObject[FoodCount];
        //食物AI列表
        foodAIInstances = new GameObject[FoodAICount];


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

                //生成主客户端的食物AI
                for(int i = 0; i < FoodAICount; i++){
                    GameObject AIInstance = Instantiate(foodAIPrefab, new Vector3(Random.Range(-90, 90), Random.Range(-95, -5), Random.Range(-90, 90)), Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180)));
                    AIInstance.GetComponent<SyncTranform>().ID = i;
                    foodAIInstances[i] = AIInstance;
                }
                StartCoroutine(SyncFoodAITranform());//定时同步食物AI

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
			//if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
				if(foodFlushLock[(int)((Vector3[])content)[2].x]!=0)
				return;
				this.foodFlushLock[(int)((Vector3[])content)[2].x] = 1;
				StartCoroutine(SetLock((int)((Vector3[])content)[2].x));
				this.foodInstances[(int)((Vector3[])content)[2].x].transform.position = ((Vector3[])content)[0];
				this.foodInstances[(int)((Vector3[])content)[2].x].GetComponent<FoodOverrideController>().translation= ((Vector3[])content)[1];
			//}
			break;
            //其他客户端根据主客户端生成食物AI
            case 5:
            Debug.Log("其他客户端创建食物AI");
            if (!PhotonNetwork.isMasterClient && sender.IsMasterClient){
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                for(int i = 0; i < FoodAICount; i++) {
                    Vector3 pos = foodAIInfoObject[i].position;
                    Quaternion rotation = foodAIInfoObject[i].rotation;
                    GameObject AIInstance = Instantiate(foodAIPrefab,pos,rotation);
                    AIInstance.GetComponent<SyncTranform>().ID = i;
                    foodAIInstances[i] = AIInstance;           
                }
            }
            break;
            //主客户端发起同步食物AI位置及旋转
            case 6:
            if(!PhotonNetwork.isMasterClient && sender.IsMasterClient){
                
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                for(int i = 0; i < FoodAICount; i++) {
                    this.foodAIInstances[i].GetComponent<SyncTranform>().syncPosition = foodAIInfoObject[i].position;
                    this.foodAIInstances[i].GetComponent<SyncTranform>().syncRotation = foodAIInfoObject[i].rotation;
                }
            }
            break;
            //主客户端重置食物AI事件
            case 7:
            /*if(!PhotonNetwork.isMasterClient && sender.IsMasterClient)*/{
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                this.foodAIInstances[foodAIInfoObject[0].ID].transform.position = foodAIInfoObject[0].position;
                this.foodAIInstances[foodAIInfoObject[0].ID].transform.rotation = foodAIInfoObject[0].rotation;
                //重新激活
                this.foodAIInstances[foodAIInfoObject[0].ID].SetActive(true);
            }
            break;
            
		}

	}

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
    	if(PhotonNetwork.isMasterClient){
    		Debug.Log("主客户端发请求给加入的客户端更新食物");
            //GameObject[] foodList = GameObject.FindGameObjectsWithTag("food");
            GameObject[] foodList = foodInstances;
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


            //主客户端发请求给加入的客户端更新食物AI       
            float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);
            PhotonNetwork.RaiseEvent(5, foodAIInfo, true, options);


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


    //定时同步食物AI位置及旋转
    IEnumerator SyncFoodAITranform()
    {
        while (true)
        {
            Debug.Log("主客户端周期发起更新食物AI");
            //主客户端定时更新其他客户端的食物AI位置及旋转
            if (FoodAICount > 0 && foodAIInstances[0] != null)
            {
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.Others;
                options.CachingOption = EventCaching.DoNotCache;
                PhotonNetwork.RaiseEvent(6, foodAIInfo, true, options);
            }  
            yield return new WaitForSeconds(0.016f);
        }
    }

	IEnumerator SetLock(int id){
		yield return new WaitForSeconds(1);
		this.foodFlushLock[id] = 0;	
	}

	
}
