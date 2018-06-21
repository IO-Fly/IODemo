using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FoodAI : PoisonAI {

    public static List<Player> allPlayers;
    public float playerDetectDistance = 50.0f;
    
    public float targetResetInv = 5.0f;
    public float directionResetInv = 2.0f;

    protected List<Player> playersDetected = new List<Player>();
    protected Player targetPlayer = null;
    protected float directionResetCount = 0.0f;
    protected float targetResetCount = 0.0f;
 
    // Use this for initialization
    void Start() {
        character = gameObject.GetComponent<CharacterController>();
        objectBehaviour = gameObject.GetComponent<ObjectBehaviour>();
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



    protected void DetectPlayers()
    {
        DetectPlayers(playerDetectDistance);
    }
    protected void DetectPlayers(float detectDistance)
    {
        playersDetected.Clear();
        Vector3 selfPosition = gameObject.transform.position;
        if (allPlayers != null)
        {
            foreach (Player player in allPlayers)
            {
                Vector3 playerPosition = player.gameObject.transform.position;
                float distance = Vector3.Distance(playerPosition, selfPosition);
                Vector3 playerDirection = (playerPosition - selfPosition).normalized;
                Vector3 forwardDirection = gameObject.transform.forward;
                if (distance < detectDistance && Vector3.Angle(forwardDirection,playerDirection)<70.0f)
                {
                    playersDetected.Add(player);
                }
            }
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
                targetPlayer = playersDetected[i];
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
