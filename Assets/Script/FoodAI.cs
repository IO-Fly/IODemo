using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAI : MonoBehaviour {

    public List<Player> playerList;
    public float alertDistance = 12.0f;
    public float speed = 10.0f;

    private Vector3 towards;
    private float timeCount;
    private CharacterController character;
    
	// Use this for initialization
	void Start () {
        towards = GetRandomDirection();
        timeCount = 0.0f;
        character = gameObject.GetComponent<CharacterController>();
        ///speed = gameObject.GetComponent<Player>().GetSpeed();

        //TODO: 
        //playerList成员添加上所有场上的玩家角色

    }
	
	// Update is called once per frame
	void Update () {
        //检测警戒范围内是否有玩家角色，并找出距离最近的玩家角色
        timeCount -= Time.deltaTime;
        if (timeCount <= 0.0f)
        {
            timeCount = Random.Range(2.0f, 3.0f);

            Player closestPlayer = null;
            float currentDistance = Mathf.Infinity;
            Vector3 selfPosition = gameObject.transform.position;
            foreach (Player player in playerList)
            {
                Vector3 playerPosition = player.gameObject.transform.position;
                float distance = Vector3.Distance(playerPosition, selfPosition);
                if (distance < Mathf.Min(alertDistance, currentDistance))
                {
                    currentDistance = distance;
                    closestPlayer = player;
                }
            }
            //如果警戒范围内有玩家，AI选择远离最近玩家的方向作为移动方向
            if (closestPlayer != null)
            {
                Vector3 closestPlayerPosition = closestPlayer.gameObject.transform.position;
                Vector3 randomOffset = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
                towards = (selfPosition - closestPlayerPosition + randomOffset).normalized;
                transform.LookAt(selfPosition + towards);
            }
            //如果警戒范围内没有玩家，随机选择移动方向
            else
            {
                towards = GetRandomDirection();
                transform.LookAt(gameObject.transform.position + towards);
            }
        }
        //执行移动操作
        ///speed = gameObject.GetComponent<Player>().GetSpeed();
        character.Move(towards * speed * Time.deltaTime);
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
