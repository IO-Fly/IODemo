using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinimapUI : MonoBehaviour {

    public  GameObject sphereForMinimapPrefab;
    private GameObject sphereForMinimap;

    private void Start ()
    {
        // 初始化 sphereForMinimap
        sphereForMinimap = GameObject.Instantiate(sphereForMinimapPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
        //sphereForMinimap.transform.SetParent(this.transform, false);

        // 初始化 sphereForMinimap的材质，即颜色
        Material materialForMinimap;
        if( this.GetComponent<Player>().photonView.isMine )
        {
            materialForMinimap = Resources.Load("Materials/Minimap_materials/blue") as Material;
        }
        else
        {
            materialForMinimap = Resources.Load("Materials/Minimap_materials/red") as Material;
        }
        sphereForMinimap.GetComponent<Renderer>().material = materialForMinimap;

	}

    private void Update()
    {
        // 更新 sphereForMinimap的位置
        sphereForMinimap.transform.position = this.transform.position + new Vector3(0.0f, 2.0f, 0.0f);
        
    }

    private void OnDestroy()
    {
        Destroy(sphereForMinimap);
    }

}
