using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CameraTransitionType
{
    SmoothLerp, Pop, Fade
}
public enum CameraTransformFilter
{
    Positon, Rotation, PositionAndRotation
}
public class MultiModeCamera : MonoBehaviour
{
    /// <summary>
    /// this stores mutliple camera profiles and switches between them. 
    /// </summary>
    // Start is called before the first frame update
    [SerializeField] GameState gameStateLocal = new GameState();
    //GameState cachedGameState = new GameState();
    Transform camObject;
    Transform targetPositionPoV;
    Transform targetPositionMovement;
    

    [SerializeField] float travelSpeed = 0.125f;

    bool cameraInTransit;
    bool inputEnabled;

    CurrentUnitHandler unitHandler;
    GameStateHandler gameStateHandler;

    Unit currentUnit;
    

    [Header("MovementModeCamera")]
    [SerializeField] Transform cameraMoveModeObj;
    [SerializeField] float edgeScrollWidth = 50.0f;
    [SerializeField] float scrollSpeed = 1.0f;
    [SerializeField] float rotateSpeed = 1.0f;
    [SerializeField] float smoothDampTime = 0.75f;
    Vector3 smoothDampVelocity = Vector3.zero;
    Vector3 currentMoveCamRotation;
    float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVelocity;
    float spin;
    [SerializeField] Vector2 camPanBounds = new Vector2(25, 25);
    

    

    [Header("POV Camera")]
    float povYaw;
    float povPitch;
    [SerializeField] float mouseSensitivity = 10.0f;
    [SerializeField] float distFromTarget = 2.0f;
    //[SerializeField] Transform target;
    [SerializeField] Vector2 povPitchClamp = new Vector2(-40, 85);
    [SerializeField] float povRotationSmoothTime = 0.12f;
    Vector3 povRotationSmoothVelocity;
    Vector3 currentPoVRotation;
    Camera mainCam;


    private void Awake()
    {
        mainCam = Camera.main;
        camObject = transform.Find("CameraPivot");
        targetPositionMovement = GameObject.Find("TargetPositionMovement").transform;
        cameraMoveModeObj = GameObject.Find("CamMovePanner").transform;
        unitHandler = GameObject.FindObjectOfType<CurrentUnitHandler>();
        gameStateHandler = GameObject.FindObjectOfType<GameStateHandler>();

    }
    


    void Start()
    {
        gameStateLocal = gameStateHandler.GetGameState();
        inputEnabled = true;
    }

    public void UpdateGameState(GameState gameState)
    {
        if (gameStateLocal!=gameState)
        {
            gameStateLocal = gameState;
            CenterCamOnUnit(currentUnit, CameraTransitionType.SmoothLerp, CameraTransformFilter.PositionAndRotation,false);
        }
        
    }
    // Update is called once per frame
    void Update()
    {   
       
        
    }

    private void LateUpdate()
    {
        //CAMERA MOVEMENT CONTROLS
        if (!cameraInTransit)
        {
            switch (gameStateLocal)
            {
                case GameState.movement:


                    Vector3 camPos = cameraMoveModeObj.position;
                    float mouseX = Input.mousePosition.x;
                    float mouseY = Input.mousePosition.y;
                    float xMin = edgeScrollWidth;
                    float xMax = Screen.width - edgeScrollWidth;
                    float yMin = edgeScrollWidth;
                    float yMax = Screen.height - edgeScrollWidth;
                    
                    
                    //key W or mouse at top and not in corners
                    if (Input.GetKey("w") || mouseY >= yMax && mouseX>=xMin&&mouseX<=xMax)
                    {
                        camPos += cameraMoveModeObj.forward * scrollSpeed * Time.deltaTime;
                    }
                    //key S or mouse at bottom and not in corners
                    if (Input.GetKey("s")|| mouseY <= yMin&& mouseX >= xMin && mouseX <= xMax)
                    {
                        camPos -= cameraMoveModeObj.forward * scrollSpeed * Time.deltaTime;
                    }
                    //key A or mouse at left side and not in corners
                    if (Input.GetKey("a")|| mouseX<=xMin&& mouseY >= yMin && mouseY <= yMax)
                    {
                        camPos -= cameraMoveModeObj.right * scrollSpeed * Time.deltaTime;
                    }
                    //key D or mouse at right side and not in corners
                    if (Input.GetKey("d")|| mouseX >= xMax && mouseY >= yMin && mouseY <= yMax)
                    {
                        camPos += cameraMoveModeObj.right * scrollSpeed * Time.deltaTime;
                    }
                    camPos.x = Mathf.Clamp(camPos.x,-camPanBounds.x, camPanBounds.x);
                    camPos.z = Mathf.Clamp(camPos.z,-camPanBounds.y, camPanBounds.y);

                    cameraMoveModeObj.position = Vector3.SmoothDamp(cameraMoveModeObj.position, camPos, ref smoothDampVelocity, smoothDampTime);

                    //CAMERA SPIN. MOUSE IN CORNERS
                    //BottomLeftCorner,TopRightCorner,Q key
                    //Check mouse position
                
                    if (mouseY<= yMin && mouseX<=yMin||mouseY>=yMax && mouseX>=xMax||Input.GetKey("q"))
                    {
                        spin += rotateSpeed * Time.deltaTime;
                    }
                    //TopLeftCorner,BottomRightCorner,E key
                    if (mouseY >= yMax && mouseX <= xMin||mouseY<=yMin &&mouseX >= xMax||Input.GetKey("e"))
                    {
                        
                        spin -= rotateSpeed * Time.deltaTime;
                    }
               
                    currentMoveCamRotation = Vector3.SmoothDamp(currentMoveCamRotation, new Vector3(0.0f, spin), ref rotationSmoothVelocity, rotationSmoothTime);

                    cameraMoveModeObj.eulerAngles = currentMoveCamRotation;
                    break;

                case GameState.pov:
                    //TODO. Weird ass pop coming after transition. Got to figure that shit out.
                    povYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                    povPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                    povPitch = Mathf.Clamp(povPitch, povPitchClamp.x, povPitchClamp.y);

                    currentPoVRotation = Vector3.SmoothDamp(currentPoVRotation, new Vector3(povPitch, povYaw), ref povRotationSmoothVelocity, povRotationSmoothTime);

                    camObject.eulerAngles = currentPoVRotation + currentUnit.povTarget.eulerAngles;
                    break;

                default:
                    break;
            }
            
        }
        

    }


