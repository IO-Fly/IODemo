using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkillController : Photon.PunBehaviour {

    public enum SkillType { SIZE, SPEED, COPY, HIDE };
    public abstract SkillType GetSkillType();
    

    public float keepTime;//技能效果持续时间
    public float cooldown;//定义技能冷却时间

    protected float curCooldown;

    protected bool skillInUse = false;//是否在技能持续时间内
    
    public float GetKeepTime()
    {
        return keepTime;
    }

    public float GetCurKeepTime()
    {
        return (keepTime - (cooldown - curCooldown));
    }

    public float GetCooldown()
    {
        return cooldown;
    }

    public float GetCurCooldown()
    {
        return curCooldown;
    }

    public bool SkillInUse()
    {
        return skillInUse;
    }
}
