using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSkill : MonoBehaviour {
    
    private Image keepShadowImage;
    private Text keepTimeText;

    private Image coolShadowImage;
    private Text coolTimeText;

    private GameObject player;
    private SkillManager skillManager;
    private float totalKeepTime;
    private float totalCoolTime;


    void Start ()
    {
        keepShadowImage = this.transform.Find("KeepShadowImage").gameObject.GetComponent<Image>();
        keepTimeText = this.transform.Find("KeepTimeText").gameObject.GetComponent<Text>();

        coolShadowImage = this.transform.Find("CoolShadowImage").gameObject.GetComponent<Image>();
        coolTimeText = this.transform.Find("CoolTimeText").gameObject.GetComponent<Text>();

        skillManager = player.GetComponent<SkillManager>();
        totalKeepTime = skillManager.GetSkillKeepTime(0);
        totalCoolTime = skillManager.GetSkillCooldown(0) - totalKeepTime;
    }

    void Update ()
    {
        float curKeepTime = skillManager.GetSkillCurKeepTime(0);
        if (curKeepTime > 0)
        {
            // 技能在释放阶段
            keepTimeText.gameObject.SetActive(true);
            keepTimeText.text = Mathf.Ceil(curKeepTime).ToString() + "s";

            coolShadowImage.fillAmount = 1;
            coolTimeText.gameObject.SetActive(false);
            return;
        }
        keepTimeText.gameObject.SetActive(false);

        float curCoolTime = skillManager.GetSkillCurCooldown(0);
        if (curCoolTime > 0)
        {
            // 技能在冷却阶段
            coolShadowImage.fillAmount = curCoolTime / totalCoolTime;
            coolTimeText.gameObject.SetActive(true);
            coolTimeText.text = Mathf.Ceil(curCoolTime).ToString() + "s";
        }
        else
        {
            // 技能在可以使用阶段
            coolShadowImage.fillAmount = 0;
            coolTimeText.gameObject.SetActive(false);
        }
    }

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }
}
