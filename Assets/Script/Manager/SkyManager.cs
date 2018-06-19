﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class SkyManager : Photon.PunBehaviour {


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other){
        Debug.Log("离开海面");
        if (other.gameObject.tag == "player"){
            if (other.gameObject.GetComponent<PlayerController>().CanFly())
            {  
                other.gameObject.GetComponent<PlayerController>().StartFly();
                Debug.Log("飞向天空");
            }else
            {
                other.gameObject.GetComponent<PlayerController>().WaitForFly();
            }
		}
     
	}
	void OnTriggerExit(Collider other){
		Debug.Log("进入海底");
		if(other.gameObject.tag == "player"){
            other.gameObject.GetComponent<PlayerController>().EndFly();
        }
       
	}
}
