using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour {

    private Vector3 towards;
    private float timeCount;
    private CharacterController character;
    private float speed;
	// Use this for initialization
	void Start () {
        towards = GetRandomDirection();
        timeCount = 0.0f;
        character = gameObject.GetComponent<CharacterController>();
        speed = gameObject.GetComponent<Player>().GetSpeed();
    }
	
	// Update is called once per frame
	void Update () {
        timeCount -= Time.deltaTime;
        if (timeCount <= 0.0f)
        {
            towards = GetRandomDirection();
            timeCount = Random.Range(2.0f, 3.0f);
            transform.LookAt(gameObject.transform.position + towards);
        }

        speed = gameObject.GetComponent<Player>().GetSpeed();
        character.Move(towards * Time.deltaTime * speed);
    }

    //随机方向，“俯仰角”限制在45度以内
    private Vector3 GetRandomDirection()
    {
        float theta = Random.Range(0.0f, 2 * Mathf.PI);
        float phi = Random.Range(-Mathf.PI / 4, Mathf.PI / 4);
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float z = Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = Mathf.Sin(phi);
        return new Vector3(x, y, z);
    }
}
