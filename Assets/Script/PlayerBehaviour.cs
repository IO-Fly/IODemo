using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {


    public enum FlyState { WaitForFly,ReadyToFly,Flying};
    public FlyState flyState=FlyState.ReadyToFly;

    public bool enterSky = false;//角色是否进入“天空”触发器内（包括角色在海平面上的情况）

    private float flySpeed=0.0f;
    public float gravity = 9.8f;

    private float speed;

    public float minFlySpeed = 5.0f;

    public float flyCooldown = 10.0f;//定义跳跃的冷却时间
    private float curFlyCoolDown=0.0f;//当前的冷却时间

    private Player player;

    private CharacterController character;
    

    // Use this for initialization
    void Awake () {
        player = GetComponent<Player>();
        character = GetComponent<CharacterController>();

        speed = player.GetSpeed();
	}
	
	// Update is called once per frame
	void Update () {
        if (curFlyCoolDown > 0)
        {
            curFlyCoolDown -= Time.deltaTime;
            if (curFlyCoolDown <= 0.0f)
            {
                curFlyCoolDown = 0.0f;
                if (flyState == FlyState.WaitForFly)
                {
                    flyState = FlyState.ReadyToFly;
                }
            }
        }
        if (enterSky)
        {
            DropInSky();
        }
    }

    public void MoveInSky(Vector3 towards)
    {
        gameObject.transform.LookAt(gameObject.transform.position + towards);
        speed = player.GetSpeed();
        towards.y = 0.0f;
        character.Move(towards.normalized * speed * Time.deltaTime);
    }

    public void DropInSky()
    {
        flySpeed -= gravity * Time.deltaTime;
        transform.Translate(0.0f, flySpeed * Time.deltaTime, 0.0f, Space.World);
    }

    public void MoveInSeaSurface()
    {
        float moveY = transform.forward.y < 0.0f ? transform.forward.y : 0.0f;
        Vector3 moveDirection = new Vector3(transform.forward.x, moveY, transform.forward.z);
        character.Move(moveDirection * speed * Time.deltaTime);
    }


    public bool CanFly()
    {
        return flyState == FlyState.ReadyToFly;
    }

    public void StartFly()
    {
        flyState = FlyState.Flying;
        curFlyCoolDown = flyCooldown;//技能冷却
        float verticalSpeed = gameObject.GetComponent<Player>().GetSpeed() * gameObject.transform.forward.normalized.y;
        flySpeed = minFlySpeed + verticalSpeed;
    }

    public void WaitForFly()
    {
        flyState = FlyState.WaitForFly;
    }

    public void EndFly()
    {
        flyState = curFlyCoolDown <= 0.0f ? FlyState.ReadyToFly : FlyState.WaitForFly;
    }

    public void EnterSky()
    {
        enterSky = true;
    }
    public void LeaveSky()
    {
        enterSky = false;
    }
    public bool InTheSky()
    {
        return enterSky;
    }
}
