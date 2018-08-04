using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopySkillAI : MonoBehaviour {

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
            if (GetComponent<PlayerCopyController>().SkillInUse())
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(Random.Range(2, 5));
            GetComponent<PlayerCopyController>().useSkill();
            yield return new WaitForSeconds(5);
        }
    }
}
