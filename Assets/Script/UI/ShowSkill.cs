using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSkill : MonoBehaviour {

    private GameObject player;
    private Text coolTimeText;

    void Start ()
    {
        coolTimeText = this.transform.Find("SkillImage/SkillTimeText").gameObject.GetComponent<Text>();

        //Debug.Log(player.name);
    }
	
	void Update ()
    {
        
        coolTimeText.text = Mathf.Ceil(player.GetComponent<PlayerSizeController>().GetCurCoolTime()).ToString();
	}

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }
}
