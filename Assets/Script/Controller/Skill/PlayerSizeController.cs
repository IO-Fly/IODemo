using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeController : PlayerSkillController {

    public Vector3 addSize;//增加的大小
    public float sizeEffect = 2;//本地视口增加效果

    public GameObject particleEffect;

    private bool skillInUse = false;//是否在技能持续时间内

    void Awake()
    {
        DisableParticle();
    }

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

            float scaleOffset = Mathf.Sqrt(player.transform.localScale.x);
            addSize = new Vector3(scaleOffset, scaleOffset, scaleOffset);

            player.AddSizeOffset(addSize);

            //开启技能效果
            this.photonView.RPC("EnableParticle", PhotonTargets.AllViaServer);

            //播放音效
            GameObject Audio = GameObject.Find("Audio");
            Audio.GetComponent<AudioManager>().PlaySizeSkill();

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
        skillInUse = true;
        yield return new WaitForSeconds(keepTime);
        
        Player player = this.gameObject.GetComponent<Player>();
        player.AddSizeOffset(-addSize);
        if (sizeEffect == 0)
        {
            Debug.LogError("大小效果倍数不能为0 !");
        }
        player.AddSizeEffect(1/sizeEffect);

        //关闭技能效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);

        skillInUse = false;
    }

    public bool SkillInUse()
    {
        return skillInUse;
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
        return PlayerSkillController.SkillType.SIZE;
    }
}
