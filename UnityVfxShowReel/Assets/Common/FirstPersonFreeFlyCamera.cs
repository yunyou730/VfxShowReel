using UnityEngine;

namespace ayy.common
{
    /// <summary>
    /// First-person free-fly camera controller for runtime scene browsing/debug.
    ///
    /// Controls:
    /// - WASD: move
    /// - Q/E: down/up
    /// - Right mouse button (hold): look around
    /// - Mouse wheel: adjust move speed
    /// - Shift: faster
    /// - Ctrl: slower
    /// - F: reset speed to default
    /// - Esc: release cursor
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FirstPersonFreeFlyCamera : MonoBehaviour
    {
        [Header("Look")]
        [SerializeField] private float lookSensitivity = 2.0f;
        [SerializeField] private bool invertY;
        [SerializeField] private float pitchMin = -89f;
        [SerializeField] private float pitchMax = 89f;

        [Header("Move")]
        [SerializeField] private float baseMoveSpeed = 8.0f;
        [SerializeField] private float shiftMultiplier = 3.0f;
        [SerializeField] private float ctrlMultiplier = 0.35f;
        [SerializeField] private float scrollSpeedStep = 1.0f;
        [SerializeField] private float minMoveSpeed = 0.2f;
        [SerializeField] private float maxMoveSpeed = 80f;

        [Header("Runtime")]
        [SerializeField] private bool lockCursorOnRightMouse = true;

        private float _yaw;
        private float _pitch;
        private float _moveSpeed;
        private bool _looking;

        private void Awake()
        {
            var euler = transform.rotation.eulerAngles;
            _yaw = euler.y;
            // Convert Unity 0..360 pitch into -180..180 for clamping
            _pitch = euler.x > 180f ? euler.x - 360f : euler.x;

            _moveSpeed = baseMoveSpeed;
        }

        private void Update()
        {
            HandleLook();
            HandleMove();
            HandleSpeedAdjust();
        }

        private void HandleLook()
        {
            if (lockCursorOnRightMouse)
            {
                if (Input.GetMouseButtonDown(1))
                    BeginLook();
                if (Input.GetMouseButtonUp(1))
                    EndLook();
            }
            else
            {
                if (!_looking)
                    BeginLook();
            }

            if (!_looking)
                return;

            float mx = Input.GetAxisRaw("Mouse X");
            float my = Input.GetAxisRaw("Mouse Y");

            _yaw += mx * lookSensitivity;
            _pitch += (invertY ? my : -my) * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            if (Input.GetKeyDown(KeyCode.Escape))
                EndLook();
        }

        private void HandleMove()
        {
            float dt = Time.unscaledDeltaTime;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            float up = 0f;
            if (Input.GetKey(KeyCode.E)) up += 1f;
            if (Input.GetKey(KeyCode.Q)) up -= 1f;

            float speed = _moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                speed *= shiftMultiplier;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                speed *= ctrlMultiplier;

            Vector3 local = new Vector3(h, up, v);
            if (local.sqrMagnitude > 1f) local.Normalize();

            transform.position += transform.TransformDirection(local) * (speed * dt);
        }

        private void HandleSpeedAdjust()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                _moveSpeed = Mathf.Clamp(_moveSpeed + scroll * scrollSpeedStep, minMoveSpeed, maxMoveSpeed);
            }

            if (Input.GetKeyDown(KeyCode.F))
                _moveSpeed = baseMoveSpeed;
        }

        private void BeginLook()
        {
            _looking = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void EndLook()
        {
            _looking = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
