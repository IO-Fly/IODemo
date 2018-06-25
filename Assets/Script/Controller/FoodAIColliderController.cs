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
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("食物AI：碰撞");
        if (other.gameObject.tag == "player" || other.gameObject.tag == "playerCopy")
        {    
            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("食物AI：删除");
                //重置主客户端食物AI位置
                this.gameObject.GetComponentInParent<FoodAI>().transform.position = GetRandomVector3();
                this.gameObject.GetComponentInParent<FoodAI>().transform.rotation = GetRandomQuaternion();

                //序列化数据
                GameObject[] foodAIInstances = new GameObject[1];
                foodAIInstances[0] = this.gameObject.GetComponentInParent<FoodAI>().transform.gameObject;
                float[] foodAIInfo = FoodAISyncInfo.Serialize(foodAIInstances);

                //发送事件
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.Others;
                options.CachingOption = EventCaching.DoNotCache;
                PhotonNetwork.RaiseEvent(7, foodAIInfo, true, options);   

            }
        }
    }

    private Vector3 GetRandomVector3()
    {
        return new Vector3(Random.Range(-20, 20), Random.Range(-95, -5), Random.Range(-20, 20));
    }

    private Quaternion GetRandomQuaternion()
    {
        return Quaternion.Euler(Random.Range(0, 180), Random.Range(0, 180), Random.Range(0, 180));
    }

}
