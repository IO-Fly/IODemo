using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedController : PlayerSkillController {

    public override int GetSkillType()
    {
        return 2;
    }

    public float addSpeed;//增加的速度

    protected ParticleSystem effect;

    void Awake()
    {
        effect = gameObject.GetComponentInChildren<ParticleSystem>();
        DisableParticle();
    }

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
            //开启粒子效果
            this.photonView.RPC("EnableParticle", PhotonTargets.AllViaServer);
           
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

        //关闭粒子效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);

    }

    [PunRPC]
    protected void EnableParticle()
    {
        effect.Play();
    }

    [PunRPC]
    protected void DisableParticle()
    {

        effect.Clear();
        effect.Pause();
    }

}
