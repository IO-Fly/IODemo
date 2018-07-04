using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour {
	private bool isActive = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Debug.Log("esc按下");
			if(isActive){
				this.gameObject.SetActive(false);
			}
			else{
				this.gameObject.SetActive(true);
			}
			
		}
	}
}
