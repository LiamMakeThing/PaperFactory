using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    float yaw;
    float pitch;
    [SerializeField] float mouseSensitivity = 10.0f;
    [SerializeField] float distFromTarget = 2.0f;
    [SerializeField] Transform target;
    [SerializeField] Vector2 pitchClamp = new Vector2(-40,85);
    [SerializeField] float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    private void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch,pitchClamp.x, pitchClamp.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        
        transform.eulerAngles = currentRotation; 

        transform.position = target.position - transform.forward * distFromTarget;


    }
    //TODO:
/*
 * 
 * 
 */ 

}
