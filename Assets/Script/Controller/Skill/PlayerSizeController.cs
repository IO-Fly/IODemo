using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSizeController : PlayerSkillController {

    public Vector3 addSize;//增加的大小
    public float sizeEffect = 2;//本地视口增加效果

    public GameObject particleEffect;

    protected bool inMaxSize = false;

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
        if (Input.GetKeyDown("space") && curCooldown <= 0 && gameObject.GetComponent<PlayerAI>() == null)
        {
            useSkill();
        }

        if (curCooldown > 0&&!inMaxSize)
        {
            curCooldown -= Time.deltaTime;
            curCooldown = curCooldown < 0 ? 0 : curCooldown;
        }

    }

    public void useSkill(){
        if (!skillInUse)
            StartCoroutine(HandleUseSkill());
    }


    IEnumerator HandleUseSkill()
    {
        skillInUse = true;


        Player player = this.gameObject.GetComponent<Player>();
        ///player.AddSizeEffect(sizeEffect);

        float scaleOffset = Mathf.Sqrt(player.transform.localScale.x);
        addSize = new Vector3(scaleOffset, scaleOffset, scaleOffset);

        yield return StartCoroutine(Largen());

        inMaxSize = true;

        

        //开启技能效果
        this.photonView.RPC("EnableParticle", PhotonTargets.AllViaServer);

        //播放音效
        GameObject Audio = GameObject.Find("Audio");
        Audio.GetComponent<AudioManager>().PlaySizeSkill();

        

        StartCoroutine("WaitForEndSkill");

        inMaxSize = false;
        curCooldown = cooldown;
        StartCoroutine(Diminish());
    }


    IEnumerator Largen()
    {
        Player player = this.gameObject.GetComponent<Player>();
        for (int i = 0; i < 100; ++i)
        {
            player.SetSizeEffect(1 + i / 100);
            player.AddSizeOffset(addSize / 100.0f);
            player.GetComponent<CharacterController>().Move(new Vector3(Random.Range(-0.001f, 0.001f), Random.Range(-0.001f, 0.001f), Random.Range(-0.001f, 0.001f)));
            yield return null;
        }
    }

    IEnumerator Diminish()
    {
        yield return new WaitForSeconds(keepTime);
        Player player = this.gameObject.GetComponent<Player>();
        for (int i = 99; i >=0; --i)
        {
            player.SetSizeEffect(1 + i / 100);
            player.AddSizeOffset(-addSize / 100.0f);
            yield return null;
        }
    }

    IEnumerator WaitForEndSkill()
    {

        yield return new WaitForSeconds(keepTime);

        ///Player player = this.gameObject.GetComponent<Player>();
       /// player.AddSizeOffset(-addSize);
        if (sizeEffect == 0)
        {
            Debug.LogError("大小效果倍数不能为0 !");
        }
        ///player.SetSizeEffect(1);

        //关闭技能效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);

        skillInUse = false;
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
