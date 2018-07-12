using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : Photon.PunBehaviour{

    //食物事件类型
    public enum Event {
        SYNC_INIT_FOOD,
        SYNC_FOOD,
        RESET_FOOD,
        SYNC_INIT_FOODAI,
        SYNC_FOODAI,
        RESET_FOODAI,
        SYNC_INIT_POISON,
        SYNC_POISON,
        RESET_POISION,
        INIT_PLAYER_FOOD,
        RESET_PLAYER_FOOD
    };

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

    //玩家死亡生成的食物
    public GameObject[][] playersFood;

    public bool isMasterBefore = false;
    public static FoodManager localFoodManager = null;

    //边界
    const int boundary = 185;
    const int treeBoundary = 40;

    void Awake(){

        //缓存全局的本地食物管理器
        localFoodManager = this;
        Debug.Log(this==null);
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

        //所有玩家死亡时生成的食物
        playersFood = new GameObject[PhotonNetwork.room.MaxPlayers][];

        //主客户端初始化食物
        InitFoodInMaster();

        //等待同步食物到其他客户端
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(WaitForInitFood());
        }


        //Debug
        Debug.LogWarning("isMaster: " + PhotonNetwork.isMasterClient);
        Debug.LogWarning("playerID: " + PhotonNetwork.player.ID);
        

    }
    
    void OnDestroy(){
        PhotonNetwork.OnEventCall -= this.OnEventRaised;
        StopAllCoroutines();
    }


    private void OnEventRaised(byte evCode, object content, int senderid){
		PhotonPlayer sender = PhotonPlayer.Find(senderid);
		switch(evCode){

            #region Sync Init

            //其他客户端根据主客户端生成食物
            case (byte)Event.SYNC_INIT_FOOD:
			Debug.Log("其他客户端更新食物");
			if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
                
				for(int i=0;i<FoodCount;i++){
					GameObject instance = (GameObject)Instantiate(foodPrefab, new Vector3(((float[])content)[i*7+0],((float[])content)[i*7+1],((float[])content)[i*7+2]), GetInitRotation());
					instance.GetComponent<FoodOverrideController>().translation = new Vector3(((float[])content)[i*7+3],((float[])content)[i*7+4],((float[])content)[i*7+5]);
					instance.GetComponent<FoodOverrideController>().ID = (int)((float[])content)[i*7+6];
					foodInstances[i] = instance;
				}
			}
			break;

            //其他客户端根据主客户端生成食物AI
            case (byte)Event.SYNC_INIT_FOODAI:
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
            
            //其他客户端根据主客户端生成毒物AI-poison
            case (byte)Event.SYNC_INIT_POISON:       
            if (!PhotonNetwork.isMasterClient && sender.IsMasterClient){      
                      
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
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

            //玩家死亡时所有客户端生成食物
            case (byte)Event.INIT_PLAYER_FOOD:
            {
                
                Debug.LogWarning("Player " + senderid + " die");

                FoodSyncInfo[] foodInfo = FoodSyncInfo.Deserialize((float[])content);
                GameObject[] foodList = new GameObject[foodInfo.Length];
                for(int i = 0; i < foodInfo.Length; i++) {
                    foodList[i] = Instantiate(foodPrefab, foodInfo[i].position, GetInitRotation());
                    foodList[i].GetComponent<FoodOverrideController>().translation = foodInfo[i].translation;
					foodList[i].GetComponent<FoodOverrideController>().ID = foodInfo[i].ID; 
                    foodList[i].GetComponent<FoodOverrideController>().playerID = senderid; 
                    foodList[i].tag = "playerFood";                    
                }
                playersFood[senderid - 1] = foodList;
                
            }
            break;
            #endregion

            #region Sync
 
            //处理发起更新食物命令
            case (byte)Event.SYNC_FOOD:
			if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient){
				Debug.Log("其他客户端刷新食物位置");
				for(int i=0;i<FoodCount;i++){
					if(foodInstances[i]==null)
					return;
					this.foodInstances[i].transform.position = ((Vector3[])content)[i];
				}
			}
			break;

            //主客户端发起同步食物AI位置及旋转
            case (byte)Event.SYNC_FOODAI:
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

            //主客户端发起同步毒物AI位置及旋转-poison
            case (byte)Event.SYNC_POISON:
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

            #endregion

            #region Reset

            //主客户端发起重置食物位置事件
            case (byte)Event.RESET_FOOD:
			/*if(!PhotonNetwork.isMasterClient&&sender.IsMasterClient)if(this!=null)*/{

				if(foodFlushLock[(int)((Vector3[])content)[2].x]!=0)
				return;
                Debug.Log("事件中的FoodManager "+(this==null));
				this.foodFlushLock[(int)((Vector3[])content)[2].x] = 1;
				StartCoroutine(SetLock((int)((Vector3[])content)[2].x));
                //未实例化时延迟同步
                int foodID = (int)((Vector3[])content)[2].x;
                Vector3 pos = ((Vector3[])content)[0];
                Vector3 translation = ((Vector3[])content)[1];
                if (this.foodInstances[foodID] == null)
                {
                    StartCoroutine(WaitForSyncFood(foodID, pos, translation));
                }
                else
                { 
                    this.foodInstances[foodID].transform.position = pos;
                    this.foodInstances[foodID].GetComponent<FoodOverrideController>().translation = translation;
                }
			}
			break;
                    
            //主客户端重置食物AI事件
            case (byte)Event.RESET_FOODAI:
            /*if(!PhotonNetwork.isMasterClient && sender.IsMasterClient)*/{
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                this.foodAIInstances[foodAIInfoObject[0].ID].transform.position = foodAIInfoObject[0].position;
                this.foodAIInstances[foodAIInfoObject[0].ID].transform.rotation = foodAIInfoObject[0].rotation;
                //重新激活
                this.foodAIInstances[foodAIInfoObject[0].ID].SetActive(true);
            }
            break;
                
            //主客户端重置毒物AI事件-poison
            case (byte)Event.RESET_POISION:
            /*if(!PhotonNetwork.isMasterClient && sender.IsMasterClient)*/ {
                float[] foodAIInfo = (float[])content;
                FoodAISyncInfo[] foodAIInfoObject = FoodAISyncInfo.Deserialize(foodAIInfo);
                this.poisonInstances[foodAIInfoObject[0].ID].transform.position = foodAIInfoObject[0].position;
                this.poisonInstances[foodAIInfoObject[0].ID].transform.rotation = foodAIInfoObject[0].rotation;

                //重新激活
                this.poisonInstances[foodAIInfoObject[0].ID].SetActive(true);
            }
            break;

            //由玩家生成的食物被吃时不再生成
            case (byte)Event.RESET_PLAYER_FOOD:
            {
                int playerID = ((int[])content)[0];
                int foodID = ((int[])content)[1];
                if(playersFood[playerID - 1][foodID] != null) {
                    Destroy(playersFood[playerID - 1][foodID]);
                }
                
            }
            break;   

            #endregion

        }

    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer){
    	if(PhotonNetwork.isMasterClient){
            //StartCoroutine(WaitForInitFood(newPlayer));
            //Debug.Log("主客户端发请求给加入的客户端更新食物");
            ////GameObject[] foodList = GameObject.FindGameObjectsWithTag("food");
            //GameObject[] foodList = foodInstances;
            //float[] foodInfomation = new float[7 * foodList.Length];
            //for (int i = 0; i < foodList.Length; i++)
            //{
            //    foodInfomation[i * 7 + 0] = foodList[i].transform.position.x;
            //    foodInfomation[i * 7 + 1] = foodList[i].transform.position.y;
            //    foodInfomation[i * 7 + 2] = foodList[i].transform.position.z;
            //    foodInfomation[i * 7 + 3] = foodList[i].GetComponent<FoodOverrideController>().translation.x;
            //    foodInfomation[i * 7 + 4] = foodList[i].GetComponent<FoodOverrideController>().translation.y;
            //    foodInfomation[i * 7 + 5] = foodList[i].GetComponent<FoodOverrideController>().translation.z;
            //    foodInfomation[i * 7 + 6] = foodList[i].GetComponent<FoodOverrideController>().ID;
            //}
            //RaiseEventOptions options = new RaiseEventOptions();
            //options.TargetActors = new int[] { newPlayer.ID };
            //PhotonNetwork.RaiseEvent(2, foodInfomation, true, options);


            ////主客户端发请求给加入的客户端更新食物AI       
            //float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);
            //PhotonNetwork.RaiseEvent(5, foodAIInfo, true, options);

            ////主客户端发请求给加入的客户端更新毒物AI
            //foodAIInfo = FoodAISyncInfo.Serialize(poisonInstances);
            //PhotonNetwork.RaiseEvent(8, foodAIInfo, true, options);
        }
    }

    public void InitFoodInMaster()
    {
        if ((foodInstances[0] == null || foodAIInstances[0] == null || 
            poisonInstances[0] == null ) && PhotonNetwork.isMasterClient)         
        {
            for (int i = 0; i < FoodCount; i++)
            {
                GameObject instance = (GameObject)Instantiate(foodPrefab, GetInitPosition(), GetInitRotation());
                instance.GetComponent<FoodOverrideController>().ID = i;
                foodInstances[i] = instance;
            }

            //生成主客户端的食物AI
            for (int i = 0; i < FoodAICount; i++)
            {
                GameObject AIInstance = Instantiate(foodAIPrefab, GetInitPosition(), GetInitRotation());
                AIInstance.GetComponent<SyncTranform>().ID = i;
                foodAIInstances[i] = AIInstance;
            }

            //生成主客户端的毒物AI
            poisonCountAll = 0;
            for (int i = 0; i < poisonCounts.Length; i++)
            {
                for (int j = 0; j < poisonCounts[i]; j++)
                {
                    GameObject AIInstance = Instantiate(poisonPrefabs[i], GetInitPosition(), GetInitRotation());
                    AIInstance.GetComponent<SyncTranform>().ID = poisonCountAll;
                    poisonInstances[poisonCountAll] = AIInstance;
                    poisonCountAll++;
                }
            }

            StartCoroutine(SyncFoodAITranform());//定时同步食物AI(包含毒物）
            isMasterBefore = true;
        }
    }

    public void InitPlayerFood(Player player)
    {

        int foodCount = player.CalculateStarNum();
        float radius = player.CalculateStarRange();
        int playerID = PhotonNetwork.player.ID;

        GameObject[] foodList = new GameObject[foodCount];
        for (int i = 0; i < foodCount; i++)
        {
            foodList[i] = Instantiate(foodPrefab, player.transform.position + GetInitSpherePos(radius), GetInitRotation());
            foodList[i].GetComponent<FoodOverrideController>().ID = i;
            foodList[i].GetComponent<FoodOverrideController>().playerID = PhotonNetwork.player.ID;
            foodList[i].tag = "playerFood";
        }
        playersFood[playerID - 1] = foodList;

        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.Others;
        options.CachingOption = EventCaching.DoNotCache;

        float[] foodInfo = FoodSyncInfo.Serialize(foodList);
        PhotonNetwork.RaiseEvent((byte)Event.INIT_PLAYER_FOOD, foodInfo, true, options);

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
            PhotonNetwork.RaiseEvent((byte)Event.SYNC_FOOD, foodsPosition, true, options);
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
                PhotonNetwork.RaiseEvent((byte)Event.SYNC_FOODAI, foodAIInfo, true, options);
            }

            if (poisonCountAll > 0 && poisonInstances[0] != null)
            {
                float[] foodAIInfo = FoodAISyncInfo.Serialize(poisonInstances);
                PhotonNetwork.RaiseEvent((byte)Event.SYNC_POISON, foodAIInfo, true, options);
            }

        }
    }
    IEnumerator SetLock(int id)
    {
        yield return new WaitForSeconds(1);
        this.foodFlushLock[id] = 0;
    }

    IEnumerator WaitForInitFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            if(foodInstances[foodInstances.Length - 1] != null &&
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
                options.Receivers = ReceiverGroup.Others;
                PhotonNetwork.RaiseEvent((byte)Event.SYNC_INIT_FOOD, foodInfomation, true, options);

                //主客户端发请求给加入的客户端更新食物AI       
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);
                PhotonNetwork.RaiseEvent((byte)Event.SYNC_INIT_FOODAI, foodAIInfo, true, options);

                //主客户端发请求给加入的客户端更新毒物AI
                foodAIInfo = FoodAISyncInfo.Serialize(poisonInstances);
                PhotonNetwork.RaiseEvent((byte)Event.SYNC_INIT_POISON, foodAIInfo, true, options);

                break;
            }

        }
    }

    IEnumerator WaitForSyncFood(int foodID, Vector3 pos, Vector3 translation)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if(foodInstances[foodID] != null)
            {
                foodInstances[foodID].transform.position = pos;
                foodInstances[foodID].GetComponent<FoodOverrideController>().translation = translation;
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
            initPos.y = Random.Range(-boundary + 20, -5);
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

    public static Vector3 GetInitSpherePos(float radius)
    {
        Vector3 initPos = new Vector3();
        while (true)
        {
            initPos.x = Random.Range(-radius, radius);
            initPos.y = Random.Range(-radius, radius);
            initPos.z = Random.Range(-radius, radius);

            if(initPos.magnitude <= radius)
            {
                break;
            }

        }
        return initPos;
    }

    #endregion

}
