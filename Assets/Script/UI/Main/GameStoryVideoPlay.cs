using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameStoryVideoPlay : MonoBehaviour
{
    public GameObject loginScene;
    public GameObject loginCanvas;
    public VideoPlayer vp;
    public static bool isSkipVideo = false;

    private void Awake()
    {
        GameObject loginRoot = GameObject.Find("Login");
        if (loginScene == null)
        {
            loginScene = loginRoot.transform.Find("LoginSceneCamera").gameObject;
        }
        if (loginCanvas == null)
        {
            loginCanvas = loginRoot.transform.Find("LoginCanvas").gameObject;
        }

        if (isSkipVideo == true)
        {
            // 跳过开场视频：销毁该gameobject，加载login场景
            this.gameObject.SetActive(false);
            loginScene.SetActive(true);
            loginCanvas.SetActive(true);
            Destroy(this.gameObject);
        }
        else
        {
            // 不跳过开场视频：初始化 videoplayer
            if (vp == null)
            {
                vp = this.GetComponent<VideoPlayer>();
            }
        }
    }

    private void Update ()
    {
        // 按下任意按钮或视频播放结束, 切换到 login场景
		if(Input.anyKeyDown|| (ulong)vp.frame >= vp.frameCount)
        {
            loginScene.SetActive(true);
            loginCanvas.SetActive(true);
            Destroy(this.gameObject);
            return;
        }
	}
}
