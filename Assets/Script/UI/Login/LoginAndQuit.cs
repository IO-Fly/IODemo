using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginAndQuit : Photon.PunBehaviour
{
    public int maxBytes = 10;
    public Text nameTip;
    public InputField inputName;
    public Button loginButton;
    public Text connectTip;
    public GameObject loadingCircle;

    private void Awake()
    {
        // 初始化PhotonNetwork
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues()
        {
            AuthType = CustomAuthenticationType.Custom
        };

        // 获取输入焦点
        inputName.ActivateInputField();
    }

    private void Update()
    {
        // 按enter键开始游戏
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnLoginGame();
        }
    }

    // 登陆游戏
    public void OnLoginGame()
    {
        // 判断输入是否为空
        if (inputName.text.Length == 0)
        {
            Debug.LogWarning("请输入账号！");
            inputName.ActivateInputField();
            return;
        }
        // 设置UI
        SetLoginUI(false);

        // 开始连接
        // 房间唯一ID，相同ID的用户不会加入同一个房间
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

        connectTip.text = "正在进入游戏";
        StartCoroutine(LoadLobbyScene());        
    }
    // 切换场景
    IEnumerator LoadLobbyScene()
    {
        yield return new WaitForEndOfFrame();
        AsyncOperation async = SceneManager.LoadSceneAsync("Scenes/Lobby");
        //while (!async.isDone)
        //{
        //    yield return null;
        //}
        yield return async;
    }

    // 连接服务器失败
    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        base.OnFailedToConnectToPhoton(cause);

        connectTip.text = "无法连接服务器";
        StartCoroutine(ReStart());
    }
    // 返回登录
    IEnumerator ReStart()
    {
        yield return new WaitForSeconds(1);
        SetLoginUI(true);
    }

    // 退出游戏
    public void OnQuitGame()
    {
        Application.Quit();
    }


    // 限制输入的账号长度
    /// <字符编码>
    ///     utf-8：中文3个字节，英文1个字节
    ///     unicode：中英文都是2个字节
    ///     gbk：中文2个字节，英文1个字节
    /// </字符编码>
    public void OnInputName()
    {
        if (inputName.text.Length > 0)
        {
            byte[] bytestr = System.Text.Encoding.GetEncoding("GBK").GetBytes(inputName.text);
            if(bytestr.Length > maxBytes)
            {
                inputName.text = inputName.text.Substring(0, inputName.text.Length - 1);
            }
        }
    }


    // 登录UI与连接服务器UI切换
    private void SetLoginUI(bool flag)
    {
        nameTip.gameObject.SetActive(flag);
        inputName.gameObject.SetActive(flag);
        loginButton.gameObject.SetActive(flag);

        connectTip.gameObject.SetActive(!flag);
        loadingCircle.SetActive(!flag);
    }
}
