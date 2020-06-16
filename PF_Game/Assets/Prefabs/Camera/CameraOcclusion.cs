using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusion : MonoBehaviour
{
    /// <summary>
    /// check if camera moves. 
    /// If moves, raycast.
    /// If hits something, check hidden list. If not in list add it.
    /// Hide all stuff on same angle? Hide all meshes within axis aligned radius.
    /// </summary>
    // Start is called before the first frame update


    Transform cachedTarget;
    Transform curTargetTransform;
    Vector3 camPosCached;
    Vector3 camPosCurrent;
    bool activeCheck;
    ElevationController elevationContoller;
    [SerializeField] List<LevelKitBase> curDetected = new List<LevelKitBase>();
    [SerializeField]List<LevelKitBase> curHidden = new List<LevelKitBase>();
    int curZoomFloor;
    int curTargetFloor;

    private void Awake()
    {
        elevationContoller = GameObject.FindObjectOfType<ElevationController>();
    }
    void Start()
    {
        activeCheck = true;
    }


    //Camera position change fire off a new focus transform target.
    public void UpdateViewTarget(Transform targetT)
    {
        
        
        cachedTarget = targetT;
        //StopCoroutine("CameraOccCheck");
        StartCoroutine("CameraOccCheck", 0.25f);
        
        //has camera moved? If yes, fire off the check again.

    }
    
    IEnumerator CameraOccCheck(float refreshRate)
    {
        

        
        while (activeCheck)
        {
            
            
            //has the camera moved? if yes, do an occlude check
            camPosCurrent = Camera.main.transform.position;
            if (camPosCurrent != camPosCached)
            {
                // current floor is not the same as the target, dont do anything. Measn player is zooming in and out and taking direct control
                curZoomFloor = elevationContoller.GetElevationLevel();
                curTargetFloor = elevationContoller.GetLevelFromYPos(cachedTarget.position.y);
                if (curZoomFloor==curTargetFloor)
                {
                    Debug.Log("Camera moved");
                    camPosCached = camPosCurrent;
                    OccludeCheck();
                }
               
            }
            yield return new WaitForSeconds(refreshRate);
        }
        
    }
    void OccludeCheck()
    {
        Debug.Log("Starting Check");
        Vector3 direction = (cachedTarget.position -camPosCached).normalized;
        float distance = Vector3.Distance(camPosCached, cachedTarget.position);


        curDetected.Clear();
        RaycastHit hit;
        if (Physics.Raycast(camPosCached, direction, out hit, distance))
        {
            
            if (hit.transform.GetComponentInParent<LevelKitBase>())
            {
                LevelKitBase curLevelKitHit = hit.transform.GetComponentInParent<LevelKitBase>();
                Debug.Log("Hit Level Kit asset: " + curLevelKitHit.name);
                if (!curDetected.Contains(curLevelKitHit))
                {
                    curDetected.Add(curLevelKitHit);
                    if (!curHidden.Contains(curLevelKitHit))
                    {
                        curHidden.Add(curLevelKitHit);
                        curLevelKitHit.ToggleVis(false, true);
                    }

                }
                
            }
            
        }
        if (curHidden.Count>0)
        {
            for(int i = 0;i<curHidden.Count;i++)
            {
                LevelKitBase kitPiece = curHidden[i];
                if (!curDetected.Contains(kitPiece))
                {
                    kitPiece.ToggleVis(true, true);
                    curHidden.Remove(kitPiece);
                }
            }
        }

        /*See what is between you and camera.
         * Hidden and currently detected.
         * is there anyhting in already hidden that is not in currently detected? Yes? Unhide it, remove from hidden.
         * Is there anything in currently detected that is not in already hidden? Yes? Hide it, add it to hidden.
         * 
         * Raycast
        hit something?
        yes? In hidden list?

        
        */

    }
    private void OnDrawGizmos()
    {
        if (cachedTarget!=null)
        {
            Gizmos.DrawLine(camPosCached, cachedTarget.position);
        }
        
    }
}
