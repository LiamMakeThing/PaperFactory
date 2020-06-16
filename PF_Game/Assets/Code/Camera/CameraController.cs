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
    [SerializeField] float travelSpeedUnitFocus = 0.125f;
    [SerializeField] float travelSpeedElevationChange = 0.125f;


    [SerializeField] float camRotateSpeed = 125.0f;
    [SerializeField] float camPanSpeed = 1000.0f;
    public Vector2 camPanBounds = new Vector2(10.0f,10.0f);

    [SerializeField] float smoothDampTime = 0.75f;
    Vector3 smoothDampVelocity = Vector3.zero;
    [SerializeField] float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVelocity;
    [SerializeField]float camFollowDist;

    CameraOcclusion camOcclusion;
    

    Camera mainCam;
    private void Awake()
    {
        camAnchor = this.transform;
        camPivot = transform.Find("CamPivot");
        mainCam = Camera.main;
        camOcclusion = GetComponent<CameraOcclusion>();
    }
    private void Start()
    {
        
    }
   
//Leave out the PoV mode for now.

    public void CenterCamOnUnit(Unit unit)
    {
        currentUnit = unit;
        centerOfMassTransform = currentUnit.stratCamTarget;
        //UPDATE Camera Occlusion with new focus.
        camOcclusion.UpdateViewTarget(unit.transform);


        
        //camAnchor.SetParent(centerOfMassTransform);
        Vector3 destination = centerOfMassTransform.position;
        //NEED TO USE A STRING INVOKE SO WE CAN INTERRUPT IT. 


        StopCoroutine("MoveCamera");
        object[] parameters = new object[2] { destination,travelSpeedUnitFocus};
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

    IEnumerator MoveCamera (object[] parameters)//only takes in vector3 position currently. Can use dotprodcut and transform forward later to get direction matching.
    {
        
        Vector3 moveDestination = (Vector3)parameters[0];
        float moveSpeed = (float)parameters[1];
        float timeCount = 0.0f;

        //will keep looping until distance is less than tolerance
        float distanceTolerance = 0.1f;
        float distanceToDestination = Vector3.Distance(camAnchor.position, moveDestination);

        while (distanceToDestination > distanceTolerance)
        {
            distanceToDestination = Vector3.Distance(camAnchor.position, moveDestination);
            camAnchor.position = Vector3.Slerp(camAnchor.position, moveDestination, timeCount);

            timeCount = timeCount + Time.deltaTime * moveSpeed;
            yield return new WaitForSeconds(0.01f);
        }
        

    }
    public void UpdateElevationLevel(int level, float elevationStep)
    {

        float yPos = level * elevationStep;
        Vector3 destination = new Vector3(transform.position.x, yPos, transform.position.z);
        
        object[] parameters = new object[2] { destination,travelSpeedElevationChange };
        StopCoroutine("MoveCamera");
        StartCoroutine("MoveCamera", parameters);
    }
}
