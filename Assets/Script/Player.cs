using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Photon.PunBehaviour {

  

    public float initialSize = 1.0f;
    public float initialSpeed = 20.0f;
    public float health;


    private float playerEnergy;
    private Vector3 playerSize;

    private float sizeEffect;//用于本地视口玩家角色大小显示效果
    private float speedOffset;//用于暂时性的增加速度
    private Vector3 sizeOffset;//用于暂时性的增加体型

    private float speed;
    private string playerName;//玩家自定义的名字

    //Debug
    public GameObject other;

    // Use this for initialization
    void Start () {
        playerEnergy = initialSize * initialSize;
        playerSize = new Vector3(initialSize, initialSize, initialSize);
        transform.localScale = playerSize;
        speed = initialSpeed;
        sizeEffect = 1.0f;
        speedOffset = 0.0f;
        sizeOffset = Vector3.zero;

    }
	
	// Update is called once per frame
	void FixedUpdate () {


        if (photonView.isMine && other != null)
        {
            Debug.Log("实时更新当前血量： " + health);
            Debug.Log("实时更新对方血量： " + other.gameObject.gameObject.GetComponent<Player>().health);
        }
        if (health < 0)
        {
            this.photonView.RPC("DestroyThis", PhotonTargets.AllViaServer);
        }
    }

     void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("food"))
        {
			Debug.Log ("玩家：碰到了食物");
            if(transform.localScale.x<25){
            playerEnergy+=0.2f; 
            float sq=Mathf.Sqrt(playerEnergy);     
            speed = 10 / sq+2;
            playerSize = new Vector3(playerEnergy, playerEnergy, playerEnergy);
            transform.localScale = playerSize + sizeOffset;
            }
        }
        if(other.gameObject.tag=="poison"){
            Debug.Log("玩家：碰到了毒物");
            playerEnergy-=0.5f; 
            float sq=Mathf.Sqrt(playerEnergy);     
            speed = 10 / sq+2;
            playerSize = new Vector3(playerEnergy, playerEnergy, playerEnergy);
            transform.localScale = playerSize + sizeOffset;
 
        }
    }

    void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject != this.gameObject && other.gameObject.tag == "player" && this.photonView.isMine)
        {
            Debug.Log("碰撞到了玩家");
            this.photonView.RPC("GetDamage", PhotonTargets.AllViaServer, other.gameObject.transform.localScale.x * 5);
            other.gameObject.GetComponent<Player>().photonView.RPC("GetDamage",
                PhotonTargets.AllViaServer, this.gameObject.transform.localScale.x * 5);

            //Debug
            this.other = other.gameObject;
        }
    }


    public void AddSpeedOffset(float speedOffset)
    {
        this.speedOffset += speedOffset;    
        Debug.Log("速度改变: "+ speedOffset);
    }

    public void AddSizeEffect(float sizeEffect)
    {
        this.sizeEffect *= sizeEffect;
    }

    public void AddSizeOffset(Vector3 sizeOffset)
    {
        this.sizeOffset += sizeOffset;
        transform.localScale = playerSize + this.sizeOffset;
    }

    public float GetSpeed()
    {
        return speed + speedOffset;
    }


    public Vector3 GetRenderPlayerSize()
    {
        if (sizeEffect != 0)
        {
            return (transform.localScale) / sizeEffect;
        }
        else
        {
            return Vector3.positiveInfinity;
        }

    }

    public void SetPlayerName(string name)
    {
        this.playerName = name;
    }
    public string GetPlayerName()
    {
        return this.playerName;
    }


    [PunRPC]
    void DestroyThis(){
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    void GetDamage(float damage){
        health -= damage;  
    }

}
