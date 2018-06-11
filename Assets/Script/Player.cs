using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.PunBehaviour {

    
    public float initialSize = 1.0f;
    public float speed;
    public float health;

    private float playerEnergy;
    private Vector3 playerSize;
    

    // Use this for initialization
    void Start () {
        playerEnergy = initialSize * initialSize;
        playerSize = new Vector3(initialSize, initialSize, initialSize);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
    }

     void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("food"))
        {
			Debug.Log ("玩家：碰到了食物");
            playerEnergy+=0.8f; 
            float sq=Mathf.Sqrt(playerEnergy);
            gameObject.GetComponent<PlayerController>().speed = 0.7f / sq;
            playerSize = new Vector3(sq, sq, sq);
            transform.localScale = playerSize;
        }
        if(other.gameObject.tag == "player"/*&&this.photonView.isMine*/){
            Debug.Log("碰撞到了玩家");
            health -= other.gameObject.transform.localScale.x * 5;
            Debug.Log("当前血量： "+health);
        }
        if(health<0){
            //PhotonView.Destroy(this.gameObject);
            this.photonView.RPC("DestroyThis",PhotonTargets.All);
        }
    }

 

    public Vector3 GetPlayerSize()
    {
        return playerSize;
    }
    [PunRPC]
    void DestroyThis(){
        PhotonView.Destroy(this.gameObject);
    }

}
