using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHideController : PlayerSkillController {


    public GameObject particleEffect;

    private Shader transparentShader;
    private List<HideObject> hideObjects = new List<HideObject>();

    //原本着色器缓存
    public struct HideObject
    {
        public GameObject gameObject;
        public Shader shader;
        public HideObject(GameObject gameObject, Shader shader)
        {
            this.gameObject = gameObject;
            this.shader = shader;
        }
    }
    

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
        transparentShader = Shader.Find("Transparent/Diffuse");
        curCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //技能触发
        if (Input.GetKeyDown("space") && curCooldown <= 0)
        {
            curCooldown = cooldown;
            this.photonView.RPC("HidePlayer", PhotonTargets.AllViaServer, true);
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
        this.photonView.RPC("HidePlayer", PhotonTargets.AllViaServer, false);

        //关闭粒子效果
        this.photonView.RPC("DisableParticle", PhotonTargets.AllViaServer);    
    }

    [PunRPC]
    void HidePlayer(bool isHide)
    {

        if (!this.photonView.isMine)
        {
            this.gameObject.GetComponent<PlayerHealthUI>().getHealthCanvas().SetActive(!isHide);
            Renderer[] renders = this.gameObject.GetComponentsInChildren<Renderer>();
            //在其他玩家的视口下隐藏/显示本身
            foreach (Renderer m in renders)
            {
                m.enabled = !isHide;
            }
            //显示粒子效果
            if (isHide)
            {
                renders = this.GetComponentInChildren<ParticleSystem>().GetComponents<Renderer>();
                foreach (Renderer m in renders)
                {
                    m.enabled = true;
                }
            }
        }
        else
        {
            //在本身的视口下的透明效果
            if (isHide)
            {
                HideSelf();
            }
            else
            {
                ShowSelf();
            }       
        }    
    }

    [PunRPC]
    protected void EnableParticle()
    {
        //effect.Play();
        //effect.transform.parent = null;
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

<<<<<<< HEAD

    public bool SkillInUse()
    {
        return curCooldown > 0.0f;
    }

=======
    void HideSelf()
    {
        Renderer[] renders = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            Material material = render.material;
            HideObject hideObject = new HideObject(render.gameObject, material.shader);
            material.shader = transparentShader;
            material.color = new Color(material.color.r, material.color.g, material.color.b, 0.5f);

            hideObjects.Add(hideObject);
        }
    }

    void ShowSelf()
    {      
        for(int i = 0; i < hideObjects.Count; i++)
        {
            Renderer render = hideObjects[i].gameObject.GetComponent<Renderer>();
            render.material.shader = hideObjects[i].shader;
        }
        hideObjects.Clear();
    }

    public override SkillType GetSkillType()
    {
        return PlayerSkillController.SkillType.HIDE;
    }


>>>>>>> master
}
