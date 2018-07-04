using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCircleProgress : MonoBehaviour
{
    private RectTransform rectTransform;
    private float rotateSpeed = 250f;

	private void Start ()
    {
        rectTransform = this.GetComponent<RectTransform>();
	}

	private void Update ()
    {
        rectTransform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
	}
}
