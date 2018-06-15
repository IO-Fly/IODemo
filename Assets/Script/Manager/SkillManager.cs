using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    public PlayerSkillController[] playerSkills;

    // 获取 总的keepTime
    public float GetSkillKeepTime(int index)
    {
        if (index >= 0 && index < playerSkills.Length)
        {
            return playerSkills[index].GetKeepTime();
        }
        else
        {
            Debug.LogError("技能索引" + index + "无效！");
            return 0;
        }
    }
    // 获取 剩余的keepTime
    public float GetSkillCurKeepTime(int index)
    {
        if (index >= 0 && index < playerSkills.Length)
        {
            return playerSkills[index].GetCurKeepTime();
        }
        else
        {
            Debug.LogError("技能索引" + index + "无效！");
            return 0;
        }
    }

    // 获取 从技能开始到下一次技能可以开始的coolTime
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

    // 获取 从现在到下一次技能可以开始的coolTime
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
