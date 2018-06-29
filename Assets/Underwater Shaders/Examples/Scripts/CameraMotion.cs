using UnityEngine;
using System.Collections;

namespace UnderwaterShaderExample
{
    public class CameraMotion : MonoBehaviour
    {
        public float drift = 1f;
        public Transform target;

        private Vector3 _targetPosition;

        void Start()
        {
            _targetPosition = new Vector3( 0, 0, 5f );
        }

        void Update()
        {
            _targetPosition.x = Mathf.Sin( Time.time * 0.15f ) * 0.25f * drift;
            _targetPosition.y = Mathf.Cos( Time.time * 0.25f ) * 0.25f * drift;

            target.localPosition = _targetPosition;

            transform.LookAt( target );
        }
    }
}