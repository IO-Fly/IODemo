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

    private float speed;

    //Debug
    public GameObject other;

    // Use this for initialization
    void Start () {
        playerEnergy = initialSize * initialSize;
        playerSize = new Vector3(initialSize, initialSize, initialSize);
        transform.localScale = playerSize;
        speed = initialSpeed;
        sizeEffect = 1.0f;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if(photonView.isMine && other != null)
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
            transform.localScale = playerSize;
            }
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


    public void AddSpeed(float addSpeed)
    {
        speed += addSpeed; 
        Debug.Log("速度改变: "+ addSpeed);
    }

    public float GetSpeed()
    {
        return speed;
    }


    public Vector3 GetRenderPlayerSize()
    {
        if(sizeEffect != 0)
        {
            return playerSize / sizeEffect;
        }
        else
        {
            return Vector3.positiveInfinity; 
        }
        
    }

    public void SetSizeEffect(float effect)
    {
        sizeEffect = effect;
    }

    public void AddPlayerSize(Vector3 addSize)
    {
        playerSize += addSize;
        transform.localScale = playerSize;
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
