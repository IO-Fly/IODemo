using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : Photon.PunBehaviour {

    //食物
	public int FoodCount;
	public GameObject foodPrefab;
	private GameObject[] foodInstances;
    private int[] foodFlushLock;

    //食物AI
    public int FoodAICount;
    public GameObject foodAIPrefab;
    private GameObject[] foodAIInstances;

    //毒物AI
    public int[] poisonCounts;
    public GameObject[] poisonPrefabs;
    private int poisonCountAll;
    private GameObject[] poisonInstances;

    public bool isMasterBefore = false;
    public static FoodManager localFoodManager = null;

    //边界
    const int boundary = 195;
    const int treeBoundary = 40;

    void Awake(){

        //缓存全局的本地食物管理器
        localFoodManager = this;

        PhotonNetwork.OnEventCall += this.OnEventRaised;

        foodFlushLock = new int[FoodCount];
		foodInstances = new GameObject[FoodCount];

        //食物AI列表
        foodAIInstances = new GameObject[FoodAICount];
        //毒物AI列表
        poisonCountAll = 0;
        for(int i = 0; i < poisonCounts.Length; i++) {
            for(int j = 0; j < poisonCounts[i]; j++) {
                poisonCountAll++;
            }
        }
        poisonInstances = new GameObject[poisonCountAll];


        RaiseEventOptions options = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.All;
		options.CachingOption = EventCaching.DoNotCache;
		PhotonNetwork.RaiseEvent(1,null,true, options);


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
					GameObject instance = (GameObject)Instantiate(foodPrefab, GetInitPosition(), GetInitRotation());
					instance.GetComponent<FoodOverrideController>().ID = i;
					foodInstances[i] = instance;
				}

                //生成主客户端的食物AI
                for(int i = 0; i < FoodAICount; i++){
                    GameObject AIInstance = Instantiate(foodAIPrefab, GetInitPosition(), GetInitRotation());
                    AIInstance.GetComponent<SyncTranform>().ID = i;
                    foodAIInstances[i] = AIInstance;
                }
        
                //生成主客户端的毒物AI
                poisonCountAll = 0;
                for(int i = 0; i < poisonCounts.Length; i++) { 
                    for(int j = 0; j < poisonCounts[i]; j++){                   
                        GameObject AIInstance = Instantiate(poisonPrefabs[i], GetInitPosition(), GetInitRotation());
                        AIInstance.GetComponent<SyncTranform>().ID = poisonCountAll;        
                        poisonInstances[poisonCountAll] = AIInstance;         
                        poisonCountAll ++;
                    }
                }

                StartCoroutine(SyncFoodAITranform());//定时同步食物AI(包含毒物）
			    isMasterBefore = true;

			}

		    break;
			//其他客户端根据主客户端生成食物
			case 2:
			Debug.Log("其他客户端更新食物");
			if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
                
                //同步主客户端场景
                PhotonNetwork.automaticallySyncScene = true;

				for(int i=0;i<FoodCount;i++){
					GameObject instance = (GameObject)Instantiate(foodPrefab, new Vector3(((float[])content)[i*7+0],((float[])content)[i*7+1],((float[])content)[i*7+2]), GetInitRotation());
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

                //同步主客户端场景
                PhotonNetwork.automaticallySyncScene = true;

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
                    //未实例化时不同步
                    if(this.foodAIInstances[i] == null){
                        continue;
                    }
                    SyncTranform syncTranform = this.foodAIInstances[i].GetComponent<SyncTranform>();
                    syncTranform.syncPosition = foodAIInfoObject[i].position;
                    syncTranform.syncRotation = foodAIInfoObject[i].rotation;           
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
            case 8:
            //其他客户端根据主客户端生成毒物AI-poison
            if (!PhotonNetwork.isMasterClient && sender.IsMasterClient){      
                      
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                //生成主客户端的毒物AI
                poisonCountAll = 0;
                for(int i = 0; i < poisonCounts.Length; i++) { 
                    for(int j = 0; j < poisonCounts[i]; j++){                   
                        Vector3 pos = foodAIInfoObject[poisonCountAll].position;
                        Quaternion rotation = foodAIInfoObject[poisonCountAll].rotation;
                        GameObject AIInstance = Instantiate(poisonPrefabs[i],pos,rotation);
                        AIInstance.GetComponent<SyncTranform>().ID = poisonCountAll;  
                        poisonInstances[poisonCountAll] = AIInstance;                     
                        poisonCountAll ++;
                    }
                }

            }
            break;
            //主客户端发起同步毒物AI位置及旋转-poison
            case 9:
            if (!PhotonNetwork.isMasterClient && sender.IsMasterClient){

                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                for (int i = 0; i < poisonCountAll; i++)
                {  
                    //未实例化时不同步
                    if(this.poisonInstances[i] == null){
                        continue;
                    }
                    SyncTranform syncTranform = this.poisonInstances[i].GetComponent<SyncTranform>();
                    syncTranform.syncPosition = foodAIInfoObject[i].position;
                    syncTranform.syncRotation = foodAIInfoObject[i].rotation;    
                }

            }
            break;
            //主客户端重置毒物AI事件-poison
            case 10:
            /*if(!PhotonNetwork.isMasterClient && sender.IsMasterClient)*/ {
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                this.poisonInstances[foodAIInfoObject[0].ID].transform.position = foodAIInfoObject[0].position;
                this.poisonInstances[foodAIInfoObject[0].ID].transform.rotation = foodAIInfoObject[0].rotation;

                //重新激活
                this.poisonInstances[foodAIInfoObject[0].ID].SetActive(true);
            }
            break;
        }

    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
    	if(PhotonNetwork.isMasterClient){
            StartCoroutine(WaitForInitFood(newPlayer));
        }
    }


    #region IEnumerator
    IEnumerator SyncPosition()
    {
        while (true)
        {
            //主客户端发起更新
            Debug.Log("主客户端发起更新");
            Vector3[] foodsPosition = new Vector3[FoodCount];
            for (int i = 0; i < FoodCount; i++)
            {
                Debug.Log(foodInstances[i].transform.position.x);
                foodsPosition[i].x = this.foodInstances[i].transform.position.x;
                foodsPosition[i].y = this.foodInstances[i].transform.position.y;
                foodsPosition[i].z = this.foodInstances[i].transform.position.z;
            }
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            options.CachingOption = EventCaching.DoNotCache;
            PhotonNetwork.RaiseEvent(3, foodsPosition, true, options);
            yield return new WaitForSeconds(0.5f);
        }
    }
    //定时同步食物AI位置及旋转
    public IEnumerator SyncFoodAITranform()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);

            Debug.Log("主客户端周期发起更新食物AI");
            RaiseEventOptions options = new RaiseEventOptions();
            options.Receivers = ReceiverGroup.Others;
            options.CachingOption = EventCaching.DoNotCache;
            //主客户端定时更新其他客户端的食物AI位置及旋转
            if (FoodAICount > 0 && foodAIInstances[0] != null)
            {
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);
                PhotonNetwork.RaiseEvent(6, foodAIInfo, true, options);
            }

            if (poisonCountAll > 0 && poisonInstances[0] != null)
            {
                float[] foodAIInfo = FoodAISyncInfo.Serialize(poisonInstances);
                PhotonNetwork.RaiseEvent(9, foodAIInfo, true, options);
            }

        }
    }
    IEnumerator SetLock(int id)
    {
        yield return new WaitForSeconds(1);
        this.foodFlushLock[id] = 0;
    }

    IEnumerator WaitForInitFood(PhotonPlayer newPlayer)
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            if(foodInstances[foodAIInstances.Length - 1] != null &&
                foodAIInstances[foodAIInstances.Length - 1] != null &&
                poisonInstances[poisonInstances.Length - 1] != null
                )
            {
                Debug.Log("主客户端发请求给加入的客户端更新食物");
                //GameObject[] foodList = GameObject.FindGameObjectsWithTag("food");
                GameObject[] foodList = foodInstances;
                float[] foodInfomation = new float[7 * foodList.Length];
                for (int i = 0; i < foodList.Length; i++)
                {
                    foodInfomation[i * 7 + 0] = foodList[i].transform.position.x;
                    foodInfomation[i * 7 + 1] = foodList[i].transform.position.y;
                    foodInfomation[i * 7 + 2] = foodList[i].transform.position.z;
                    foodInfomation[i * 7 + 3] = foodList[i].GetComponent<FoodOverrideController>().translation.x;
                    foodInfomation[i * 7 + 4] = foodList[i].GetComponent<FoodOverrideController>().translation.y;
                    foodInfomation[i * 7 + 5] = foodList[i].GetComponent<FoodOverrideController>().translation.z;
                    foodInfomation[i * 7 + 6] = foodList[i].GetComponent<FoodOverrideController>().ID;
                }
                RaiseEventOptions options = new RaiseEventOptions();
                options.TargetActors = new int[] { newPlayer.ID };
                PhotonNetwork.RaiseEvent(2, foodInfomation, true, options);


                //主客户端发请求给加入的客户端更新食物AI       
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);
                PhotonNetwork.RaiseEvent(5, foodAIInfo, true, options);

                //主客户端发请求给加入的客户端更新毒物AI
                foodAIInfo = FoodAISyncInfo.Serialize(poisonInstances);
                PhotonNetwork.RaiseEvent(8, foodAIInfo, true, options);

                break;
            }

        }
    }

    #endregion


    #region Utility
    public static Vector3 GetInitPosition()
    {

        Vector3 initPos = new Vector3();

        while (true)
        {
            initPos.x = Random.Range(-boundary, boundary);
            initPos.y = Random.Range(-boundary, -5);
            initPos.z = Random.Range(-boundary, boundary);

            if (IsInBoundary(initPos))
            {
                break;
            }
        }
        return initPos;

    }

    public static Quaternion GetInitRotation()
    {
        return Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
    }

    public static bool IsInBoundary(Vector3 pos)
    {
        if(pos.x < -boundary || pos.x > boundary ||
            pos.y < -boundary || pos.y > -5 ||
            pos.z <-boundary || pos.z > boundary
            )
        {
            return false;
        }
        else if(pos.x > -treeBoundary && pos.x < treeBoundary &&
            pos.z > -treeBoundary && pos.z < treeBoundary)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion

}
