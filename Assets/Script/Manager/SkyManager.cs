using System.Collections;
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
            other.gameObject.GetComponent<PlayerBehaviour>().EnterSky();
            if (other.gameObject.GetComponent<PlayerBehaviour>().CanFly())
            {
                other.gameObject.GetComponent<PlayerBehaviour>().StartFly();   
                Debug.Log("飞向天空");

                //粒子效果
                other.gameObject.GetComponent<SeaParticleController>().EnterSky();
            }
            else
            {
                ///other.gameObject.GetComponent<PlayerBehaviour>().WaitForFly();
            }
		}
     
	}
	void OnTriggerExit(Collider other){
		Debug.Log("进入海底");
        
        if (other.gameObject.tag == "player"){
            other.gameObject.GetComponent<PlayerBehaviour>().LeaveSky();
            other.gameObject.GetComponent<PlayerBehaviour>().EndFly();

            //粒子效果
            other.gameObject.GetComponent<SeaParticleController>().LeaveSky();

        }
       
	}
}
