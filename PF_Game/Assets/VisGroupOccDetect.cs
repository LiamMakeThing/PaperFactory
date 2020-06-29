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
    [SerializeField] List<VisGroup> curDetVisGs = new List<VisGroup>();
    [SerializeField] List<VisGroup> curHidVisGs = new List<VisGroup>();
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

    int environmentLayerMask = 1 << 9;
    [SerializeField] Collider[] collidersOverlapped = new Collider[0];
    [SerializeField] float overlapSphereRad = 2.0f;



    // Start is called before the first frame update
    void Start()
    {
        unitHandler = GameObject.FindObjectOfType<CurrentUnitHandler>();
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

        collidersOverlapped = Physics.OverlapSphere(targetPos, overlapSphereRad, environmentLayerMask);

        if (collidersOverlapped.Length>0)
        {
            print("env found");
            //Clear detected.
            

            //get the visgroups found
            //check to make sure they are not in the curDetList. If no, add them
            CollectVisGroups();

            //ShowItemsNoLongerDetected();
            //* * check to see what is in the in the curHidList that is not in the curDetList. If anything, remove them and show them

            HideItemsCurDetected();
             //check to see what is in the curDetList that is not in the curHidList. If anything, add them and hide them.
             
            
        }
        else
        {
            ClearCurDetList();
            print("nothing");
            //clear the curHidList and show everthing in it.
            ShowItems();
            //ShowItemsNoLongerDetected();
        }
        
        
    }
    void ClearCurDetList()
    {
        curDetVisGs = new List<VisGroup>();
    }
    

    void CollectVisGroups()
    {
        //find the parent visgroup
        //check to see if it exists in list already
        //Add it
        ClearCurDetList();
        foreach (Collider c in collidersOverlapped)
        {
            //check to see if it is further from camera than destination.
            float distToCollider = Vector3.Distance(cachedCameraPos,c.transform.position);
            float distToTarget = Vector3.Distance(cachedCameraPos, cachedTargetPos);
            if (distToCollider < distToTarget)
            {
                if (c.transform.GetComponentInParent<VisGroup>())
                {
                    VisGroup curVisG = c.transform.GetComponentInParent<VisGroup>();

                    if (!curDetVisGs.Contains(curVisG))
                    {
                        curDetVisGs.Add(curVisG);
                    }
                }
            }
            
            
        }
    }

    void ShowItemsNoLongerDetected()
    {
        //what is hidden that doesnt need to be anymore.
        int numHidden = curHidVisGs.Count;
   
        if (numHidden>0)
        {
            for (int i = 0; i<numHidden;i++)
            {
                if (curHidVisGs[i])
                {


                    VisGroup curVisG = curHidVisGs[i];
                    if (!curDetVisGs.Contains(curVisG) && curDetVisGs.Count > 0)
                    {
                        curHidVisGs.Remove(curVisG);
                        //show
                        curVisG.SetVis(true, VisGroupTransitionType.Partial);
                    }
                }
            }
        }

    }
    void HideItemsCurDetected()
    {
        int numDetected = curDetVisGs.Count;
        if (numDetected > 0)
        {
            for(int i = 0; i < numDetected; i++)
            {
                VisGroup curVisG = curDetVisGs[i];
                if (!curHidVisGs.Contains(curVisG))
                {
                    curHidVisGs.Add(curVisG);
                    //hide
                    curVisG.SetVis(false, VisGroupTransitionType.Partial);
                }
            }
        }
    }

    void ShowItems()
    {
        if(curDetVisGs.Count == 0)
        {
            if (curHidVisGs.Count > 0)
            {
                for (int i = 0; i<curHidVisGs.Count;i++)
                {
                    VisGroup curVisG = curHidVisGs[i];
                    curVisG.SetVis(true, VisGroupTransitionType.Partial);
                    curHidVisGs.Remove(curVisG);
                }
                
            }
        }
    }
    private void OnDrawGizmosSelected()
    {

        //Gizmos.DrawRay(cachedCameraPos, occludeDir);
        //Gizmos.DrawLine(cachedCameraPos, cachedTargetPos);
        Gizmos.DrawWireSphere(targetPos, overlapSphereRad);
        
        
    }
}
