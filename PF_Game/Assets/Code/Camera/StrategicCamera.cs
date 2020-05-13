using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategicCamera : MonoBehaviour
{
    /// <summary>
    /// Pan and scroll limits relative to target transform.
    /// </summary>

    [SerializeField] float panSpeed = 20.0f;
    [SerializeField] float screenBorderThickness = 20.0f;
    [SerializeField] float xLimit;
    [SerializeField] float yLimit;

    [SerializeField] Vector3 orbitPoint;
    [SerializeField] Vector3 orbitAxis = Vector3.up;
    [SerializeField] float orbitSpeed = 10.0f;
    /// <summary>
    /// POV Variant. POV at all times, though can access strat camera view at cost of ap as an ability.
    /// Destination cursor can give player some feedback about the intended location.
    /// bool to toggle modes.
    /// Third person view
    /// </summary>
    
    public void SetNewTarget(Vector3 targetPosition)
    {
        float curHeight = transform.position.y;
        orbitPoint = targetPosition;
        transform.position = new Vector3(targetPosition.x,curHeight, targetPosition.z);
    }
        // Update is called once per frame
    void Update()
    {
        
        Vector3 cameraPos = transform.position;
        if (Input.GetKey("w")||Input.mousePosition.y>=Screen.height-screenBorderThickness)
        {
            cameraPos += transform.forward * (panSpeed * Time.deltaTime);
        
        }
        if (Input.GetKey("s")||Input.mousePosition.y<=screenBorderThickness)
        {
            cameraPos -= transform.forward * (panSpeed * Time.deltaTime);
        }
        if (Input.GetKey("d")||Input.mousePosition.x>=Screen.width-screenBorderThickness)
        {
            cameraPos += transform.right * (panSpeed * Time.deltaTime);
        }
        if (Input.GetKey("a")||Input.mousePosition.x<=screenBorderThickness)
        {
            cameraPos -= transform.right * (panSpeed * Time.deltaTime);
        }
        //ORBIT
        if (Input.GetKey("q"))
        {
            print("Orbit left");
            transform.RotateAround(orbitPoint, orbitAxis, orbitSpeed * Time.deltaTime);
        }
        if (Input.GetKey("e"))
        {
            transform.RotateAround(orbitPoint, orbitAxis, -orbitSpeed * Time.deltaTime);
        }
        
        //Clamp positions
        cameraPos.x = Mathf.Clamp(cameraPos.x, -xLimit, xLimit);
        cameraPos.z = Mathf.Clamp(cameraPos.z,-yLimit, yLimit);
        transform.position = cameraPos;

    }
}
