using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAI : MonoBehaviour {

    public float speed = 10.0f;

    protected float resetCountForWander = 0.0f;
    protected float turnHorizontal = 0.0f;
    protected float turnVertical = 0.0f;

    protected CharacterController character;
    protected ObjectBehaviour objectBehaviour;

    // Use this for initialization
    void Start () {
        character = gameObject.GetComponent<CharacterController>();
        objectBehaviour = gameObject.GetComponent<ObjectBehaviour>();
        //设置一个随机的初始方向
        objectBehaviour.SetForwardDirecion(GetRandomDirection());
        resetCountForWander = 1.0f;
    }
	
	// Update is called once per frame
	void Update () {
        Wander();
	}

    protected void Wander()
    {
        resetCountForWander -= Time.deltaTime;
        if (resetCountForWander <= 0.0f)
        {
            float p = Random.Range(0.0f, 1.0f);
            if (p < 0.8f)
            {
                turnHorizontal = Random.Range(-0.3f, 0.3f);
                turnVertical = Random.Range(-0.1f, 0.1f);
                resetCountForWander = Random.Range(2.0f, 5.0f);
            }
            else
            {
                Vector3 dir = GetRandomDirection();
                objectBehaviour.SetForwardDirecion(dir);
                turnHorizontal = 0.0f;
                turnVertical = 0.0f;
                resetCountForWander = 0.1f;
            }
        }
        objectBehaviour.Turn(turnHorizontal, turnVertical);
        objectBehaviour.Move(ObjectBehaviour.MoveDirection.Front, speed);
    }

    //随机方向，“俯仰角”限制在45度以内
    protected Vector3 GetRandomDirection()
    {
        float theta = Random.Range(0.0f, 2 * Mathf.PI);
        float phi = Random.Range(-Mathf.PI / 4, Mathf.PI / 4);
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float z = Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = Mathf.Sin(phi);
        return new Vector3(x, y, z).normalized;
    }
}
