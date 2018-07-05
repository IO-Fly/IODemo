using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuUI : MonoBehaviour {
	private bool isActive = false;
	// Use this for initialization
	void Awake(){
		
	}
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
			else {
				transform.Find("Menu").gameObject.SetActive(true);
				isActive = true;
			}
			
		}
	}

	public void ContinueGame(){
		transform.Find("Menu").gameObject.SetActive(false);
		isActive = false;
	}

	public void ExitGame(){
		Application.Quit();
	}

	public void BackLobby(){
		FoodManager.localFoodManager = null;
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.Disconnect();
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
		SceneManager.LoadScene(0);

	}

}
