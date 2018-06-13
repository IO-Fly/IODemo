using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCopyController : Photon.PunBehaviour {

    public GameObject playerCopyPrefab;
    public float cooldown;//定义技能冷却时间
    public float keepTime;//技能效果持续时间
    public float distance;//分身距离本体的初始距离

    private GameObject playerCopy;//分身
    private float curCooldown;

    // Use this for initialization
    void Start()
    {
        curCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
       
        //技能触发
        if (Input.GetKeyDown("space") && curCooldown <= 0)
        {
            curCooldown = cooldown;

            //根据玩家位置初始化及方向
            Vector3 posOffset = Vector3.Normalize(transform.forward) * distance;
            Vector3 InitPosition = transform.position + posOffset;
            playerCopy = PhotonNetwork.Instantiate(playerCopyPrefab.name, InitPosition ,Quaternion.identity, 0);
            playerCopy.transform.rotation = transform.rotation;
            //此处应该在分身对象中保留所有者的对象，用于碰撞检测

            StartCoroutine("WaitForEndSkill");
        }

        if (curCooldown >= 0)
        {
            curCooldown -= Time.deltaTime;
        }

    }

    IEnumerator WaitForEndSkill()
    {
        yield return new WaitForSeconds(keepTime);
        if(playerCopy != null)
        {
            PhotonNetwork.Destroy(playerCopy);
            Destroy(playerCopy.GetComponent<PlayerHealthUI>().getHealthCanvas());
        }    
    }

    public GameObject getPlayerCopy()
    {
        return playerCopy;
    }
}
