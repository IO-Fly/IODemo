using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.PunBehaviour {

    
    public float initialSize = 1.0f;
    public float initialSpeed = 0.7f;
    public float health;

    private float playerEnergy;
    private Vector3 playerSize;

    private float speed;

    //Debug
    public GameObject other;

    // Use this for initialization
    void Start () {
        playerEnergy = initialSize * initialSize;
        playerSize = new Vector3(initialSize, initialSize, initialSize);
        speed = initialSpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if(photonView.isMine && other != null)
        {
            Debug.Log("实时更新当前血量： " + health);
            Debug.Log("实时更新对方血量： " + other.gameObject.gameObject.GetComponent<Player>().health);
        }

    }

     void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("food"))
        {
			Debug.Log ("玩家：碰到了食物");
            playerEnergy+=0.8f; 
            float sq=Mathf.Sqrt(playerEnergy);
            speed = initialSpeed / sq;
            playerSize = new Vector3(sq, sq, sq);
            transform.localScale = playerSize;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.gameObject != this.gameObject && other.gameObject.tag == "player" && this.photonView.isMine)
        {
            Debug.Log("碰撞到了玩家");
            this.photonView.RPC("GetDamage", PhotonTargets.AllViaServer, other.gameObject.transform.localScale.x * 5);
            other.gameObject.GetComponent<Player>().photonView.RPC("GetDamage",
                PhotonTargets.AllViaServer, other.gameObject.transform.localScale.x * 5);

            //Debug
            this.other = other.gameObject;
        }
    }


    public float GetSpeed()
    {
        return speed;
    }


    public Vector3 GetPlayerSize()
    {
        return playerSize;
    }
    [PunRPC]
    void DestroyThis(){
        PhotonNetwork.Destroy(this.gameObject);
    }

    [PunRPC]
    void GetDamage(float damage){
        health -= damage;
        if (health < 0)
        {   
            this.photonView.RPC("DestroyThis", PhotonTargets.AllViaServer);
        }
    }

}
