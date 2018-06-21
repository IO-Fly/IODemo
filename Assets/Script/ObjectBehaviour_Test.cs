using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour_Test : MonoBehaviour {


    public ObjectBehaviour testMod;
    public float timeCount;

	// Use this for initialization
	void Start () {
        timeCount = 0.0f;
        testMod = GetComponent<ObjectBehaviour>();	
	}
	
	// Update is called once per frame
	void Update () {
        ///TestTurn();
        timeCount -= Time.deltaTime;
        if (timeCount <= 0.0f)
        {
            timeCount = 2.0f;
            TestSetForward();
        }
        
        TestMove();
    }

    void TestMove()
    {
        //处理WSAD输入（改变角色的方向）
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        if (moveHorizontal > 0.0f)
            if (moveVertical > 0.0f)
                testMod.Move(ObjectBehaviour.MoveDirection.FrontRight);
            else
                testMod.Move(ObjectBehaviour.MoveDirection.Right);
        else if (moveHorizontal < 0.0f)
            if (moveVertical > 0.0f)
                testMod.Move(ObjectBehaviour.MoveDirection.FrontLeft);
            else
                testMod.Move(ObjectBehaviour.MoveDirection.Left);
        else if (moveVertical > 0.0f)
            testMod.Move(ObjectBehaviour.MoveDirection.Front);
        else
            testMod.Move(ObjectBehaviour.MoveDirection.Stay);
    }

    void TestTurn()
    {
        float mouseSensitivity = 3.0f;
        float mouseX = -Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        testMod.Turn(mouseX, mouseY);
    }

    void TestSetForward()
    {

        Vector3 randomVector = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        testMod.SetForwardDirecion(randomVector);
    }
}
