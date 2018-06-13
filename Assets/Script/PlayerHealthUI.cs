using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {

    //public Slider screenHealthSliderPrefab; //当前player的血条

    public GameObject healthCanvasPrefab;   //场景中可移动画布prefab
    public Slider healthSliderPrefab;       //场景中可移动血条prefab

    private GameObject healthCanvas;//场景中可移动画布
    private Slider healthSlider;    //场景中可移动血条

    private Player player;
    private float modelHalfHeight;


    void Start ()
    {
        player = GetComponent<Player>();
        modelHalfHeight = GetComponent<MeshFilter>().mesh.bounds.size.y / 2;
        //Debug.Log(modelHalfHeight);

        //float newY = transform.position.y + transform.localScale.y * modelHalfHeight + 0.2f;
        //Vector3 newPos = new Vector3(transform.position.x, newY, transform.position.z);

        healthCanvas = GameObject.Instantiate(healthCanvasPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        healthCanvas.transform.SetParent(transform, false);
        float Yoffset = transform.localScale.y * modelHalfHeight + 0.2f;
        healthCanvas.transform.Translate(0.0f, Yoffset, 0.0f);


        healthSlider = Slider.Instantiate(healthSliderPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as Slider;
        healthSlider.transform.SetParent(healthCanvas.transform, false);
        healthSlider.maxValue = player.health;
        healthSlider.value = player.health;




        //if (player.photonView.isMine)
        //{
        //    screenHealthSlider.maxValue = player.health;
        //    screenHealthSlider.value = player.health;
        //}

    }
	

	void Update ()
    {
        if (player.health <= 0)
        {
            Destroy(healthSlider);
            Destroy(healthCanvas);
            return;
        }
        if (player.photonView.isMine)
        {
            //screenHealthSlider.value = player.health;
            healthCanvas.SetActive(false);
            return;
        }
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
