using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeController : PlayerSkillController {

    
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
            player.AddSizeOffset(addSize);
            player.AddSizeEffect(sizeEffect);
            StartCoroutine("WaitForEndSkill");
        }

        if(curCooldown >= 0)
        {
            curCooldown -= Time.deltaTime;
        }

	}

    IEnumerator WaitForEndSkill()
    {
        yield return new WaitForSeconds(keepTime);
        Player player = this.gameObject.GetComponent<Player>();
        player.AddSizeOffset(-addSize);
        if(sizeEffect == 0)
        {
            Debug.LogError("大小效果倍数不能为0 !");
        }
        player.AddSizeEffect(1/sizeEffect);
    }

}
