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
	private void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "player" || other.gameObject.tag == "playerCopy"){

            if (other.gameObject.GetComponent<Player>().photonView.isMine)
            {
                Debug.Log("毒物AI：删除");
                //重置主客户端食物AI位置

                this.gameObject.SetActive(false);

                this.gameObject.GetComponentInParent<PoisonAI>().transform.position = GetRandomVector3();
                this.gameObject.GetComponentInParent<PoisonAI>().transform.rotation = GetRandomQuaternion();

                //序列化数据
                GameObject[] foodAIInstances = new GameObject[1];
                foodAIInstances[0] = this.gameObject.GetComponentInParent<PoisonAI>().transform.gameObject;
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);

                //发送事件
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                options.CachingOption = EventCaching.DoNotCache;
                PhotonNetwork.RaiseEvent(10, foodAIInfo, true, options);

                //触发玩家吃到毒物事件
                other.gameObject.GetComponent<Player>().photonView.RPC("EatPoison", PhotonTargets.AllViaServer);
            }
           
        }
	}


}
