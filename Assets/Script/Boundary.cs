using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : Photon.PunBehaviour{

    public enum Axis { X, Y, Z,None };
    public Axis verticalAxis=Axis.None;//表示边界所在的面与哪一个坐标轴垂直

    public bool blockFood = true;//边界是否限制可移动食物（AI）的移动
    public bool blockPlayer = true;//边界是否限制玩家角色（玩家或AI控制）的移动

    public enum ForceDirection { Positive, Negative };//在触发器内的玩家强制更改的方向
    public ForceDirection forceDirection=ForceDirection.Positive;

	void Start () {

	}

    void Update()
    {
        
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if ((collision.gameObject.tag == "player" && blockPlayer)
    //        || (collision.gameObject.tag == "food" && blockFood))
    //    {
    //        ObjectBehaviour objectBehaviour = collision.gameObject.GetComponent<ObjectBehaviour>();
    //        if (objectBehaviour != null)
    //        {
    //            switch (verticalAxis)
    //            {
    //                case Axis.X: objectBehaviour.TurnBack(true, false, false); break;
    //                case Axis.Y: objectBehaviour.TurnBack(false, true, false); break;
    //                case Axis.Z: objectBehaviour.TurnBack(false, false, true); break;
    //                default: objectBehaviour.TurnBack(); break;
    //            }
    //        }
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag=="player" && blockPlayer)
            || (other.gameObject.tag=="food" && blockFood)) 
        {
            ObjectBehaviour objectBehaviour = other.gameObject.GetComponent<ObjectBehaviour>();
            if (objectBehaviour != null)
            {
                //float theta = Random.Range(0.0f, 2 * Mathf.PI);
                //float phi = Random.Range(Mathf.PI / 3, Mathf.PI / 2);
                //float n1 = Mathf.Cos(theta) * Mathf.Cos(phi);
                //float n2 = Mathf.Sin(theta) * Mathf.Cos(phi);
                //float n3 = Mathf.Sin(phi);
                //if (forceDirection == ForceDirection.Negative) n3 *= -1;
                Vector3 randomDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                switch (verticalAxis)
                {
                    //case Axis.X:objectBehaviour.SetForwardDirecion(new Vector3(n3, n1, n2));break;
                    //case Axis.Y:objectBehaviour.SetForwardDirecion(new Vector3(n2, n3, n1));break;
                    //case Axis.Z:objectBehaviour.SetForwardDirecion(new Vector3(n1, n2, n3));break;
                    case Axis.X: objectBehaviour.SetForwardDirecion(randomDirection); break;
                    case Axis.Y: objectBehaviour.SetForwardDirecion(randomDirection); break;
                    case Axis.Z: objectBehaviour.SetForwardDirecion(randomDirection); break;
                    //case Axis.X: objectBehaviour.TurnBack(true, false, false); break;
                    //case Axis.Y: objectBehaviour.TurnBack(false, true, false); break;
                    //case Axis.Z: objectBehaviour.TurnBack(false, false, true); break;
                    default: objectBehaviour.TurnBack(); break;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.tag == "player" && blockPlayer)
            || (other.gameObject.tag == "food" && blockFood))
        {
            ObjectBehaviour objectBehaviour = other.gameObject.GetComponent<ObjectBehaviour>();
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
    }
}
