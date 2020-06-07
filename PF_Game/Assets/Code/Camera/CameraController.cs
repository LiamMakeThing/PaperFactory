using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    
    
    Unit currentUnit;
    Transform centerOfMassTransform;
    Transform camAnchor;
    Transform camPivot;
    float camSpinValue;

    Vector3 stratCamRot;
    Vector3 camAimDirection;
    [SerializeField] float camPitch = 60.0f;
    float camYaw;
    float camRoll;
    [SerializeField] float travelSpeed = 0.125f;


    [SerializeField] float camRotateSpeed = 125.0f;
    [SerializeField] float camPanSpeed = 1000.0f;
    public Vector2 camPanBounds = new Vector2(10.0f,10.0f);

    [SerializeField] float smoothDampTime = 0.75f;
    Vector3 smoothDampVelocity = Vector3.zero;
    [SerializeField] float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVelocity;
    [SerializeField]float camFollowDist;
    //[SerializeField] Vector2 camFollowDistRange = new Vector2(1.0f, 10.0f);
    //[SerializeField] float zoomStep = 0.5f;
    

    Camera mainCam;
    private void Awake()
    {
        camAnchor = this.transform;
        camPivot = transform.Find("CamPivot");
        mainCam = Camera.main;
    }
    private void Start()
    {
        
    }
   
//Leave out the PoV mode for now.

    public void CenterCamOnUnit(Unit unit)
    {
        currentUnit = unit;
        centerOfMassTransform = currentUnit.stratCamTarget;

        
        camAnchor.SetParent(centerOfMassTransform);
        Transform destination = centerOfMassTransform;
        //NEED TO USE A STRING INVOKE SO WE CAN INTERRUPT IT. 


        StopCoroutine("MoveCamera");
        object[] parameters = new object[1] { destination};
        StartCoroutine("MoveCamera",parameters);
        //transform.position = centerOfMassTransform.position;

    }
    private void LateUpdate()
    {
        Vector3 camPos = camAnchor.position;
        
        //Strat cam controls.
        //ToDo-Reintroduce corner scroll spin and edge scroll
        //KEYBOARD CONTROLS
        if (Input.GetKey("w"))
        {
            camPos += camAnchor.forward * camPanSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            camPos -= camAnchor.forward * camPanSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            camPos -= camAnchor.right * camPanSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            camPos += camAnchor.right * camPanSpeed * Time.deltaTime;
        }

        //SPIN
        if (Input.GetKey("q"))
        {
            camSpinValue += camRotateSpeed * Time.deltaTime;
        }
        //TopLeftCorner,BottomRightCorner,E key
        if (Input.GetKey("e"))
        {
            camSpinValue -= camRotateSpeed * Time.deltaTime;
        }

        //Scroll Wheel Zoom
        //Scroll wheel is getting used for elevationchange. 
        /*
         * if(Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            
            camFollowDist -= zoomStep;

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            camFollowDist += zoomStep;
        }
        camFollowDist = Mathf.Clamp(camFollowDist, camFollowDistRange.x, camFollowDistRange.y);
        */
        



        camPos.x = Mathf.Clamp(camPos.x, -camPanBounds.x, camPanBounds.x);
        camPos.z = Mathf.Clamp(camPos.z, -camPanBounds.y, camPanBounds.y);

        camAnchor.position = Vector3.SmoothDamp(camAnchor.position, camPos, ref smoothDampVelocity, smoothDampTime);
        stratCamRot = Vector3.SmoothDamp(stratCamRot, new Vector3(0.0f, camSpinValue), ref rotationSmoothVelocity, rotationSmoothTime);

        camAnchor.eulerAngles = stratCamRot;


        camYaw = camAnchor.eulerAngles.y;
        camAimDirection = new Vector3(camPitch, camYaw, camRoll);

        //mainCam.transform.LookAt(cameraPivot);
        camPivot.eulerAngles = camAimDirection;
        mainCam.transform.position = camAnchor.position - camFollowDist * mainCam.transform.forward;
    }

    IEnumerator MoveCamera (object[] parameters)
    {
        Debug.Log("Ie");
        Transform moveDestination = (Transform)parameters[0];
        float timeCount = 0.0f;

        //will keep looping until distance is less than tolerance
        float distanceTolerance = 0.1f;
        float distanceToDestination = Vector3.Distance(camAnchor.position, moveDestination.position);

        while (distanceToDestination > distanceTolerance)
        {
            distanceToDestination = Vector3.Distance(camAnchor.position, moveDestination.position);
            camAnchor.position = Vector3.Slerp(camAnchor.position, moveDestination.position, timeCount);

            timeCount = timeCount + Time.deltaTime * travelSpeed;
            yield return new WaitForSeconds(0.01f);
        }

    }
    public void UpdateElevationLevel(int level, float elevationStep)
    {

        Transform destination = transform;
        destination.position = new Vector3(transform.position.x,0.0f,transform.position.z) + new Vector3(0,elevationStep*level,0);
        
        StopCoroutine("MoveCamera");
        object[] parameters = new object[1] { destination};
        StartCoroutine("MoveCamera", parameters);
    }
}
