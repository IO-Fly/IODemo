using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    public PlayerSkillController[] playerSkills;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


	}

    public float GetSkillCooldown(int index)
    {
        if(index >= 0  && index < playerSkills.Length)
        {
            return playerSkills[index].GetCooldown();
        }
        else
        {
            Debug.LogError("技能索引" + index + "无效！");
            return 0;
        }
    }

    public float GetSkillCurCooldown(int index)
    {
        if (index >= 0 && index < playerSkills.Length)
        {
            return playerSkills[index].GetCurCooldown();
        }
        else
        {
            Debug.LogError("技能索引" + index + "无效！");
            return 0;
        }
    }


}
