using UnityEngine;
using UnityEngine.InputSystem;

namespace ayy
{
    public class CameraController : MonoBehaviour
    {
        public float _moveSpeed = 10.0f;
        Vector3 _movement = Vector3.zero;
        Camera _camera;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHorizontalMovement();
            UpdateFocalMovement();
        }

        private void UpdateHorizontalMovement()
        {
            _movement = Vector3.zero;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _movement -= transform.right;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _movement += transform.right;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _movement += new Vector3(0,0,1);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _movement -= new Vector3(0,0,1);
            }
            if (_movement.sqrMagnitude > 0.0f)
            {
                transform.Translate(_movement * _moveSpeed * Time.deltaTime, Space.World);
            }
        }

        private void UpdateFocalMovement()
        {
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.0f)
            {
                float delta = Input.mouseScrollDelta.y * _moveSpeed * Time.deltaTime;
                transform.Translate(transform.forward * delta, Space.World);
            }
        }
    }
    
}

