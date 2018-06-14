using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {

    public GameObject healthUIPrefab;   //场景中可移动的画布及血条prefab
    private GameObject healthCanvas;    //场景中可移动画布节点
    private Slider healthSlider;        //场景中可移动血条节点

    private Player player;
    private float modelHalfHeight = 1.5f;

    private Slider screenHealthSlider; //当前player的血条,固定位置

    void Start ()
    {
        // 获得player
        player = GetComponent<Player>();

        // 获得模型原始高度，为了可移动血条的位置，这里固定为 1.5f
        //modelHalfHeight = GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
        //Debug.Log("模型高度" + modelHalfHeight);

        // 初始化可移动的画布及血条
        healthCanvas = GameObject.Instantiate(healthUIPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        healthCanvas.transform.localScale += new Vector3(0.02f, 0.05f, 0.05f);
        healthCanvas.transform.SetParent(transform, false);
        //Debug.Log("模型缩放比例" + healthCanvas.transform.localScale);

        healthSlider = healthCanvas.transform.Find("PlayerHealthSlider").gameObject.GetComponent<Slider> ();
        healthSlider.maxValue = player.health;
        healthSlider.value = player.health;

        // 初始化固定的血条
        if (player.photonView.isMine)
        {
            //screenHealthSlider = GameObject.Find("HUDCanvas/CurPlayerHealthUI/HealthSlider").GetComponent<Slider>();
            GameObject rootCanvas = GameObject.Find("HUDCanvas");
            screenHealthSlider = rootCanvas.transform.Find("CurPlayerHealthUI/HealthSlider").gameObject.GetComponent<Slider>();

            screenHealthSlider.maxValue = player.health;
            screenHealthSlider.value = player.health;
        }

    }
	

	void Update ()
    {
        // 判断player是否死亡
        if (!player || player.health <= 0)
        {
            Destroy(healthSlider);
            Destroy(healthCanvas);
            if (player.photonView.isMine)
            {
                screenHealthSlider.value = 0;
            }
            return;
        }
        // 判断是否是当前客户端的player
        if (player.photonView.isMine)
        {
            screenHealthSlider.value = player.health;
            healthCanvas.SetActive(false);
            return;
        }
        // 更新敌人血条位置和血量
        if (healthCanvas)
        {
            healthSlider.value = player.health;

            float newY = transform.position.y + transform.localScale.y * modelHalfHeight + 0.2f;
            Vector3 newPos = new Vector3(transform.position.x, newY, transform.position.z);
            healthCanvas.transform.position = newPos;
            //healthCanvas.transform.rotation = Camera.main.transform.rotation;
            healthCanvas.transform.LookAt(Camera.main.transform);
        }

    }

    public GameObject getHealthCanvas()
    {
        return healthCanvas;
    }
}
