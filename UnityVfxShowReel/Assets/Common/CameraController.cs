using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;  // 相机移动速度
    public float rotateSpeed = 50f;  // 相机旋转速度
    public float zoomSpeed = 5f;  // 相机缩放速度

    private Vector3 lastMousePosition;
    private bool isDragging;

    void Update()
    {
        // 处理键盘输入来移动相机
        // float horizontalInput = Input.GetAxis("Horizontal");
        // float verticalInput = Input.GetAxis("Vertical");
        // Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        // transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        
        // 通过WASD键控制相机沿着局部坐标系移动
        float horizontalInput = 0;
        float verticalInput = 0;
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);        

        // 处理鼠标右键拖动来旋转相机
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            if (isDragging)
            {
                Vector3 offset = Input.mousePosition - lastMousePosition;
                offset.x *= rotateSpeed * Time.deltaTime;
                offset.y *= rotateSpeed * Time.deltaTime;
                transform.Rotate(Vector3.up, offset.x, Space.World);
                transform.Rotate(Vector3.right, -offset.y, Space.Self);
                lastMousePosition = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        // 处理鼠标滚轮来缩放相机
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelInput!= 0)
        {
            transform.Translate(Vector3.forward * scrollWheelInput * zoomSpeed, Space.Self);
        }
    }
}