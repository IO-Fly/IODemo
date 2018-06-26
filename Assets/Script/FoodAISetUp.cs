using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAISetUp : MonoBehaviour {

    public Behaviour[] stuffNeedDisable;
    private bool isEnable;
    // Use this for initialization
    void Start () {

        isEnable = true;
        if (!PhotonNetwork.isMasterClient)
        {
            for(int i = 0;i < stuffNeedDisable.Length; i++)
            {
                stuffNeedDisable[i].enabled = false;
            }
            isEnable = false;
        }

       
	}

    void Update(){

        if (PhotonNetwork.isMasterClient && !isEnable)
        {
            for (int i = 0; i < stuffNeedDisable.Length; i++)
            {
                stuffNeedDisable[i].enabled = true;
            }
            isEnable = true;
        }

    }
	
}
