using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinimapUI : MonoBehaviour {

    private GameObject sphereForMinimap;

    void Start ()
    {
        sphereForMinimap = this.transform.Find("SphereForMinimap").gameObject;

        Material materialForMinimap;
        if( this.GetComponent<Player>().photonView.isMine )
        {
            materialForMinimap = Resources.Load("Minimap_materials/blue") as Material;
        }
        else
        {
            materialForMinimap = Resources.Load("Minimap_materials/red") as Material;
        }

        sphereForMinimap.GetComponent<Renderer>().material = materialForMinimap;
	}
	
}
