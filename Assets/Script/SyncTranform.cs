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
            float lerpRate = 15;
            float distance = Vector3.Distance(this.transform.position, syncPosition);
            Debug.LogWarning("Distance: " + distance);
           
            if(distance > 0.5)
            {
                lerpRate += (distance - 0.5f) * 10; ;
            }

            Vector3 newPosition;
            //插值
            if (distance < 0.2)
            {
                newPosition = syncPosition;     
                this.transform.rotation = syncRotation;
            }
            else
            {
                newPosition = Vector3.Lerp(transform.position, syncPosition, 25 * Time.deltaTime);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, syncRotation, 25 * Time.deltaTime);
            }
           
            this.transform.position = newPosition;
            //this.transform.LookAt(newPosition);
            //this.transform.rotation = syncRotation;

        }

    }
   
}
