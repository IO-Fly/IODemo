using UnityEngine;
using System.Collections;

namespace UnderwaterShaderExample
{
    public class Fish : MonoBehaviour
    {
        public bool reverse = false;
        public Animator pathAnimator;
        public Animator fishAnimator;

        private float _rand1;
        private float _rand2;

        void Start()
        {
            _rand1 = Random.Range( 0.5f, 1.5f );
            _rand2 = Random.Range( 0.5f, 1.5f );
        }

        void Update()
        {
            float speed = Mathf.PerlinNoise( Time.time * _rand1, Time.time * _rand2 ) + 1f;

            if( reverse )
            {
                pathAnimator.SetBool( "Reversed", true );
            }

            pathAnimator.speed = speed * 0.25f;
            fishAnimator.speed = speed * 1.5f;
        }
    }
}