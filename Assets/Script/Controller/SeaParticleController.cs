using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaParticleController : MonoBehaviour {

    public ParticleSystem effect;
	// Use this for initialization
	void Start () {
        DisableParticle();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void EnableParticle()
    {
        effect.transform.Rotate(180, 0, 0);
        effect.Play();
    }

    void DisableParticle()
    {

        effect.Clear();
        effect.Pause();
    }

    public void EnterSky()
    {
        effect.transform.Rotate(180, 0, 0);
        EnableParticle();
        StartCoroutine(WaitForEndEffect());
    }

    public void LeaveSky()
    {
        effect.transform.Rotate(180, 0, 0);
        EnableParticle();
        StartCoroutine(WaitForEndEffect());
    }


    IEnumerator WaitForEndEffect()
    {
        yield return new WaitForSeconds(1.0f);
        //关闭粒子效果
        DisableParticle();

    }
}
