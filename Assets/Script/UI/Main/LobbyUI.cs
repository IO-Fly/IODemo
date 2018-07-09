using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public List<GameObject> characterPrefabList = new List<GameObject>();
    public List<string> characterNameList = new List<string>();
    public int curCharacterIndex = 0;
    public Text characterNameText;


    private GameObject lobbySceneNode;
    public void SetLobbyScene(GameObject LobbySceneNode)
    {
        lobbySceneNode = LobbySceneNode;
    }


    public float CameraMoveTime = 0.5f;
    private GameObject mainCamera;

    private void Start()
    {
        SetCharacterNameList();
        mainCamera = Camera.main.gameObject;
    }

    // 初始化角色名字
    private void SetCharacterNameList()
    {
        if(characterNameList.Count != characterPrefabList.Count)
        {
            characterNameList.Clear();
            // 初始化character列表
            for (int i = 0; i < characterPrefabList.Count; i++)
            {
                characterNameList.Add(characterPrefabList[i].name);
            }
        }
        // 默认角色
        if (characterNameList.Count > 0)
        {
            characterNameText.text = characterNameList[curCharacterIndex];
        }
    }

    // 选择角色
    public void OnLeftButton()
    {
        curCharacterIndex = (curCharacterIndex == 0) ? (characterNameList.Count - 1) : (curCharacterIndex - 1);
        ChangeCharacter();
    }
    public void OnRightButton()
    {
        curCharacterIndex = (curCharacterIndex == characterNameList.Count - 1) ? 0 : (curCharacterIndex + 1);
        ChangeCharacter();
    }

    // 转换角色
    public void ChangeCharacter()
    {
        // 更换角色名字
        characterNameText.text = characterNameList[curCharacterIndex];

        // 更换camera位置
        if(lobbySceneNode == null)
        {
            lobbySceneNode = GameObject.Find("LobbyScene").gameObject;
        }
        GameObject curCameraNode = lobbySceneNode.transform.Find("character" + (curCharacterIndex + 1).ToString() + "/Camera").gameObject;
        StartCoroutine(MoveCamera(mainCamera.transform, curCameraNode.transform, CameraMoveTime));
    }

    IEnumerator MoveCamera(Transform start, Transform target, float time)
    {
        var dur = 0.0f;
        while (dur <= time)
        {
            dur += Time.deltaTime;
            Vector3 curCamPos = Vector3.Lerp(start.position, target.position, dur / time);
            Quaternion curCamRotation = Quaternion.Lerp(start.rotation, target.rotation, dur / time);
            mainCamera.transform.SetPositionAndRotation(curCamPos, curCamRotation);
            yield return null;
        }
        mainCamera.transform.SetPositionAndRotation(target.position, target.rotation);
    }

    // 开始匹配
    public void OnStartMatching()
    {
        PhotonNetwork.player.NickName = characterPrefabList[curCharacterIndex].name;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("0.0.1");
        }
    }


}
