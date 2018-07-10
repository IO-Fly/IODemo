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

    public float maxSafeFlyingDuration = 10.0f;//在天上逗留超过该时间开始扣血
    private float flyingDuration=0.0f;//在天上的持续时间

    // Use this for initialization
    void Awake () {
        player = GetComponent<Player>();
	}

    private void Start()
    {
        character = GetComponent<CharacterController>();
        speed = player.GetSpeed();

        StartCoroutine(GetHurtWhenAground());
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
                    if (enterSky)
                        flyState = FlyState.Flying;
                    else
                        flyState = FlyState.ReadyToFly;
                }
            }
        }
        if (flyState==FlyState.Flying)
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
        character.Move(new Vector3(0.0f, flySpeed * Time.deltaTime, 0.0f));
    }

    public void MoveInSeaSurface()
    {
        Vector3 playerForward = GetComponent<ObjectBehaviour>().GetForwardDirection();
        transform.LookAt(transform.position + playerForward);
        float moveY = playerForward.y < 0.0f ? playerForward.y : 0.0f;
        Vector3 moveDirection = new Vector3(playerForward.x, moveY, playerForward.z);
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

        //播放音效
        if(this.tag == "player" && this.GetComponent<Player>().photonView.isMine)
        {
            GameObject Audio = GameObject.Find("Audio");
            Audio.GetComponent<AudioManager>().PlaySeaOut();
        }
       
    }

    public void WaitForFly()
    {
        flyState = FlyState.WaitForFly;
    }

    public void EndFly()
    {
        flyState = curFlyCoolDown <= 0.0f ? FlyState.ReadyToFly : FlyState.WaitForFly;

        //播放音效
        if (this.tag == "player" && this.GetComponent<Player>().photonView.isMine)
        {
            GameObject Audio = GameObject.Find("Audio");
            Audio.GetComponent<AudioManager>().PlaySeaIn();
        }
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

    //在天空或小岛上逗留时间过长时削减生命
    IEnumerator GetHurtWhenAground()
    {
        while (true)
        {
            if (flyState == FlyState.Flying)
            {
                flyingDuration += Time.deltaTime;
                if (flyingDuration >= maxSafeFlyingDuration)
                {
                    Player player = gameObject.GetComponent<Player>();
                    player.photonView.RPC("GetDamage", PhotonTargets.AllViaServer, 10.0f);
                    yield return new WaitForSeconds(1.2f);
                }
            }
            else
            {
                flyingDuration = 0.0f;
            }
            yield return null;
        }
    }
}
