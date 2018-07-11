using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCircleProgress : MonoBehaviour
{
    public float LoadingCircleSpeed = 250f;

    private RectTransform rectTransform;
	private void Start ()
    {
        rectTransform = this.GetComponent<RectTransform>();
	}

	private void Update ()
    {
        rectTransform.Rotate(-Vector3.forward * Time.deltaTime * LoadingCircleSpeed);
	}
}
