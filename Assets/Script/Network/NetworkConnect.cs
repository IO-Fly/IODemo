using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnect : Photon.PunBehaviour
{
    public GameObject loginScene;
    public GameObject lobbyCanvas;

    private void Awake()
    {
        // 初始化PhotonNetwork
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.AuthValues = new AuthenticationValues()
        {
            AuthType = CustomAuthenticationType.Custom
        };
    }

    // 连接服务器失败
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);

        GetComponent<LoginUI>().SetConnectTip("无法连接服务器");
        StartCoroutine(Reset());
    }
    // 连接失败，重置登陆界面
    IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<LoginUI>().SetEnterUI(true);
    }

    // 连接服务器成功
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        GetComponent<LoginUI>().SetConnectTip("正在进入游戏");
        StartCoroutine(LoadLobbyResource());
    }
    // 连接成功，加载lobby资源，释放login资源
    IEnumerator LoadLobbyResource()
    {
        yield return new WaitForSeconds(0.5f);

        ResourceRequest resourceRequest = Resources.LoadAsync("UI_prefabs/Lobby/LobbyScene");
        yield return resourceRequest;

        //Debug.LogWarning("异步加载完成");
        // 加载lobby资源
        GameObject lobbyScene = GameObject.Instantiate(resourceRequest.asset, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        lobbyCanvas.SetActive(true);
        lobbyCanvas.GetComponent<LobbyUI>().SetLobbyScene(lobbyScene);

        // 释放login资源
        //this.gameObject.SetActive(false);
        //loginScene.gameObject.SetActive(false);
        Destroy(this.gameObject);
        Destroy(loginScene);
    }

}
