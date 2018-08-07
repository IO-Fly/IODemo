using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSkillAI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(useSkill());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator useSkill()
    {
        while (true)
        {
            if (GetComponent<PlayerSpeedController>().SkillInUse())
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(Random.Range(2, 5));
            GetComponent<PlayerSpeedController>().useSkill();
            yield return new WaitForSeconds(5);
        }
    }
}
