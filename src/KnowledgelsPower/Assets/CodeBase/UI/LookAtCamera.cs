using UnityEngine;

namespace CodeBase.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Start() => 
            _camera = Camera.main;

        private void Update()
        {
            var rotation = _camera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.back, rotation * Vector3.up);
        }
    }
}