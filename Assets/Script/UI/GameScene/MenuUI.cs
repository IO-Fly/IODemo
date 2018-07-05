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
				transform.Find("Menu").gameObject.SetActive(false);
				isActive = false;
			}
			else{
				transform.Find("Menu").gameObject.SetActive(true);
				isActive = true;
			}
			
		}
	}

}
