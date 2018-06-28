using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFllow : MonoBehaviour {

    private GameObject player;
    private Camera mainCamera;

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }

    private void Awake()
    {
        this.mainCamera = Camera.main;
    }

    private void Update ()
    {
        if(player == null)
        {
            return;
        }

        // 更新minimapCamera的位置
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
        transform.position = minimapCameraPosition;

        // 更新minimapCamera的旋转：
        // 跟随player的旋转
        // transform.eulerAngles = new Vector3(90.0f, 0.0f, -player.transform.eulerAngles.y);
        // 跟随mainCamera的旋转
        transform.eulerAngles = new Vector3(90.0f, 0.0f, -mainCamera.transform.eulerAngles.y);
    }


}
