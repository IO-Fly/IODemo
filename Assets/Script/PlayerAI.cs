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
        character = gameObject.GetComponent<CharacterController>();
        objectBehaviour = gameObject.GetComponent<ObjectBehaviour>();
        player = gameObject.GetComponent<Player>();
        playerBehaviour = gameObject.GetComponent<PlayerBehaviour>();
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
            if (BiggerThanTarget())
                ChaseTarget();
            else
                Flee();
        }
        else if (HasTargetFood())
            GoGetFood();
        else
            Wander();
    }

    void ChaseTarget()
    {
        if (targetPlayer != null)
        {
            if (InTheSky())
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
}


 ////public class PlayerAI : MonoBehaviour {



 ////   public List<Player> playerList;

 ////   public bool EnableEatFood = true;
 ////   public bool EnableAwayFromPlayer = true;
 ////   public bool EnableAttackPlayer = true;

 ////   public float playerDetectDistance = 50.0f;
 ////   public float foodDetectDistance = 20.0f;
 ////   public float angleOfView = 45.0f;
 ////   public float targetResetIntervalMin = 2.0f;
 ////   public float targetResetIntervalMax = 3.0f;
 ////   private Player selfPlayer;
 ////   private Player targetPlayer;
 ////   private GameObject targetFood;
 ////   private float timeCount;
 ////   private float timeCount2;
 ////   private CharacterController characterController;
 ////   private float speed;
 ////   private Vector3 towards;
 ////   private Vector3 targetPosition;
 ////   private Vector3 direction;
 ////   private float angle;

	////// Use this for initialization
	////void Start () {
       
 ////       timeCount = 0.0f;
 ////       timeCount2 = 0.0f;
 ////       selfPlayer = gameObject.GetComponent<Player>();
 ////       characterController = gameObject.GetComponent<CharacterController>();
        
	////}
	
	////// Update is called once per frame
	////void Update () {
 ////       Vector3 selfPosition = gameObject.transform.position;

 ////       timeCount -= Time.deltaTime;
 ////       if (timeCount <= 0.0f)
 ////       {
 ////           timeCount = Random.Range(targetResetIntervalMin, targetResetIntervalMax);

 ////           targetPlayer = null;
 ////           targetFood = null;
 ////           float currentDistance = Mathf.Infinity;
 ////           if (EnableAttackPlayer || EnableAwayFromPlayer)
 ////           {
 ////               foreach(Player player in playerList)
 ////               {
 ////                   Vector3 playerPosition = player.gameObject.transform.position;
 ////                   direction = (playerPosition - selfPosition).normalized;
 ////                   angle = Vector3.Angle(towards, direction);
 ////                   float distance = Vector3.Distance(playerPosition, selfPosition);
 ////                   if (angle < angleOfView && distance < Mathf.Min(playerDetectDistance, currentDistance)) 
 ////                   {
 ////                       currentDistance = distance;
 ////                       targetPlayer = player;
 ////                   }
 ////               }
 ////           }
 ////           if (EnableEatFood && targetPlayer == null)
 ////           {
 ////               Collider[] foodColliders = Physics.OverlapSphere(selfPosition, foodDetectDistance);
 ////               foreach (Collider collider in foodColliders)
 ////               {
 ////                   if (collider.gameObject.CompareTag("food"))
 ////                   {
 ////                       Vector3 foodPosition = collider.gameObject.transform.position;
 ////                       direction = (foodPosition - selfPosition).normalized;
 ////                       angle = Vector3.Angle(towards, direction);
 ////                       float distance = Vector3.Distance(selfPosition, foodPosition);
 ////                       if (angle<angleOfView && distance < currentDistance) 
 ////                       {
 ////                           targetFood = collider.gameObject;
 ////                           currentDistance = distance;
 ////                       }
 ////                   }
 ////               }

 ////           }

 ////           if (targetPlayer == null && targetFood != null) 
 ////           {
 ////               targetPosition = targetFood.gameObject.transform.position;
 ////               towards = (targetPosition - selfPosition).normalized;
 ////           }
 ////           else
 ////           {
 ////               towards = GetRandomDirection();
 ////           }
 ////           transform.LookAt(selfPosition + towards);
 ////       }

 ////       if (targetPlayer != null)
 ////       {
 ////           targetPosition = targetPlayer.gameObject.transform.position;
 ////           if (GetComponent<Player>().GetPlayerSize() >= targetPlayer.GetComponent<Player>().GetPlayerSize())
 ////           {
 ////               if (EnableAttackPlayer)
 ////               {
 ////                   towards = (targetPosition - selfPosition).normalized;
 ////               }
 ////           }
 ////           else
 ////           {
 ////               if (EnableAwayFromPlayer)
 ////               {
 ////                   towards = (selfPosition - targetPosition).normalized;
 ////               }
 ////           }

 ////           transform.LookAt(selfPosition + towards);
 ////       }
        
 ////       speed = selfPlayer.GetSpeed();
 ////       characterController.Move(towards * speed * Time.deltaTime);
 ////   }

 ////   //随机方向，“俯仰角”限制在45度以内
 ////   private Vector3 GetRandomDirection()
 ////   {
 ////       float theta = Random.Range(0.0f, 2 * Mathf.PI);
 ////       float phi = Random.Range(-Mathf.PI / 4, Mathf.PI / 4);
 ////       float x = Mathf.Cos(theta) * Mathf.Cos(phi);
 ////       float z = Mathf.Sin(theta) * Mathf.Cos(phi);
 ////       float y = Mathf.Sin(phi);
 ////       return new Vector3(x, y, z);
 ////   }
 ////}
