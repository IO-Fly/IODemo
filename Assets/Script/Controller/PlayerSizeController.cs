using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeController : PlayerSkillController {

    public override SkillType GetSkillType()
    {
        return PlayerSkillController.SkillType.SIZE;
    }

    public Vector3 addSize;//增加的大小
    public float sizeEffect = 2;//本地视口增加效果


	// Use this for initialization
	void Start () {
        curCooldown = 0;
	}
	
	// Update is called once per frame
	void Update () {

        //技能触发
        if (Input.GetKeyDown("space") && curCooldown <= 0)
        {
            curCooldown = cooldown;
            Player player = this.gameObject.GetComponent<Player>();
            player.AddSizeEffect(sizeEffect);

            player.photonView.RPC("AddSizeOffset", PhotonTargets.AllViaServer, addSize);
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
        Player player = this.gameObject.GetComponent<Player>();
        player.photonView.RPC("AddSizeOffset", PhotonTargets.AllViaServer, -addSize);
        if (sizeEffect == 0)
        {
            Debug.LogError("大小效果倍数不能为0 !");
        }
        player.AddSizeEffect(1/sizeEffect);
    }

}
