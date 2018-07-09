using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVideoUI : MonoBehaviour
{
    public GameObject loginScene;
    public GameObject loginCanvas;
    public static bool isSkipVideo = false;

    private void Awake()
    {
        if (isSkipVideo == true)
        {
            this.gameObject.SetActive(false);
            loginScene.SetActive(true);
            loginCanvas.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    private void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            loginScene.SetActive(true);
            loginCanvas.SetActive(true);
            Destroy(this.gameObject);
        }
	}
}
