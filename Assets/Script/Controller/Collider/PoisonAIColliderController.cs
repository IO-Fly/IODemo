using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAIColliderController : FoodAIColliderController {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void OnTriggerStay(Collider other){
		if(other.gameObject.tag == "player" || other.gameObject.tag == "playerCopy"){

            if (other.gameObject.GetComponent<Player>().photonView.isMine)
            {
                Debug.Log("毒物AI：删除");

                //重置主客户端食物AI位置
                GameObject parent = this.gameObject.GetComponentInParent<SyncTranform>().gameObject;

                //隐藏父物体
                parent.SetActive(false);

                //随机生成transform
                parent.transform.position = FoodManager.GetInitPosition();
                parent.transform.rotation = FoodManager.GetInitRotation();

                //序列化数据
                GameObject[] foodAIInstances = new GameObject[1];
                foodAIInstances[0] = parent;
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);

                //发送事件
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                options.CachingOption = EventCaching.DoNotCache;
                PhotonNetwork.RaiseEvent((byte)FoodManager.Event.RESET_POISION, foodAIInfo, true, options);

                //触发玩家吃到毒物事件
                other.gameObject.GetComponent<Player>().photonView.RPC("EatPoison", PhotonTargets.AllViaServer);
            }
           
        }
	}


}
