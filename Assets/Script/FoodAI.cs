using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodAI : PoisonAI {

    public static List<Player> allPlayers;
    public float playerDetectDistance = 50.0f;
    
    public float targetResetInv = 5.0f;
    public float directionResetInv = 1.0f;

    protected List<GameObject> playersDetected = new List<GameObject>();
    protected Player targetPlayer = null;
    protected float directionResetCount = 0.0f;
    protected float targetResetCount = 0.0f;
 
    // Use this for initialization
    void Start() {
        character = gameObject.GetComponent<CharacterController>();
        objectBehaviour = gameObject.GetComponent<ObjectBehaviour>();
        //设置一个随机的初始方向
        objectBehaviour.SetForwardDirecion(GetRandomDirection());
        resetCountForWander = 1.0f;
    }

    // Update is called once per frame
    void Update() {
        targetResetCount -= Time.deltaTime;
        if (targetResetCount <= 0.0f)
        {
            targetResetCount = targetResetInv;

            DetectPlayers();
            SetTargetPlayer();
        }
        if (HasTargetPlayer())
        {
            Flee();
        }
        else
        {
            Wander();
        }
        
    }



    protected virtual void DetectPlayers()
    {
        DetectPlayers(playerDetectDistance);
    }
    protected virtual void DetectPlayers(float detectDistance)
    {
        playersDetected.Clear();
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, playerDetectDistance);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("player"))
                playersDetected.Add(collider.gameObject);
        }
    }

    protected void SetTargetPlayer()
    {
        float rand;
        targetPlayer = null;
        for (int i = 0; i < playersDetected.Count; ++i)
        {
            rand = Random.Range(0.0f, 1.0f);
            if (rand < 0.8f)
            {
                targetPlayer = playersDetected[i].gameObject.GetComponent<Player>();
                break;
            }
        }
    }
    protected void SetTargetPlayer(Player player)
    {
        targetPlayer = player;
    }

    protected bool HasTargetPlayer()
    {
        return targetPlayer != null;
    }

    protected void Flee()
    {
        if (targetPlayer != null)
        {
            MoveTowards(transform.position * 2 - targetPlayer.gameObject.transform.position);
        }
        else
        {
            Wander();
        }
    }

    


    protected void MoveTowards(Vector3 targetPosition,float speed)
    {
        directionResetCount -= Time.deltaTime;
        if (directionResetCount <= 0.0f)
        {
            directionResetCount = directionResetInv;
            Vector3 direction = targetPosition - gameObject.transform.position;
            direction.Normalize();
            objectBehaviour.SetForwardDirecion(direction);
        }
        objectBehaviour.Move(ObjectBehaviour.MoveDirection.Front, speed);
    }

    protected void MoveTowards(Vector3 targetPosition)
    {
        MoveTowards(targetPosition, speed);
    }

    


}
