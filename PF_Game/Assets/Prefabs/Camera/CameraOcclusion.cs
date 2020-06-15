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

    Vector3 cachedTarget;
    void Start()
    {

    }

    //Camera position change fire off a new focus transform target.
    public void UpdateViewTarget(Vector3 targetPos)
    {
        if (cachedTarget != targetPos||cachedTarget == null)
        {
            cachedTarget = targetPos;
            StartCoroutine("CameraOccCheck", 0.5f);
        }
    }

    IEnumerator CameraOccCheck(float refreshRate)
    {
        Debug.Log("Check");
        yield return new WaitForSeconds(refreshRate);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
