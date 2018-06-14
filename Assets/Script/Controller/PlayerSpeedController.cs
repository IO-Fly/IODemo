using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedController : MonoBehaviour {

    public float cooldown;//定义技能冷却时间
    public float addSpeed;//增加的速度
    public float keepTime;//技能效果持续时间

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
            gameObject.GetComponent<Player>().AddSpeed(addSpeed);
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
        gameObject.GetComponent<Player>().AddSpeed(-addSpeed);
        Debug.Log("当前速度:" +gameObject.GetComponent<Player>().GetSpeed());
    }
}
