using UnityEngine;
using System.Collections;

namespace UnderwaterShaderExample
{
    public class Plant : MonoBehaviour
    {
        void Start()
        {
            float scale = Random.Range( 0.75f, 1.25f );

            transform.localScale = transform.localScale * scale;
            transform.localEulerAngles = new Vector3( 0, Random.Range( 0, 360f ), 0 );
            
            GetComponent<Animator>().speed = Random.Range( 0.1f, 0.3f ) / scale;
        }
    }
}