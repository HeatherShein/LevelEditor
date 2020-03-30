using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool canMove = true;

    public float panSpeed = 30f; // Camera MS

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    public float scrollSpeed = 5f;

    public float minY = 10f;
    public float maxY = 80f;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            canMove = !canMove;
        }

        if (canMove)
        {
            // Forward
            if (Input.GetKey(KeyCode.Z))
            {
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            }

            // Backward
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World); 
            }

            // Right
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            }

            // Left
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World); 
            }

            // Rotation
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 pos = transform.position;

        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;

        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;

    }
}
