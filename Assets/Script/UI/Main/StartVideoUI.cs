using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StartVideoUI : MonoBehaviour
{
    public GameObject loginScene;
    public GameObject loginCanvas;
    public static bool isSkipVideo = false;
    public VideoPlayer vp;

    private void Awake()
    {
        if (isSkipVideo == true)
        {
            this.gameObject.SetActive(false);
            loginScene.SetActive(true);
            loginCanvas.SetActive(true);
            Destroy(this.gameObject);
        }
        else
        {
            if(vp == null)
            {
                vp = this.GetComponent<VideoPlayer>();
            }
        }
    }

    private void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Space) || (ulong)vp.frame >= vp.frameCount)
        {
            loginScene.SetActive(true);
            loginCanvas.SetActive(true);
            Destroy(this.gameObject);
            return;
        }
	}
}