    public void CenterCamOnUnit(Unit newUnit, CameraTransitionType transitionType, CameraTransformFilter camTransformFilter,bool overrideExistingTransit)
    {

        if (!cameraInTransit||overrideExistingTransit)
        {
            currentUnit = newUnit;
            Transform targetTransform = this.transform;//temporarily assign it as this transform to initialize it. It gets overridden in the following switch
            switch (gameStateLocal)
            {
                case GameState.movement:
                    targetTransform = currentUnit.movementTarget;
                    camObject.SetParent(cameraMoveModeObj);
                    break;
                case GameState.pov:
                    targetTransform = currentUnit.povTarget;
                    camObject.SetParent(null);

                    povPitch = 0.0f;
                    povYaw = 0.0f;
                    currentPoVRotation = new Vector3(0, 0, 0);
                    break;
            }

            
            switch (transitionType)
            {
                case CameraTransitionType.Fade:
                    StartCoroutine(RunFadeTransitionSequence(targetTransform, camTransformFilter));
                    break;
                case CameraTransitionType.Pop:
                    camObject.position = targetTransform.position;
                    camObject.rotation = targetTransform.rotation;
                    break;
                case CameraTransitionType.SmoothLerp:
                    object[] parms = new object[2] { targetTransform,camTransformFilter};
                    StopCoroutine("MoveObject");
                    StartCoroutine("MoveObject", parms);
                    
                    break;
                default:
                    break;
            }
        }
        
    }

    IEnumerator RunFadeTransitionSequence(Transform newTransform,CameraTransformFilter camTransformFilter)
    {
        yield return StartCoroutine(Fade(true));
        //yield return StartCoroutine(MoveObject(newTransform, camTransformFilter));
        yield return StartCoroutine(Fade(false));
    }

    IEnumerator Fade(bool direction)
    {
        float valueA = 3.0f;
        float valueB = 60.0f;
        if (direction==true)
        {
            Debug.Log("Fade Out");
            
            while(valueB>valueA)
            {
                valueB = valueB - 3.0f;
                Camera.main.fieldOfView = valueB;
                yield return new WaitForSeconds(0.01f);
            }
        }else if(direction == false)
        {
            Debug.Log("Fade In");
          
            while (valueA < valueB)
            {
                valueA = valueA + 3.0f;
                Camera.main.fieldOfView = valueA;
                yield return new WaitForSeconds(0.01f);
            }
        }
        
    }

    IEnumerator MoveObject(object[] parms)
    {
        Transform targetTransform = (Transform)parms[0];
        CameraTransformFilter camTransformFilter = (CameraTransformFilter)parms[1];
        float timeCount = 0.0f;
        //loop while distance and rotation are not equal
        float distTolerance = 0.1f;
        float dist = Vector3.Distance(camObject.position, targetTransform.position);
        //use dot product of two directions to compare to tolerance value.
        float angleDotProd = Mathf.Abs(Quaternion.Dot(camObject.rotation, targetTransform.rotation));
        float angleTolerance = 0.1f;

        while (distTolerance < dist && angleTolerance<angleDotProd)
        {

            cameraInTransit = true;

            angleDotProd = Mathf.Abs(Quaternion.Dot(camObject.rotation, targetTransform.rotation));
            dist = Vector3.Distance(camObject.position, targetTransform.position);

            switch (camTransformFilter)
            {
                case CameraTransformFilter.Positon:
                    camObject.position = Vector3.Slerp(camObject.position, targetTransform.position, timeCount);
                    break;
                case CameraTransformFilter.Rotation:
                    camObject.rotation = Quaternion.Slerp(camObject.rotation, targetTransform.rotation, timeCount);
                    break;
                case CameraTransformFilter.PositionAndRotation:
                    camObject.position = Vector3.Slerp(camObject.position, targetTransform.position, timeCount);
                    camObject.rotation = Quaternion.Slerp(camObject.rotation, targetTransform.rotation, timeCount);
                    break;
                default:
                    break;
            }
            timeCount = timeCount + Time.deltaTime*travelSpeed;

            yield return new WaitForSeconds(0.01f);
        }
        cameraInTransit = false;
        Debug.Log("Setting position");
    }
    
}
