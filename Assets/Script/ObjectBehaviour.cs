using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour {

    public float defaultSpeed=20.0f;

    public enum MoveDirection { Left,FrontLeft,Front,FrontRight,Right,BackRight,Back,BackLeft,Stay};

    private Vector3 towards=new Vector3(0.0f,0.0f,1.0f);
    private Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
    private Vector3 right = new Vector3(1.0f, 0.0f, 0.0f);

    private Vector3 targetTowards = new Vector3(0.0f, 0.0f, 1.0f);

    private float currentLookAtSlerp = 0.5f;
    private float targetLookAtSlerp = 0.5f;

    protected delegate void MoveHandler(Vector3 offset);
    protected MoveHandler moveHandler;
    ///private CharacterController character;

    private void Awake()
    {
        moveHandler = null;
    }

    private void Start()
    {
        ///character = GetComponent<CharacterController>();
        if(moveHandler==null)
            SetMove(MoveWay.ChangePosition);
    }

    private void Update()
    {
        //平滑过渡
        if (Mathf.Abs((targetTowards - towards).magnitude) < 1e-6)
            towards = targetTowards;
        else
            towards = Vector3.Slerp(towards, targetTowards, 0.2f);
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
            case MoveDirection.Left: SetTargetSlerp(0.0f); break;
            case MoveDirection.FrontLeft: SetTargetSlerp(0.25f); break;
            case MoveDirection.Front: SetTargetSlerp(0.5f); break;
            case MoveDirection.FrontRight: SetTargetSlerp(0.75f); break;
            case MoveDirection.Right: SetTargetSlerp(1.0f); break;
            case MoveDirection.BackRight: SetTargetSlerp(1.25f); break;
            case MoveDirection.Back: SetTargetSlerp(1.5f); break;
            case MoveDirection.BackLeft: SetTargetSlerp(1.75f); break;
            default: SetTargetSlerp(0.5f); break;
        }
        //平滑过渡（角色的转向）
        if (Mathf.Abs(targetLookAtSlerp - currentLookAtSlerp) < 1e-6)
            currentLookAtSlerp = targetLookAtSlerp;
        else
            currentLookAtSlerp += (targetLookAtSlerp - currentLookAtSlerp) * 0.2f;


        Vector3 lookAt = Vector3.SlerpUnclamped(-right, towards, currentLookAtSlerp * 2);
        lookAt.Normalize();
        gameObject.transform.LookAt(gameObject.transform.position + lookAt);
        if (direction != MoveDirection.Stay)
            ///character.Move(lookAt * speed * Time.deltaTime);
            ///transform.position += lookAt * speed * Time.deltaTime;
            moveHandler(lookAt * speed * Time.deltaTime);
    }
    private void SetTargetSlerp(float slerp)
    {
        float x = 2.0f;
        float b = currentLookAtSlerp;
        float n = slerp;
        float k = Mathf.Floor((n - b) / x + 0.5f);
        currentLookAtSlerp = k * x + b;
        targetLookAtSlerp = slerp;
    }
    


    //控制角色的转向操作
    public void Turn(float yaw, float pitch)
    {
        if (towards != targetTowards) return;

        if (yaw == 0 && pitch == 0) return;
        //ctrlX和ctrlY可置为1或-1，控制角色朝向的操控方式
        float ctrlX = -1.0f, ctrlY = 1.0f;

        right = Vector3.Cross(up,towards);
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
        return targetTowards;
    }

    protected void MoveByChangePosition(Vector3 offset)
    {
        transform.position += offset;
    }
    protected void MoveWithCharacterController(Vector3 offset)
    {
        CharacterController ctrl = gameObject.GetComponent<CharacterController>();
        if (ctrl == null) return;
        ctrl.Move(offset);
    }

    public enum MoveWay { ChangePosition,ByCharacterController};
    public void SetMove(MoveWay move)
    {
        switch (move)
        {
            case MoveWay.ChangePosition:moveHandler = new MoveHandler(MoveByChangePosition); break;
            case MoveWay.ByCharacterController:moveHandler = new MoveHandler(MoveWithCharacterController); break;
        }
    }

}
