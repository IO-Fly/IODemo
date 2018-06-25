using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFllow : MonoBehaviour {

    private GameObject player;

    void Update ()
    {
        if(player == null)
        {
            return;
        }

        Vector3 minimapCameraPosition = player.transform.position;
        if(minimapCameraPosition.y < -4.0f)
        {
            minimapCameraPosition.y = 1.0f;
        }
        else if (minimapCameraPosition.y < 1.0f)
        {
            minimapCameraPosition.y += 20.0f;
        }
        else
        {
            minimapCameraPosition.y = 100.0f;
        }
        //transform.position = Vector3.Slerp(transform.position, minimapCameraPosition);
        transform.position = minimapCameraPosition;

    }

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }
}
