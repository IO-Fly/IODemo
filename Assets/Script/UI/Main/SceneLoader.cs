using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject SceneLoaderRoot;
    public Slider LoadBarSlider;
    public Text LoadProgressText;

    private bool isOperationStarted = false;
    private bool FinishLoad = false;
    private AsyncOperation async;
    public float SceneSmoothLoad = 3;

    private void Update()
    {
        if (!isOperationStarted)
        {
            return;
        }
        if (async == null)
        {
            return;
        }
        UpdateUI();
    }

    public void LoadScene(string sceneName)
    {
        SetUpUI();
        StartCoroutine(StartAsyncOperation(sceneName));
    }

    private IEnumerator StartAsyncOperation(string sceneName)
    {
        async = PhotonNetwork.LoadLevelAsync(sceneName);
        async.allowSceneActivation = false;
        isOperationStarted = true;
        yield return async;
    }

    private void SetUpUI()
    {
        SceneLoaderRoot.SetActive(true);
        LoadBarSlider.value = 0;
        LoadProgressText.text = "0";
    }

    private void UpdateUI()
    {
        if (LoadBarSlider != null)
        {
            float p = async.progress + 0.1f;
            LoadBarSlider.value = Mathf.Lerp(LoadBarSlider.value, p, Time.deltaTime * SceneSmoothLoad);
            if (async.isDone || LoadBarSlider.value > 0.99f)
            {
                if (!FinishLoad)
                {
                    OnFinish();
                }
            }
            if (LoadProgressText != null)
            {
                string percent = (LoadBarSlider.value * 100).ToString("F0");
                LoadProgressText.text = percent;
            }
        }
    }

    private void OnFinish()
    {
        FinishLoad = true;
        async.allowSceneActivation = true;
    }

}
