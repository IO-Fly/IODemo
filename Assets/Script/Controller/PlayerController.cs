using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerController : MonoBehaviour
{


    public float mouseSensitivity = 1.0f;


    private Vector3 towards;//角色朝向的方向
    private Vector3 up;//向上向量
    private Vector3 right;
    private float speed;

    private float currentLookAtSlerp;
    private float targetLookAtSlerp;

    private Player player;

    private CharacterController character;

    private ObjectBehaviour objectBehaviour;
    private PlayerBehaviour playerBehaviour;

<<<<<<< HEAD
=======
    private bool fly = false;
    private bool waitForFly = false;
    public float gravity = 9.8f;
    public float minFlySpeed = 10f;
    private float flySpeed;//空中飞行的垂直速度
>>>>>>> master

    // Use this for initialization
    void Start()
    {
        
        towards = new Vector3(0.0f, 0.0f, 1.0f);
        up = new Vector3(0.0f, 1.0f, 0.0f);
        right = new Vector3(1.0f, 0.0f, 0.0f);
        character = gameObject.GetComponent<CharacterController>();
        currentLookAtSlerp = 0.5f;
        targetLookAtSlerp = 0.5f;

        player = gameObject.GetComponent<Player>();

        objectBehaviour = gameObject.GetComponent<ObjectBehaviour>();
        playerBehaviour = gameObject.GetComponent<PlayerBehaviour>();

        speed = player.GetSpeed();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        HandleMouseMove();
        HandleWSAD();

        

        ////处理玩家的WSAD输入（改变角色的方向）
        //float moveHorizontal = Input.GetAxis("Horizontal");
        //float moveVertical = Input.GetAxis("Vertical");

        ////由WSAD决定的球面插值的参数（角色移动方向）
        //Vector3 lookAt = new Vector3();
        //if (moveHorizontal > 0.0f)
        //    if (moveVertical > 0.0f)
        //        targetLookAtSlerp = 0.75f;
        //    else
        //        targetLookAtSlerp = 1.0f;
        //else if (moveHorizontal < 0.0f)
        //    if (moveVertical > 0.0f)
        //        targetLookAtSlerp = 0.25f;
        //    else
        //        targetLookAtSlerp = 0.0f;
        //else if (moveVertical >= 0.0f)
        //    targetLookAtSlerp = 0.5f;

        ////平滑过渡（角色的转向）
        //if (Mathf.Abs(targetLookAtSlerp - currentLookAtSlerp) < 1e-6)
        //    currentLookAtSlerp = targetLookAtSlerp;
        //else
        //    currentLookAtSlerp += (targetLookAtSlerp - currentLookAtSlerp) * 0.4f;

        ////执行转向操作
        //lookAt = Vector3.SlerpUnclamped(-right, towards, currentLookAtSlerp * 2);
        //transform.LookAt(gameObject.transform.position + lookAt);

        ////限制不能往后移动
        //if (moveVertical < 0.0f)
        //    moveVertical = 0.0f;







        //Vector3 move = towards * moveVertical + right * moveHorizontal;



        //if (waitForFly)
        //{
        //    if (move.y > 0)
        //    {
        //        move.y = 0;
        //    }
        //    Debug.Log("向上移动：" + move.y);
        //}
        //else if (fly)
        //{
        //    move.y = 0;
        //    flySpeed -= gravity * Time.deltaTime;
        //    move.y = flySpeed * Time.deltaTime;
        //    //if (this.gameObject.transform.position.y < height && drop == false)
        //    //{
        //    //    move.y += Mathf.Sqrt(this.gameObject.transform.localScale.x);
        //    //}
        //    //else
        //    //{
        //    //    drop = true;
        //    //    move.y -= Mathf.Sqrt(this.gameObject.transform.localScale.x);
        //    //}       
        //}

        ////执行移动操作
        //speed = gameObject.GetComponent<Player>().GetSpeed();
        //character.Move(move.normalized * speed * Time.deltaTime);

    }

    public Vector3 GetTowards()
    {
        return transform.forward;
    }
    

    private void HandleMouseMove()
    {
        //当鼠标右键未被按下时，才需要对角色的朝向方向作修改
        if (!Input.GetMouseButton(1))
        {
            float mouseX = -Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            objectBehaviour.Turn(mouseX, mouseY);
        }

    }

    private void HandleWSAD()
    {
        //处理WSAD输入（改变角色的方向）
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        if (moveHorizontal > 0.0f)
            if (moveVertical > 0.0f)
                objectBehaviour.Move(ObjectBehaviour.MoveDirection.FrontRight);
            else
                objectBehaviour.Move(ObjectBehaviour.MoveDirection.Right);
        else if (moveHorizontal < 0.0f)
            if (moveVertical > 0.0f)
                objectBehaviour.Move(ObjectBehaviour.MoveDirection.FrontLeft);
            else
                objectBehaviour.Move(ObjectBehaviour.MoveDirection.Left);
        else if (moveVertical > 0.0f)
        {
            if (playerBehaviour.flyState == PlayerBehaviour.FlyState.Flying)
            {
<<<<<<< HEAD
                Vector3 towards = objectBehaviour.GetForwardDirection();
                playerBehaviour.MoveInSky(towards);
            }
            else if (playerBehaviour.enterSky&&playerBehaviour.flyState == PlayerBehaviour.FlyState.WaitForFly)
            {
                playerBehaviour.MoveInSeaSurface();
            }
            else
            {
                objectBehaviour.Move(ObjectBehaviour.MoveDirection.Front);
            }
        }
        else
            objectBehaviour.Move(ObjectBehaviour.MoveDirection.Stay);
=======
                move.y = 0;
            }   
            Debug.Log("向上移动：" + move.y);
        }
        else if (fly)
        {
            move.y = 0;
            flySpeed -= gravity * Time.deltaTime;
            move.y = flySpeed * Time.deltaTime; 
        }

        //执行移动操作
        speed = gameObject.GetComponent<Player>().GetSpeed();
        character.Move(move.normalized*speed*Time.deltaTime);

    }

    public Vector3 GetTowards()
    {
        return towards;
    }

    public bool CanFly()
    {
        if (curFlyCooldown <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public void StartFly()
    {
        waitForFly = false;
        fly = true;
        this.curFlyCooldown = flyCooldown;//技能冷却
        flySpeed = gameObject.GetComponent<Player>().GetSpeed();

        flySpeed += minFlySpeed;

    }

    public void WaitForFly()
    {
        waitForFly = true;
    }

    public void EndFly()
    {
        waitForFly = false;
        fly = false;
        flySpeed = 0;
>>>>>>> master
    }

}
