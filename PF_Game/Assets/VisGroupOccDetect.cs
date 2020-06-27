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
    [SerializeField] List<VisGroup> currentlyDetectedVisGroups = new List<VisGroup>();
    [SerializeField] List<VisGroup> currentlyHiddenVisGroups = new List<VisGroup>();
    Transform cameraT;
    [SerializeField] Transform targetT;
    Vector3 occludeDir;
    float occludeSearchDist;
    Vector3 targetPos;
    Vector3 curCameraPos;
    Vector3 cachedTargetPos = Vector3.zero;
    Vector3 cachedCameraPos = Vector3.zero;
    Vector3 hitLocation;

    float camPosDelta;
    float targetPosDelta;

    [SerializeField] float deltaDistTolerance;
    [SerializeField] VisGroup tempVisGroup;

    [SerializeField] CurrentUnitHandler unitHandler;
    


    // Start is called before the first frame update
    void Start()
    {
        unitHandler = GameObject.FindObjectOfType<CurrentUnitHandler>();
        cameraT = Camera.main.transform;
        
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
        if (unitHandler.GetCurrentUnit()!=null)
        {
            targetT = unitHandler.GetCurrentUnit().transform;


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
    }
    void OccludeCheck()
    {
        /*If somthign is hit and it is not in curDet list add it.
         * if nothing is hit, empty the curdetected list
         
         * 
         */ 
         
        occludeDir = (cachedCameraPos - cachedTargetPos).normalized;
        occludeSearchDist =Vector3.Distance(cachedCameraPos,cachedTargetPos);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, -occludeDir, out hit, occludeSearchDist))
        {
            

            if (hit.transform.GetComponentInParent<LevelKitBase>())
            {
                
                tempVisGroup = hit.transform.GetComponentInParent<VisGroup>();
                if (!currentlyDetectedVisGroups.Contains(tempVisGroup))
                {
                    print("unique item founf");
                    currentlyDetectedVisGroups.Add(tempVisGroup);

                }
                

            }

        }
        else if(currentlyDetectedVisGroups.Count>0)
        {
            print("nothing found");
            foreach(VisGroup visG in currentlyDetectedVisGroups)
            {
                currentlyDetectedVisGroups.Remove(visG);
            }
        }
        
        /*
        foreach(VisGroup visG in currentlyHiddenVisGroups)
        {
            if (!currentlyDetectedVisGroups.Contains(visG))
            {
                currentlyHiddenVisGroups.Remove(visG);
                visG.ToggleVis(VisGroupTransitionType.Partial);
            }
            
        }

        if (currentlyDetectedVisGroups.Count > 0)
        {
            foreach (VisGroup visG in currentlyDetectedVisGroups)
            {
                if (!currentlyHiddenVisGroups.Contains(visG))
                {
                    currentlyHiddenVisGroups.Add(visG);
                    visG.ToggleVis(VisGroupTransitionType.Partial);
                }
            }
        }
        
    */
        
    }
    private void OnDrawGizmosSelected()
    {
        
        
            Gizmos.DrawLine(cachedCameraPos, cachedTargetPos);
        Gizmos.DrawWireSphere(hitLocation, 1.0f);
        
        
    }
}
