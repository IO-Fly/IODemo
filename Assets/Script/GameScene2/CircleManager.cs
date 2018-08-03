using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CircleManager : MonoBehaviour {
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

	// Use this for initialization
	void Start () {
		Stage = 0;
		Timeing = LimitTime[Stage];
		IsArrive = true;
		mRadius = 450;
		mCircleX = 0f;
		mCircleY = 0f;
		StartCoroutine(ReduceTime());
	}
	
	// Update is called once per frame
	void Update () {
		if(IsArrive){
			TimeingText.text = "还有"+ Timeing + "刷新安全区";
			StageText.text = "目前是第" + (Stage+1) +"阶段";
		}
		if(Timeing<=0){
			Timeing = 0;
			PoisonCircleMove();
			Stage++;
			IsArrive = false;

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
		IsArrive = true;
		Timeing = LimitTime[Stage];
		StartCoroutine(ReduceTime());
	}
//生成安全区的范围
	public void SafetyZoneMove(float radius,float circleX,float circleY){
		SafetyZone = Instantiate(CirclePrefab, new Vector3(circleX,130,circleY),Quaternion.identity);//到时转为网络场景对象的创建
		SafetyZone.transform.DOMove(new Vector3(circleX, 130,circleY),0.1f);
		SafetyZone.transform.DOScaleX(radius,0.1f);
		SafetyZone.transform.DOScaleY(radius,0.1f);
	}
//毒圈缩小方法
	public void  PoisonCircleMove(){
		CircleCenter();
		SafetyZoneMove(mRadius,mCircleX,mCircleY);
		CirclePrefab.transform.DOMove(new Vector3(mCircleX, 130, mCircleY),ShrinkTime);
		CirclePrefab.transform.DOScaleX(mRadius, ShrinkTime);
		CirclePrefab.transform.DOScaleY(mRadius, ShrinkTime);
		if(Stage<LimitTime.Length - 1){
			StartCoroutine(StartNextTime());
		}
	}
	public void CircleCenter(){
		mRadius = 0.5f * mRadius;
		mCircleX = Random.Range(mCircleX - 0.2f*mRadius, mCircleX +0.2f*mRadius);
		mCircleY = Random.Range(mCircleY - 0.2f*mRadius, mCircleY +0.2f*mRadius);
	}

}
