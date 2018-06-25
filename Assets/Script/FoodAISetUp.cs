using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAISetUp : MonoBehaviour {

    public Behaviour[] stuffNeedDisable;
    // Use this for initialization
    void Start () {

        if (!PhotonNetwork.isMasterClient)
        {
            for(int i = 0;i < stuffNeedDisable.Length; i++)
            {
                stuffNeedDisable[i].enabled = false;
            }
        }

	}
	
}
