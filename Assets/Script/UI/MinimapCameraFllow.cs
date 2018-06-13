using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFllow : MonoBehaviour {

    private GameObject player;

    void Update ()
    {
        transform.position = player.transform.position + new Vector3(0.0f, 10.0f, 0.0f);
	}

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }
}
