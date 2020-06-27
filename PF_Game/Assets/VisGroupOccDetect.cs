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
    [SerializeField] VisGroup curDetectedVisGroup;
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

    [SerializeField] bool result;

    float camPosDelta;
    float targetPosDelta;

    [SerializeField] float deltaDistTolerance;
   

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
      
        /*raycast camera forward
         * if hit a visgroup that it not a floor, update the current detected list. Add it to current detected if not there already, add to hidden and hide.
         * if nothing detected, clear the current detected, clear the hidden and unhide stuff in hidden.
         */ 
        occludeDir = cameraT.forward;
        occludeSearchDist = 20.0f;
        RaycastHit hit;
        if(Physics.Raycast(cachedCameraPos, occludeDir, out hit))
        {
            if (hit.transform.GetComponentInParent<VisGroup>()) 
            {
                VisGroup tempVisGroup = hit.transform.GetComponentInParent<VisGroup>();
                if (!tempVisGroup.GetOmitState())
                {
                    //visgroup should not be ommitted from occluder and it is not already detected.
                    curDetectedVisGroup = tempVisGroup;
                    UpdateCurDetList();
                }
                else
                {
                    curDetectedVisGroup = null;
                    ClearDetectedList();
                    ClearHiddenList();
                }
            }
            
        }
        
        
    }
    //TO DO: SPhere cast to collect within a range, then do a compare between what is detected and what is hidden. Similar to previous system.

    void UpdateCurDetList()//makes sure currently detected are only added once each and that the list is dumped if nothing is currently detected.
    {

        if (!currentlyDetectedVisGroups.Contains(curDetectedVisGroup))
        {
            currentlyDetectedVisGroups.Add(curDetectedVisGroup);
        }
      
        if (!currentlyHiddenVisGroups.Contains(curDetectedVisGroup))
        {
            currentlyHiddenVisGroups.Add(curDetectedVisGroup);
            curDetectedVisGroup.SetVis(false, VisGroupTransitionType.Partial);
        }
    }
    void ClearDetectedList()
    {
        
        currentlyDetectedVisGroups.Clear();
    }

    void ClearHiddenList()
    {
        for (int i = 0; i<currentlyHiddenVisGroups.Count;i++)
        {
            currentlyHiddenVisGroups[i].SetVis(true, VisGroupTransitionType.Partial);
            
        }
        currentlyHiddenVisGroups.Clear();
    }
    private void OnDrawGizmosSelected()
    {
        
        
            Gizmos.DrawLine(cachedCameraPos, cachedTargetPos);
        Gizmos.DrawWireSphere(hitLocation, 1.0f);
        
        
    }
}
