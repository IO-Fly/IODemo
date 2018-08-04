using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpeedController : PlayerSkillController {


    public float addSpeed;//增加的速度

    public GameObject particleEffect;

    void Awake()
    {    
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
        if (Input.GetKeyDown("space") && curCooldown <= 0 && gameObject.GetComponent<PlayerAI>() == null)
        {
            useSkill();
            
        }

        if (curCooldown > 0)
        {
            curCooldown -= Time.deltaTime;
            curCooldown = curCooldown < 0 ? 0 : curCooldown;
        }

    }

    public void useSkill()
    {
        curCooldown = cooldown;
        gameObject.GetComponent<Player>().AddSpeedOffset(addSpeed);

        //开启粒子效果
        this.photonView.RPC("EnableParticle", PhotonTargets.AllViaServer);

        //播放音效
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlaySpeedSkill();

        StartCoroutine("WaitForEndSkill");
    }

    IEnumerator WaitForEndSkill()
    {
        yield return new WaitForSeconds(keepTime);
        gameObject.GetComponent<Player>().AddSpeedOffset(-addSpeed);

        //播放音效
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().StopSpeedSkill();

        //关闭粒子效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);

    }


    [PunRPC]
    protected void EnableParticle()
    {

        ParticleSystem[] systems = particleEffect.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Play();
        }

    }

    [PunRPC]
    protected void DisableParticle()
    {

        ParticleSystem[] systems = particleEffect.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Clear();
            systems[i].Pause();
        }
    }

    public override SkillType GetSkillType()
    {
        return PlayerSkillController.SkillType.SPEED;
    }
}
