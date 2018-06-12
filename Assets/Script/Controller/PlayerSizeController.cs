using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeController : Photon.PunBehaviour {

    public float cooldown;//定义技能冷却时间
    public Vector3 addSize;//增加的大小
    public float keepTime;//技能效果持续时间

    private float curCooldown;

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
            transform.localScale += addSize;
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
        transform.localScale -= addSize;
    }

}
