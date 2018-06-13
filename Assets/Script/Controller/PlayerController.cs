using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float mouseSensitivity = 1.0f;

    private Vector3 towards;//角色朝向的方向
    private Vector3 up;//向上向量
    private Vector3 right;
    public float speed;

    public bool fly = false;
    public bool drop = false;
    public float height = 5;
    public float gravity = 0.001f;

    private enum MOVE_DIRECTION { FRONT,FRONT_LEFT,FRONT_RIGHT, LEFT, RIGHT };
    private MOVE_DIRECTION moveDirection;
    public  CharacterController _character;
    // Use this for initialization
    void Start () {
        speed = gameObject.GetComponent<Player>().speed;
        towards = new Vector3(0.0f, 0.0f, 1.0f);
        up = new Vector3(0.0f, 1.0f, 0.0f);
        right = new Vector3(1.0f, 0.0f, 0.0f);
        moveDirection = MOVE_DIRECTION.FRONT;
	}
	
	// Update is called once per frame
	void Update () {
		
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

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 lookAt = new Vector3();
        if (moveHorizontal > 0.0f)
            if (moveVertical > 0.0f)
                moveDirection = MOVE_DIRECTION.FRONT_RIGHT;
            else
                moveDirection = MOVE_DIRECTION.RIGHT;
        else if (moveHorizontal < 0.0f)
            if (moveVertical > 0.0f)
                moveDirection = MOVE_DIRECTION.FRONT_LEFT;
            else
                moveDirection = MOVE_DIRECTION.LEFT;
        else if (moveVertical >= 0.0f)
            moveDirection = MOVE_DIRECTION.FRONT;
        switch (moveDirection)
        {
            case MOVE_DIRECTION.FRONT:
                lookAt = towards;
                break;
            case MOVE_DIRECTION.FRONT_LEFT:
                lookAt = Vector3.Slerp(towards, -right, 0.5f);
                break;
            case MOVE_DIRECTION.LEFT:
                lookAt = -right;
                break;
            case MOVE_DIRECTION.FRONT_RIGHT:
                lookAt = Vector3.Slerp(towards, right, 0.5f);
                break;
            case MOVE_DIRECTION.RIGHT:
                lookAt = right;
                break;
        }
        transform.LookAt(gameObject.transform.position + lookAt);

        if (moveVertical < 0.0f)
            moveVertical = 0.0f;
        Vector3 move = towards * moveVertical + right * moveHorizontal;
       if(fly){
        move.y = 0;
        if(this.gameObject.transform.position.y < height&&drop==false){
            move.y +=0.5f;
        }
        else{
            drop =true;
            move.y-=0.5f;
        }
       }
        _character.Move(move*speed);

    }

    public Vector3 GetTowards()
    {
        return towards;
    }
}
