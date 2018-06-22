using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerSkillController : Photon.PunBehaviour {

    public abstract int GetSkillType();


    public float keepTime;//技能效果持续时间
    public float cooldown;//定义技能冷却时间

    protected float curCooldown;


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


}
