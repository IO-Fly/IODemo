using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : Photon.PunBehaviour{

    public enum Axis { X, Y, Z,None };
    public Axis verticalAxis=Axis.None;//表示边界所在的面与哪一个坐标轴垂直

    public bool blockFood = true;//边界是否限制可移动食物（AI）的移动
    public bool blockPlayerAI = true;//边界是否限制玩家角色（玩家或AI控制）的移动

    public enum ForceDirection { Positive, Negative };//在触发器内的玩家强制更改的方向
    public ForceDirection forceDirection=ForceDirection.Positive;
	void Start () {
	}

    void Update()
    {
        if(transform.parent!=null&&transform.parent.position.z > 100){
            ChangeTopEdgeAlpha();
        }
        if(transform.parent!=null&&transform.parent.position.z < -100){
            ChangeDownEdgeAlpha();
        }
        if(transform.parent!=null&&transform.parent.position.x > 100){
            ChangeRightEdgeAlpha();
        }
        if(transform.parent!=null&&transform.parent.position.x < -100){
            ChangeLeftEdgeAlpha();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        string tag = other.gameObject.tag;
        if ((tag == "playerCopy" && blockPlayerAI)
            || ((tag == "food" || tag == "AICollider"||tag=="foodAI") && blockFood))
        {
            ObjectBehaviour objectBehaviour;
            if (tag == "AICollider")
            {
                objectBehaviour = other.gameObject.GetComponentInParent<ObjectBehaviour>();
            }
            else
            {
                objectBehaviour = other.gameObject.GetComponent<ObjectBehaviour>();
            }
            if (objectBehaviour != null)
            {
                Vector3 direction = objectBehaviour.GetForwardDirection();
                bool positiveLimit = (forceDirection == ForceDirection.Positive);
                bool negativeLimit = (forceDirection == ForceDirection.Negative);
                float theta = Random.Range(0.0f, 2 * Mathf.PI);
                float phi = Random.Range(Mathf.PI / 3, Mathf.PI / 2);
                float n1 = Mathf.Cos(theta) * Mathf.Cos(phi);
                float n2 = Mathf.Sin(theta) * Mathf.Cos(phi);
                float n3 = Mathf.Sin(phi);
                if (forceDirection == ForceDirection.Negative) n3 *= -1;
                switch (verticalAxis)
                {
                    case Axis.X:
                        if ((positiveLimit && direction.x < 0.0f) || (negativeLimit && direction.x > 0.0f))
                            objectBehaviour.SetForwardDirecion(new Vector3(n3, n1, n2));
                        break;
                    case Axis.Y:
                        if ((positiveLimit && direction.y < 0.0f) || (negativeLimit && direction.y > 0.0f))
                            objectBehaviour.SetForwardDirecion(new Vector3(n2, n3, n1));
                        break;
                    case Axis.Z:
                        if ((positiveLimit && direction.z < 0.0f) || (negativeLimit && direction.z > 0.0f))
                            objectBehaviour.SetForwardDirecion(new Vector3(n1, n2, n3));
                        break;
                }
            }
        }
         /*  else if(tag == "player"&&other.gameObject.GetComponent<Player>().photonView.isMine){
            if (transform.parent != null)
                transform.parent.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }*/
    }
    
    /*private void OnTriggerExit(Collider other){
        string tag = other.gameObject.tag;
        if(tag == "player"&&other.gameObject.GetComponent<Player>().photonView.isMine){
            if(transform.parent!=null)
            transform.parent.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        
    }
    */

    private void ChangeTopEdgeAlpha(){
        Renderer render = transform.parent.gameObject.GetComponent<Renderer>();
        if(networkManager.localPlayer.transform.position.z>=0){
        render.material.color =new Color(render.material.color.r, render.material.color.g, render.material.color.b, 1 - (200-networkManager.localPlayer.transform.position.z)/200); 
        }
        else{
        render.material.color = new Color(render.material.color.r, render.material.color.g, render.material.color.b, 0); 
       
        }
    }
    private void ChangeDownEdgeAlpha(){
        Renderer render = transform.parent.gameObject.GetComponent<Renderer>();
        if(networkManager.localPlayer.transform.position.z<0){
        render.material.color =new Color(render.material.color.r, render.material.color.g, render.material.color.b, 1 - (networkManager.localPlayer.transform.position.z+200)/200); 
        }
        else{
        render.material.color = new Color(render.material.color.r, render.material.color.g, render.material.color.b, 0); 
       
        }
    }
    private void ChangeLeftEdgeAlpha(){
        Renderer render = transform.parent.gameObject.GetComponent<Renderer>();
        if(networkManager.localPlayer.transform.position.x<0){
        render.material.color =new Color(render.material.color.r, render.material.color.g, render.material.color.b, 1 - (networkManager.localPlayer.transform.position.x+200)/200); 
        }
        else{
        render.material.color = new Color(render.material.color.r, render.material.color.g, render.material.color.b, 0); 
       
        }
    }
    private void ChangeRightEdgeAlpha(){
        Renderer render = transform.parent.gameObject.GetComponent<Renderer>();
        if(networkManager.localPlayer.transform.position.x>=0){
        render.material.color =new Color(render.material.color.r, render.material.color.g, render.material.color.b, 1 - (200-networkManager.localPlayer.transform.position.x)/200); 
        }
        else{
        render.material.color = new Color(render.material.color.r, render.material.color.g, render.material.color.b, 0); 
       
        }
    }



}
