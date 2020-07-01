using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisGroupOccDetect : MonoBehaviour
{

    //raycast between target destination cursor and camera? Better than getting everything in the scene and checking distance.

    /*Get the destination cursor and the camera.
    Timed update with coroutine so not hammering update.
    Every update, get the camera position and the destination cursor position. 
    If delta for either of them is larger than tolerance, cache the new position and call a new raycast.
    if raycast returns a hit that is a level kit, grab the parent vis group.

    check to see if the item is in the currently detected list, if not add it, if yes, return
    
    check currently hidden list to see what is in that list that is not in currently detected. If there is anything, remove it from cur hidden list and unhide it.

    add all vis groups in currently detected to currently hidden and hide them.
    */

    [SerializeField] float occludeCheckFrequency = 0.5f;
    [SerializeField] VisGroup curDetVisG;
    [SerializeField] VisGroup curHidVisG;
    
    
    Transform cameraT;
    Vector3 curCameraPos;
    Vector3 cachedCameraPos = Vector3.zero;


    Transform targetT;
    Vector3 targetPos;
    Vector3 cachedTargetPos = Vector3.zero;



    float camPosDelta;

    float targetPosDelta;

    [SerializeField] float deltaDistTolerance;


    

    int environmentLayerMask = 1 << 9;

    [SerializeField] bool visGroupFound;

    [SerializeField] float searchDistance;



    // Start is called before the first frame update
    void Start()
    {
        
        cameraT = Camera.main.transform;
        targetT = GameObject.FindObjectOfType<NavCursor>().transform;
        
        StartCoroutine("RequestOccludeCheck");
        
        
    }
    IEnumerator RequestOccludeCheck()
    {
        while (true)
        {
            
            VerifyDelta();
            yield return new WaitForSeconds(occludeCheckFrequency);
        }
    }
    void VerifyDelta()
    {
            curCameraPos = cameraT.position;
            targetPos = targetT.position;

            camPosDelta = Vector3.Distance(curCameraPos, cachedCameraPos);
            targetPosDelta = Vector3.Distance(targetPos, cachedTargetPos);

            if (camPosDelta >= deltaDistTolerance || targetPosDelta >= deltaDistTolerance)
            {
                //Cam has moved or nav cursor has moved
                cachedCameraPos = curCameraPos;
                cachedTargetPos = targetPos;
                OccludeCheck();
            }
    }
    void OccludeCheck()
    {
        //get distance and direction
        Vector3 heading = cachedTargetPos - cachedCameraPos;
        searchDistance = heading.magnitude;
        Vector3 dirToCursor = heading / searchDistance;


        Ray ray = new Ray(cachedCameraPos, dirToCursor);
        RaycastHit hit;
        //Racyast. Make sure cursor is actually behind visgroup to fire, otherwsise it should stay visible.
        visGroupFound = Physics.Raycast(ray, out hit, searchDistance, environmentLayerMask);


        if (visGroupFound)
        {
            curDetVisG = hit.transform.GetComponentInParent<VisGroup>();
            if (curDetVisG != curHidVisG)
            {
                if (curHidVisG)
                {
                    curHidVisG.SetVis(true, VisGroupTransitionType.Partial);
                }
                curDetVisG.SetVis(false,VisGroupTransitionType.Partial);
                curHidVisG = curDetVisG;
            }
        }
        else
        {
            if (curHidVisG)
            {
                curHidVisG.SetVis(true, VisGroupTransitionType.Partial);
                curHidVisG = null;
            }
        }
        //Collect what is currently detected.

    }   
  

}
