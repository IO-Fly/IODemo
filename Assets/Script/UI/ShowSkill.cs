using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSkill : MonoBehaviour {

    private Text coolTimeText;

    private GameObject player;
    private SkillManager skillManager;

    void Start ()
    {
        Debug.Log(player.name);

        coolTimeText = this.transform.Find("SkillImage/SkillTimeText").gameObject.GetComponent<Text>();
        skillManager = player.GetComponent<SkillManager>();
    }

    void Update ()
    {
        coolTimeText.text = Mathf.Ceil(skillManager.GetSkillCurCooldown(0)).ToString();
	}

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }
}
