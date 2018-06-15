using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedController : PlayerSkillController {

    public float addSpeed;//增加的速度
  
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
            gameObject.GetComponent<Player>().AddSpeedOffset(addSpeed);    
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
        gameObject.GetComponent<Player>().AddSpeedOffset(-addSpeed); 
    }
}
