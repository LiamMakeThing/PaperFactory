using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoVCamera : MonoBehaviour
{
    [SerializeField] float yAngleMin = 0.0f;
    [SerializeField] float yAngleMax = 50.0f;
    [SerializeField] Transform lookAt;
    [SerializeField] Transform camTransform;
    private Camera cam;

    [SerializeField] float distance = 10.0f;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
   

    private void Awake()
    {
        camTransform = transform;
        cam = Camera.main;
    }
    private void Update()
    {
        currentX += Input.GetAxis("Mouse X");
        currentY -= Input.GetAxis("Mouse Y");
        currentY = Mathf.Clamp(currentY,yAngleMin,yAngleMax);
    }
    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0,0,-distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt.position + rotation * dir;
        camTransform.LookAt(lookAt.position);

    }

}
