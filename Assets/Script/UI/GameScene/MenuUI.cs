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
	private GameObject Status;
	private GameObject Menu;
	// Use this for initialization

	void Awake(){
	}
	void Start () {
		//GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Status").gameObject.GetComponent<Image>().sprite = this.pause;
		Status = GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Status").gameObject;
		Menu = GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject;
		Status.GetComponent<Image>().sprite = this.pause;
	}
	
	// Update is called once per frame
	void Update () {
		if(!isActive){
			Cursor.visible = false;
		}
		else{
			Cursor.visible = true;
		}
		if(Input.GetKeyDown(KeyCode.Escape)&&freeze!=true){
			Debug.Log("esc按下");
			if(isActive){
				//GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(false);
				Menu.SetActive(false);
				isActive = false;
			}
			else {
			//GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(true);
				Menu.SetActive(true);
				isActive = true;
			}
			
		}
		Debug.Log("freeze状态"+ freeze);
		if(freeze){
			//GameObject.Find("HUDCanvas").transform.Find("Menu").Find("Continue").gameObject.SetActive(false);
			//GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(true);
			Menu.transform.Find("Continue").gameObject.SetActive(false);
			Menu.SetActive(true);
			isActive = true;	
		}
				
	}

	public void ContinueGame(){
		//GameObject.Find("HUDCanvas").transform.Find("Menu").gameObject.SetActive(false);
		Menu.SetActive(false);
		isActive = false;
	}

	public void ExitGame(){
		Application.Quit();
	}

	public void BackLobby(){
		FoodManager.localFoodManager = null;
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.Disconnect();
		//SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GameScene"));
		//SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        GameStoryVideoPlay.isSkipVideo = true;
		SceneManager.LoadScene(0);

	}

}
