using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavCamera : MonoBehaviour
{

    [SerializeField] float moveSpeed = 750.0f;
    [SerializeField]Transform cameraT;

    Vector3 smoothDampVelocity = Vector3.zero;
    Vector3 smoothDampVelocityCameraCouple = Vector3.zero;
    [SerializeField] float smoothDampTime = 0.75f;
    [SerializeField] float smoothDampTimeCameraCouple = 0.75f;
    [SerializeField] bool inputEnabled;
    [SerializeField] float distTolerance;
    [SerializeField]float dist;
    [SerializeField] bool lockCursor;
    Transform povPivot;
    Transform navPopOutPivot;
    bool isCamEngaged;
    bool cameraInTransit;
    bool panInputEnabled;

    [SerializeField] bool movementModeEnabled;
    UnitBase currentUnit;
    InputStateHandler inputStateHandler;
    
    // Start is called before the first frame update
    private void Awake()
    {
        inputStateHandler = GameObject.FindObjectOfType<InputStateHandler>();
    }
    void Start()
    {
        

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //Get the active unit from the turn manager.
        
        if (currentUnit)
        {
            if(inputStateHandler.GetGameState() == GameState.pov)
            {
                SetNewTarget(povPivot.position, true);
            }
            else if (inputStateHandler.GetGameState() == GameState.movement)
            {
                SetNewTarget(navPopOutPivot.position, true);
            }
            
            isCamEngaged = true;
        }
        
    }

    public void UpdateNavCameraNewUnit(UnitBase unit)
    {
        if (!cameraInTransit)
        {
            currentUnit = unit;
            povPivot = currentUnit.povPivot;
            navPopOutPivot = currentUnit.navPopOutPivot;

            GameState gameState = inputStateHandler.GetGameState();
            if (gameState == GameState.pov)
            {
                SetNewTarget(povPivot.position,false);
            }else if(gameState == GameState.movement)
            {
                SetNewTarget(navPopOutPivot.position,false);
            }
        }
    }

    void SetNewTarget(Vector3 newPosition,bool shouldPop)
    {
        //if shouldPop is true, the camera does not lerp with a coroutine, it just snaps.
        if (shouldPop)
        {
            transform.position = newPosition;

        }else if (shouldPop==false)
        {
            StartCoroutine(MoveCamera(newPosition));
        }
        

    }


    IEnumerator MoveCamera(Vector3 targetPos)
    {
        dist = Vector3.Distance(transform.position, targetPos);
        while (distTolerance<dist)
        {
            cameraInTransit = true;
            Vector3 curCamPos = transform.position;
            dist = Vector3.Distance(curCamPos, targetPos);
            transform.position = Vector3.SmoothDamp(curCamPos, targetPos, ref smoothDampVelocityCameraCouple, smoothDampTimeCameraCouple);
            yield return new WaitForSeconds(0.01f);
        }
        cameraInTransit = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!cameraInTransit)
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                inputStateHandler.SetGameState(GameState.pov);
                SetNewTarget(currentUnit.povPivot.position,false);

            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                inputStateHandler.SetGameState(GameState.movement);
                SetNewTarget(currentUnit.navPopOutPivot.position,false);
            }


            //CAMERA DRIVE CONTROLS

            if (inputStateHandler.GetGameState() == GameState.movement)
            {


                Vector3 currentPos = transform.position;
                transform.eulerAngles = new Vector3(0, cameraT.eulerAngles.y, 0);
                //add ray cast to ground to always get consistent height above ground
                //Pan controls



                if (Input.GetKey("w"))
                {
                    currentPos += transform.forward * moveSpeed * Time.deltaTime;
                    print("forward");
                }
                if (Input.GetKey("s"))
                {
                    currentPos -= transform.forward * moveSpeed * Time.deltaTime;
                    print("backwards");
                }
                if (Input.GetKey("a"))
                {
                    currentPos -= transform.right * moveSpeed * Time.deltaTime;
                    print("left");
                }
                if (Input.GetKey("d"))
                {
                    currentPos += transform.right * moveSpeed * Time.deltaTime;
                    print("right");
                }
                transform.position = Vector3.SmoothDamp(transform.position, currentPos, ref smoothDampVelocity, smoothDampTime);
                //transform.position = currentPos;
            }
        }


        


        //Detect clicks
        //TEMP: rIGHT CLICK RETURNS CAM TO BASE, LEFT CLICK DECOUPLES.

        //HOTKEYS
        //Movement mode ability 1


    }

}
