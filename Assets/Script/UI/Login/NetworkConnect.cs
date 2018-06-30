using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnect : Photon.PunBehaviour
{
    public GameObject lobbyCanvas;

    private void Awake()
    {
        // 初始化PhotonNetwork
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
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
    // 连接成功，加载lobby资源，切换 loginCanvas 到 lobbyCanvas
    IEnumerator LoadLobbyResource()
    {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.5f);

        lobbyCanvas.SetActive(true);
        this.gameObject.SetActive(false);
    }

}
