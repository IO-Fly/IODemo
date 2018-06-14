using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSkill : MonoBehaviour {

    private Image shadowImage;
    private Text coolTimeText;

    private GameObject player;
    private SkillManager skillManager;
    private float totalTime;

    void Start ()
    {
        shadowImage = this.transform.Find("ShadowImage").gameObject.GetComponent<Image>();
        coolTimeText = this.transform.Find("CoolTimeText").gameObject.GetComponent<Text>();

        Debug.Log(player.name);
        skillManager = player.GetComponent<SkillManager>();
        totalTime = skillManager.GetSkillCooldown(0);
    }

    void Update ()
    {
        float remainTime = skillManager.GetSkillCurCooldown(0);

        if (remainTime <= 0 )
        {
            shadowImage.fillAmount = 0;
            coolTimeText.gameObject.SetActive(false);
            return;
        }

        shadowImage.fillAmount = remainTime / totalTime;
        coolTimeText.gameObject.SetActive(true);
        coolTimeText.text = Mathf.Ceil(remainTime).ToString() + "s";
    }

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }
}
