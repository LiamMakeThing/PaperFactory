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


    [SerializeField] Transform cachedTarget;
    Transform curTargetTransform;
    Vector3 camPosCached;
    Vector3 camPosCurrent;
    bool activeCheck;
    ElevationController elevationContoller;
    [SerializeField] List<LevelKitBase> curOverlapItems = new List<LevelKitBase>();
    [SerializeField] List<LevelKitBase> curHidden = new List<LevelKitBase>();
    int curZoomFloor;
    int curTargetFloor;
    Vector3 hitPos;
    [SerializeField] float sphereOverlapRadius = 5.0f;
    [SerializeField] float camPosTolerance = 0.5f;
    
    [SerializeField] bool hitDetected;

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
        StartCoroutine("CameraOccCheck", 0.025f);
        
        //has camera moved? If yes, fire off the check again.

    }
    
    IEnumerator CameraOccCheck(float refreshRate)
    {
        

        
        while (activeCheck)
        {
            /*The camera move damping means that the camera is moving slightly, after the input has ended. Which is good. However this means that the check is ongoing and takes a while to 'settle.' 
             * Instead of using an "if different" check to fire off occludechecks, do a distance greater than tolerance check.
             */


            

            camPosCurrent = Camera.main.transform.position;
            float camDist = Vector3.Distance(camPosCurrent, camPosCached);

            if (camDist>camPosTolerance)
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
        hitDetected = false;
        
        Vector3 direction = ((cachedTarget.position + new Vector3(0,1,0)) - camPosCached).normalized;
        float distance = Vector3.Distance(camPosCached, cachedTarget.position);

        curOverlapItems = new List<LevelKitBase>();

        RaycastHit hit;
        if (Physics.Raycast(camPosCached, direction, out hit, distance))
        {
            hitDetected = true;
            hitPos = hit.point;

            Collider[] overlappedColliders = Physics.OverlapSphere(hitPos,sphereOverlapRadius);
            foreach(Collider c in overlappedColliders)
            {
                if (c.GetComponentInParent<LevelKitBase>())
                {
                    LevelKitBase tempKitAsset = c.GetComponentInParent<LevelKitBase>();
                    //Check to make sure its not the floor under the current unit. Y pos is lower?
                    if (!tempKitAsset.GetIsFloor())
                    {
                      
                            //check to see if the asset is closer to the camera than the target. 
                            if (!curOverlapItems.Contains(tempKitAsset))
                            {


                                curOverlapItems.Add(tempKitAsset);


                            }
                        
                    }
                    
                }
            }
        }

        //check to see what should be unhidden. (What is in hidden list that is not currently being overlapped)

        if (curHidden.Count > 0)
        {

            for(int i = 0;i<curHidden.Count;i++)
            {
                LevelKitBase curKitAsset = curHidden[i];
                if (!curOverlapItems.Contains(curKitAsset))
                {
                    curHidden.Remove(curKitAsset);
                    curKitAsset.ToggleVis(true, true);
                }
            }
        }
        foreach(LevelKitBase tempKitAsset in curOverlapItems)
        {
            if (!curHidden.Contains(tempKitAsset))
            {
                curHidden.Add(tempKitAsset);
                tempKitAsset.ToggleVis(false, true);
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

    
    private void OnDrawGizmosSelected()
    {
        float debugSphereRad = 0.0f;
        if (cachedTarget!=null)
        {
            Gizmos.DrawLine(camPosCached, cachedTarget.position);
        }
        if (hitDetected)
        {
            debugSphereRad = sphereOverlapRadius;
        }else if (!hitDetected)
        {
            debugSphereRad = 0.0f;
        }
        Gizmos.DrawWireSphere(hitPos, debugSphereRad);

    }
}
