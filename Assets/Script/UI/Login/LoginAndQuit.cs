using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginAndQuit : Photon.PunBehaviour
{
    private InputField inputName;

    private void Awake()
    {
        // 获取UI组件
        inputName = this.transform.Find("NameInputField").gameObject.GetComponent<InputField>();

        // 初始化PhotonNetwork
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues()
        {
            AuthType = CustomAuthenticationType.Custom
        };
    }

    // 限制输入的账号长度
    public void OnInputName()
    {
        if (inputName.text.Length > 10)
        {
            inputName.text = inputName.text.Substring(0, 10);
        }

    }

    // 登陆游戏
    public void OnLoginGame()
    {
        //房间唯一ID，相同ID的用户不会加入同一个房间
        PhotonNetwork.AuthValues.UserId = inputName.text;
        NetworkMatch.playerName = inputName.text;

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("0.0.1");
        }
    }

    // 连接服务器成功
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        SceneManager.LoadScene("Scenes/Lobby");
    }

    // 连接服务器失败
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);
        Debug.LogWarning("无法连接服务器");
    }

    // 退出游戏
    public void OnQuitGame()
    {
        Application.Quit();
    }
}
