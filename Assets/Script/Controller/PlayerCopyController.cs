using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCopyController : PlayerSkillController {

    public override SkillType GetSkillType()
    {
        return PlayerSkillController.SkillType.COPY;
    }

    public GameObject playerCopyPrefab;
    //public float distance;//分身距离本体的初始距离

    private GameObject playerCopy;//分身

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

            //根据玩家初始化位置，方向，大小
            //Vector3 posOffset = Vector3.Normalize(transform.forward) * distance;
            //Vector3 InitPosition = transform.position + posOffset;
            Vector3 InitPosition = transform.position;
            playerCopy = PhotonNetwork.Instantiate(playerCopyPrefab.name, InitPosition ,Quaternion.identity, 0);
            //复制本身属性到分身
            playerCopy.GetComponent<Player>().CopyPlayer(this.gameObject.GetComponent<Player>());
            
            StartCoroutine("WaitForEndSkill");
        }

        if (curCooldown > 0)
        {
            curCooldown -= Time.deltaTime;
            curCooldown = curCooldown < 0 ? 0 : curCooldown;
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
