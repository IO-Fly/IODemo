using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTranform : MonoBehaviour {

    //同步的位置及旋转
    public int ID;
    public Vector3 syncPosition;
    public Quaternion syncRotation;

    //private 

    // Use this for initialization
    void Start () {

        //初始位置及旋转
        syncRotation = transform.rotation;
        syncPosition = transform.position;

    }

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log("Time-deltaTime:" + Time.deltaTime);
        if (!PhotonNetwork.isMasterClient)
        {
            //根据距离计算插值速率
            float lerpRate = 5;
            float distance = Vector3.Distance(this.transform.position, syncPosition);        
            if(distance > 0.4)
            {
                lerpRate += (distance - 0.4f) * 10;
            }

            this.transform.position = Vector3.Lerp(transform.position, syncPosition,lerpRate * Time.deltaTime);
            this.transform.rotation = syncRotation;
            //this.transform.position = syncPosition;
            //this.transform.rotation = Quaternion.Slerp(transform.rotation, syncRotation, 5 * Time.deltaTime);
        }

    }
   
}
