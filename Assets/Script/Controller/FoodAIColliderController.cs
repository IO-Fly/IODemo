using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAIColliderController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("食物AI：碰撞");
        if (other.gameObject.tag == "player" || other.gameObject.tag == "playerCopy")
        {    
            if (other.gameObject.GetComponent<Player>().photonView.isMine)
            {
                Debug.Log("食物AI：删除");

                //重置主客户端食物AI位置
                GameObject parent = this.gameObject.GetComponentInParent<SyncTranform>().gameObject;

                //隐藏父物体
                parent.SetActive(false);

                //随机生成transform
                parent.transform.position = GetRandomVector3();
                parent.transform.rotation = GetRandomQuaternion();

                //序列化数据
                GameObject[] foodAIInstances = new GameObject[1];
                foodAIInstances[0] = parent;
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);

                //发送事件
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                options.CachingOption = EventCaching.DoNotCache;
                PhotonNetwork.RaiseEvent(7, foodAIInfo, true, options);

                //触发玩家吃到食物事件
                other.gameObject.GetComponent<Player>().photonView.RPC("EatFood", PhotonTargets.AllViaServer);

            }
        }
    }

    public Vector3 GetRandomVector3()
    {
        return new Vector3(Random.Range(-20, 20), Random.Range(-95, -5), Random.Range(-20, 20));
    }

    public Quaternion GetRandomQuaternion()
    {
        return Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
    }

}
