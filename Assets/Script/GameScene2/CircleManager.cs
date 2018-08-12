using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CircleManager : Photon.PunBehaviour {
	public Text StageText;
	public Text TimeingText;
	public float[] LimitTime;
	public int Stage;
	public bool IsArrive;
	public float Timeing;
	private float mRadius;
	private float mCircleX;
	private float mCircleY;
	public GameObject CirclePrefab;
	private GameObject SafetyZone;
	public float ShrinkTime=10f;

    static public CircleManager LocalCircleManager = null;

    // Use this for initialization

    void Start () {


        LocalCircleManager = this;

		Stage = 0;
		Timeing = LimitTime[Stage];
		IsArrive = true;
		mRadius = 450;
		mCircleX = 0f;
		mCircleY = 0f;
		if(PhotonNetwork.isMasterClient){
		
		CirclePrefab = PhotonNetwork.InstantiateSceneObject("circle", new Vector3(0,-40,0), Quaternion.Euler(-90,0,0),0,null);

        StartCoroutine(ReduceTime());
		CircleCenter();
		SafetyZoneMove(mRadius,mCircleX,mCircleY);
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(IsArrive){
			TimeingText.text = "还有"+ Timeing + "刷新安全区";
			StageText.text = "目前是第" + (Stage+1) +"阶段";
		
		if(Timeing<=0){
			Timeing = 0;
			if(PhotonNetwork.isMasterClient)
			PoisonCircleMove();
			Stage++;
			IsArrive = false;

		}
		}
	}
	IEnumerator ReduceTime(){
		while(Timeing>0){
			yield return new WaitForSeconds(1f);
			Timeing--;
		}
	}
	IEnumerator StartNextTime(){
		yield return new WaitForSeconds(ShrinkTime);
		StartNextPoisonCircle();
	}

	public void StartNextPoisonCircle(){
		CircleCenter();
		SafetyZoneMove(mRadius,mCircleX,mCircleY);
		IsArrive = true;
		Timeing = LimitTime[Stage];
		StartCoroutine(ReduceTime());
	}
//生成安全区的范围
	public void SafetyZoneMove(float radius,float circleX,float circleY){
		if(PhotonNetwork.isMasterClient){
		if(SafetyZone!=null){
			PhotonNetwork.Destroy(SafetyZone);
		}
		SafetyZone = PhotonNetwork.InstantiateSceneObject("circle", new Vector3(circleX,-40,circleY), Quaternion.Euler(-90,0,0),0,null);//到时转为网络场景对象的创建
		SafetyZone.transform.DOMove(new Vector3(circleX, -40,circleY),0.1f);
		SafetyZone.transform.DOScaleX(radius,0.1f);
		SafetyZone.transform.DOScaleY(radius,0.1f);
		}
	}
//毒圈缩小方法
	public void  PoisonCircleMove(){
		//CircleCenter();
		//SafetyZoneMove(mRadius,mCircleX,mCircleY);
		CirclePrefab.transform.DOMove(new Vector3(mCircleX, -40, mCircleY),ShrinkTime);
		CirclePrefab.transform.DOScaleX(mRadius, ShrinkTime);
		CirclePrefab.transform.DOScaleY(mRadius, ShrinkTime);
		if(Stage<LimitTime.Length - 1){
			StartCoroutine(StartNextTime());
		}
	}
	public void CircleCenter(){
		mRadius = 0.5f * mRadius;
		mCircleX = Random.Range(mCircleX - 0.5f*mRadius, mCircleX +0.5f*mRadius);
		mCircleY = Random.Range(mCircleY - 0.5f*mRadius, mCircleY +0.5f*mRadius);
	}
 public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting&&PhotonNetwork.isMasterClient)
        {
            stream.SendNext(this.Timeing);
			stream.SendNext(this.Stage);
        }
        else
        {
            this.Timeing = (float)stream.ReceiveNext();
			this.Stage = (int)stream.ReceiveNext();
        }
    }

}
