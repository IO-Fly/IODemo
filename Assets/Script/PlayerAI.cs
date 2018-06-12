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

    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.5f, 0.5f), Random.Range(-1.0f, 1.0f)).normalized;

    }
}
