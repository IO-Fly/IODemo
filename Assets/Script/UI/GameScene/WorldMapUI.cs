using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapUI : MonoBehaviour {
	private GameObject WorldMap;
	// Use this for initialization
	void Start () {
		WorldMap = GameObject.Find("MiniMapCanvas3D");
		WorldMap.SetActive(false);
		GameObject.Find("MiniMap").GetComponent<bl_MiniMap>().m_Target = networkManager.localPlayer; 
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.M)&&WorldMap!=null){
			if(WorldMap.GetActive()){
				WorldMap.SetActive(false);
			}
			else{
				WorldMap.SetActive(true);
			}
		}
	}
}
