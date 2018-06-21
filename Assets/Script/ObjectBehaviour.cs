using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour {

    public float defaultSpeed=20.0f;

    public enum MoveDirection { Left,FrontLeft,Front,FrontRight,Right,Stay};

    private Vector3 towards;
    private Vector3 up;
    private Vector3 right;

    private Vector3 targetTowards;

    private float currentLookAtSlerp = 0.5f;
    private float targetLookAtSlerp = 0.5f;

    private CharacterController character;

    private void Awake()
    {
        towards = new Vector3(0.0f, 0.0f, 1.0f);
        up = new Vector3(0.0f, 1.0f, 0.0f);
        right = new Vector3(1.0f, 0.0f, 0.0f);

        towards = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));

        character = GetComponent<CharacterController>();
    }


    private void Update()
    {
        //平滑过渡
        if (Mathf.Abs((targetTowards - towards).magnitude) < 1e-6)
            towards = targetTowards;
        else
            towards = Vector3.Slerp(towards, targetTowards, 0.2f);

        ///right = new Vector3(towards.z, 0.0f, -towards.x).normalized;
        ///up = Vector3.Cross(towards, right);
    }


    //控制角色移动的操作
    public void Move(MoveDirection direction)
    {
        Move(direction, this.defaultSpeed);
    }
    public void Move(float speed)
    {
        Move(MoveDirection.Front, speed);
    }
    public void Move(MoveDirection direction,float speed)
    {
        //由移动方向决定球面插值的参数
        switch (direction)
        {
            case MoveDirection.Left:  targetLookAtSlerp = 0.0f; break;
            case MoveDirection.FrontLeft: targetLookAtSlerp = 0.25f;break;
            case MoveDirection.Front: targetLookAtSlerp = 0.5f; break;
            case MoveDirection.FrontRight: targetLookAtSlerp = 0.75f;break;
            case MoveDirection.Right: targetLookAtSlerp = 1.0f; break;
            default: targetLookAtSlerp = 0.5f;break;
        }
        //平滑过渡（角色的转向）
        if (Mathf.Abs(targetLookAtSlerp - currentLookAtSlerp) < 1e-6)
            currentLookAtSlerp = targetLookAtSlerp;
        else
            currentLookAtSlerp += (targetLookAtSlerp - currentLookAtSlerp) * 0.4f;


        Vector3 lookAt = Vector3.SlerpUnclamped(-right, towards, currentLookAtSlerp * 2);
        lookAt.Normalize();
        gameObject.transform.LookAt(gameObject.transform.position + lookAt);
        if (direction!=MoveDirection.Stay)
            character.Move(lookAt * speed * Time.deltaTime);
    }

    //控制角色的转向操作
    public void Turn(float yaw, float pitch)
    {
        if (yaw == 0 && pitch == 0) return;
        //ctrlX和ctrlY可置为1或-1，控制角色朝向的操控方式
        float ctrlX = 1.0f, ctrlY = 1.0f;

        right = Vector3.Cross(towards, up);
        towards = Vector3.SlerpUnclamped(towards, up, ctrlY * pitch / 90.0f);
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
        towards = Vector3.SlerpUnclamped(towards, right, ctrlX * yaw / 90.0f);
        towards.Normalize();
        right = new Vector3(towards.z, 0.0f, -towards.x).normalized;
        up = Vector3.Cross(towards, right);
        up.Normalize();

        targetTowards = towards;
    }

    public void TurnBack()
    {
        TurnBack(true, true, true);
    }
    public void TurnBack(bool turnX,bool turnY,bool turnZ)
    {
        if (turnX)
            targetTowards.x *= -1;
        if (turnY)
            targetTowards.y *= -1;
        if (turnZ)
            targetTowards.z *= -1;
    }

    public void SetForwardDirecion(Vector3 forward)
    {
        targetTowards = forward;
    }

    public Vector3 GetForwardDirection()
    {
        return towards;
    }

    

}
