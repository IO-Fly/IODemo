using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {

    public GameObject healthCanvasPrefab;
    public Slider healthSliderPrefab;

    private GameObject healthCanvas;
    private Slider healthSlider;

    private Player player;
    private float modelHalfHeight;
    private float modelStartLocalScale;


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

    }
	

	void Update ()
    {
        if (player.health <= 0)
        {
            Destroy(healthSlider);
            Destroy(healthCanvas);
        }
        if (healthCanvas)
        {
            healthSlider.value = player.health;

            //float newY = transform.position.y + transform.localScale.y * modelHalfHeight + 0.2f;
            //Vector3 newPos = new Vector3(transform.position.x, newY, transform.position.z);
            //healthCanvas.transform.position = newPos;
            healthCanvas.transform.rotation = Camera.main.transform.rotation;
            //healthCanvas.transform.LookAt(Camera.main.transform);
        }

    }

    public GameObject getHealthCanvas()
    {
        return healthCanvas;
    }
}
