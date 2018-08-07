using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnect : Photon.PunBehaviour
{
    public GameObject login;
    public GameObject lobbyCanvas;
    private GameObject lobbyScenes;

    private void Awake()
    {
        // 初始化PhotonNetwork
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.AuthValues = new AuthenticationValues()
        {
            AuthType = CustomAuthenticationType.Custom
        };

        // 获得login场景root节点
        if (login == null)
        {
            login = this.transform.parent.gameObject;
        }
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
    // 连接成功，加载lobby资源，隐藏login资源
    IEnumerator LoadLobbyResource()
    {
        yield return new WaitForSeconds(0.5f);

        if(lobbyScenes == null)
        {
            Debug.Log("加载");
            // 加载lobby资源
            ResourceRequest resourceRequest = Resources.LoadAsync("UI_prefabs/Main/LobbyScene");
            yield return resourceRequest;
            lobbyScenes = GameObject.Instantiate(resourceRequest.asset, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
            lobbyCanvas.GetComponent<LobbyUI>().SetLobbyScene(lobbyScenes);
        }

        lobbyScenes.SetActive(true);
        lobbyCanvas.SetActive(true);

        // 隐藏login资源
        login.SetActive(false);
    }

}
