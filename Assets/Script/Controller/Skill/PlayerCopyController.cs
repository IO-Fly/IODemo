using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCopyController : PlayerSkillController {


    public GameObject playerCopyPrefab;
    public GameObject particleEffect;

    private GameObject playerCopy;//分身

    void Awake()
    {
        ParticleSystem[] systems = particleEffect.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Pause();
        }

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

            //根据玩家初始化位置，方向，大小
            //Vector3 posOffset = Vector3.Normalize(transform.forward) * distance;
            //Vector3 InitPosition = transform.position + posOffset;
            Vector3 InitPosition = transform.position;
            playerCopy = PhotonNetwork.Instantiate(playerCopyPrefab.name, InitPosition ,Quaternion.identity, 0);
            //复制本身属性到分身
            playerCopy.GetComponent<Player>().CopyPlayer(this.gameObject.GetComponent<Player>());

            //开启技能效果
            this.photonView.RPC("EnableParticle", PhotonTargets.AllViaServer);

            //播放音效
            GameObject Audio = GameObject.Find("Audio");
            Audio.GetComponent<AudioManager>().PlayCopySkill();

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

        //关闭技能效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);

    }

    public GameObject getPlayerCopy()
    {
        return playerCopy;
    }

    [PunRPC]
    protected void EnableParticle()
    {
        ParticleSystem[] systems = particleEffect.GetComponentsInChildren<ParticleSystem>();
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Play();
        }
        particleEffect.transform.parent = null;

    }

    [PunRPC]
    protected void DisableParticle()
    {

        particleEffect.transform.parent = this.transform;
        particleEffect.transform.localPosition = Vector3.zero;

    }

    public override SkillType GetSkillType()
    {
        return PlayerSkillController.SkillType.COPY;
    }


}
