using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShowCameraBounds : MonoBehaviour
{
    CameraController camController;
    Vector2 cameraBounds;
    
    void Update()
    {
        if (camController == null)
        {
            camController = GetComponent<CameraController>();
        }
        cameraBounds = camController.camPanBounds*2;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(cameraBounds.x, 0.0f, cameraBounds.y));
    }
}
