using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : FoodAI
{
    public float foodDetectDistance = 50.0f;

    private List<GameObject> FoodsDetected = new List<GameObject>();
    private GameObject targetFood = null;

    private Player player;
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        player = gameObject.GetComponent<Player>();
        character = gameObject.GetComponent<CharacterController>();
        objectBehaviour = gameObject.GetComponent<ObjectBehaviour>();
        playerBehaviour = gameObject.GetComponent<PlayerBehaviour>();

        StartCoroutine(CheckObstacle());
    }

    void Update()
    {
        targetResetCount -= Time.deltaTime;
        if (targetResetCount <= 0.0f){
            targetResetCount = targetResetInv;

            DetectPlayers();
            SetTargetPlayer();
            if (!HasTargetPlayer())
            {
                DetectFoods();
                SetTargetFood();
            }
        }
        if (HasTargetPlayer())
        {
            if (SmallerThanTarget())
                Flee();
            else
                ChaseTarget();
        }
        else if (HasTargetFood())
            GoGetFood();
        else
            Wander();
    }

    protected override void DetectPlayers()
    {
        this.DetectPlayers(playerDetectDistance);
    }
    protected override void DetectPlayers(float detectDistance)
    {
        playersDetected.Clear();
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, playerDetectDistance);
        foreach (Collider collider in colliders)
        {
            PlayerCopyController copyJudge = collider.gameObject.GetComponent<PlayerCopyController>();
            if (copyJudge != null)
                if (copyJudge.getPlayerCopy() == gameObject)
                    continue;
            //if (player.isCopyRelation(collider.gameObject))
            //    continue;
            if (collider.gameObject.CompareTag("player"))
                playersDetected.Add(collider.gameObject);
        }
    }

    void ChaseTarget()
    {
        if (targetPlayer != null)
        {
            if (!InTheSky())
                MoveTowards(targetPlayer.gameObject.transform.position);
        }
        else
        {
            Wander();
        }
    }

    void DetectFoods()
    {
        FoodsDetected.Clear();
        Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, foodDetectDistance);
        foreach(Collider collider in colliders)
        {
            if(collider.gameObject.CompareTag("food"))
                FoodsDetected.Add(collider.gameObject);
        }
    }

    void SetTargetFood()
    {
        float rand;
        targetFood = null;
        for(int i = 0; i < FoodsDetected.Count; ++i)
        {
            rand = Random.Range(0.0f, 1.0f);
            if (rand < 0.5f)
            {
                targetFood = FoodsDetected[i];
                break;
            }
        }
    }

    bool HasTargetFood()
    {
        return targetFood != null;
    }
    
    void GoGetFood()
    {
        if (targetFood != null)
        {
            MoveTowards(targetFood.gameObject.transform.position);
        }
        else
        {
            Wander();
        }
    }

    bool BiggerThanTarget()
    {
        return player.GetPlayerSize() > targetPlayer.gameObject.GetComponent<Player>().GetPlayerSize();
    }

    bool SmallerThanTarget()
    {
        return player.GetPlayerSize() < targetPlayer.gameObject.GetComponent<Player>().GetPlayerSize();
    }

    bool InTheSky()
    {
        return playerBehaviour.enterSky;
    }

    void MoveTowardsInTheSky(Vector3 targetPosition)
    {
        directionResetCount -= Time.deltaTime;
        if (directionResetCount <= 0.0f)
        {
            directionResetCount = directionResetInv;
            Vector3 direction = targetPosition - gameObject.transform.position;
            direction.Normalize();
            playerBehaviour.MoveInSky(direction);
        }
        
    }

    protected override void HandleCheckObstacle()
    {
        base.HandleCheckObstacle();
        targetFood = null;
    }
}