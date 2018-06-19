﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    
    public float mouseSensitivity = 1.0f;
    public float flyCooldown;//定义跳跃的冷却时间
    private float curFlyCooldown;//当前的冷却时间

    private Vector3 towards;//角色朝向的方向
    private Vector3 up;//向上向量
    private Vector3 right;
    private float speed;

    private float currentLookAtSlerp;
    private float targetLookAtSlerp;

    private CharacterController character;


    private bool fly = false;
    private bool waitForFly = false;
    public float gravity = 9.8f;
    public float minFlySpeed = 10f;
    private float flySpeed;//空中飞行的垂直速度

    // Use this for initialization
    void Start () {
        speed = gameObject.GetComponent<Player>().GetSpeed();
        towards = new Vector3(0.0f, 0.0f, 1.0f);
        up = new Vector3(0.0f, 1.0f, 0.0f);
        right = new Vector3(1.0f, 0.0f, 0.0f);
        character = gameObject.GetComponent<CharacterController>();
        currentLookAtSlerp = 0.5f;
        targetLookAtSlerp = 0.5f;
        curFlyCooldown = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (curFlyCooldown > 0)
        {
            curFlyCooldown -= Time.deltaTime;
            curFlyCooldown = curFlyCooldown < 0 ? 0 : curFlyCooldown;    
        }

    }

    private void FixedUpdate()
    {
        //如果鼠标右键被按下，则不需要对角色的朝向方向作任何修改
        if (!Input.GetMouseButton(1))
        {
            float mouseX = -Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            //ctrlX和ctrlY可置为1或-1，控制角色朝向的操控方式
            float ctrlX = -1.0f, ctrlY = -1.0f;

            right = Vector3.Cross(towards, up);
            towards = Vector3.SlerpUnclamped(towards, up, ctrlY * mouseY / 90.0f);
            towards.Normalize();
            //限制最大“俯仰角”不超过45度
            if (towards.y * towards.y > towards.x * towards.x + towards.z * towards.z)
            {
                float sqXXZZ = Mathf.Sqrt(towards.x * towards.x + towards.z * towards.z);
                towards.y = towards.y > 0 ? 0.707f : -0.707f;
                towards.x = 0.707f * towards.x / sqXXZZ;
                towards.z = 0.707f * towards.z / sqXXZZ;
            }
            towards.Normalize();
            towards = Vector3.SlerpUnclamped(towards, right, ctrlX * mouseX / 90.0f);
            towards.Normalize();
            right = new Vector3(towards.z, 0.0f, -towards.x).normalized;
            up = Vector3.Cross(right, towards);
            up.Normalize();
        }

        //处理玩家的WSAD输入（改变角色的方向）
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //由WSAD决定的球面插值的参数（角色移动方向）
        Vector3 lookAt = new Vector3();
        if (moveHorizontal > 0.0f)
            if (moveVertical > 0.0f)
                targetLookAtSlerp = 0.75f;
            else
                targetLookAtSlerp = 1.0f;
        else if (moveHorizontal < 0.0f)
            if (moveVertical > 0.0f)
                targetLookAtSlerp = 0.25f;
            else
                targetLookAtSlerp = 0.0f;
        else if (moveVertical >= 0.0f)
            targetLookAtSlerp = 0.5f;

        //平滑过渡（角色的转向）
        if (Mathf.Abs(targetLookAtSlerp - currentLookAtSlerp) < 1e-6)
            currentLookAtSlerp = targetLookAtSlerp;
        else
            currentLookAtSlerp += (targetLookAtSlerp - currentLookAtSlerp) * 0.4f;

        //执行转向操作
        lookAt = Vector3.SlerpUnclamped(-right, towards, currentLookAtSlerp*2);
        transform.LookAt(gameObject.transform.position + lookAt);

        //限制不能往后移动
        if (moveVertical < 0.0f)
            moveVertical = 0.0f;

        
        Vector3 move = towards * moveVertical + right * moveHorizontal;

       
      
        if (waitForFly)
        {
            if(move.y > 0)
            {
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
    }

}
