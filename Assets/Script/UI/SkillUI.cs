using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour {

    private Image keepShadowImage;
    private Text keepTimeText;

    private Image coolShadowImage;
    private Text coolTimeText;

    private GameObject player;
    private SkillManager skillManager;
    private float totalKeepTime;
    private float totalCoolTime;


    private Image skillImage;

    void Start ()
    {

        keepShadowImage = this.transform.Find("KeepShadowImage").gameObject.GetComponent<Image>();
        keepTimeText = this.transform.Find("KeepTimeText").gameObject.GetComponent<Text>();

        coolShadowImage = this.transform.Find("CoolShadowImage").gameObject.GetComponent<Image>();
        coolTimeText = this.transform.Find("CoolTimeText").gameObject.GetComponent<Text>();

        skillManager = player.GetComponent<SkillManager>();
        totalKeepTime = skillManager.GetSkillKeepTime(0);
        totalCoolTime = skillManager.GetSkillCooldown(0) - totalKeepTime;

        // 初始化技能图片，加载对应技能的图片
        Sprite skillSprite;
        if(skillManager.playerSkills[0].GetSkillType() == PlayerSkillController.SkillType.SIZE)
        {
            skillSprite = Resources.Load<Sprite>("UI_textures/SkillImage/big");
        }
        else if(skillManager.playerSkills[0].GetSkillType() == PlayerSkillController.SkillType.SPEED)
        {
            skillSprite = Resources.Load<Sprite>("UI_textures/SkillImage/fast");
        }
        else if (skillManager.playerSkills[0].GetSkillType() == PlayerSkillController.SkillType.COPY)
        {
            skillSprite = Resources.Load<Sprite>("UI_textures/SkillImage/bomb");
        }
        else 
        {
            skillSprite = Resources.Load<Sprite>("UI_textures/SkillImage/hide");
        }

        skillImage = this.transform.Find("SkillImage").gameObject.GetComponent<Image>();
        skillImage.sprite = skillSprite;


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
