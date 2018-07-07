using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuUI : MonoBehaviour {
	private bool isActive = false;
	public bool freeze = false;
	public Sprite lose;
	public Sprite win;
	public Sprite pause;
	// Use this for initialization
	void Awake(){
	}
	void Start () {
		GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Status").gameObject.GetComponent<Image>().sprite = this.pause;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)&&freeze!=true){
			Debug.Log("esc按下");
			if(isActive){
				GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(false);
				isActive = false;
			}
			else {
			GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(true);
				isActive = true;
			}
			
		}
		Debug.Log("freeze状态"+ freeze);
		if(freeze){
			GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Continue").gameObject.SetActive(false);
			GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(true);
			isActive = true;	
		}
				
	}

	public void ContinueGame(){
		GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(false);
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
