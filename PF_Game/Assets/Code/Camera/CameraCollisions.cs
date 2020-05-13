using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisions : MonoBehaviour
{
    [SerializeField] float minDist;
    [SerializeField] float maxDist;
    [SerializeField] float smooth;
    Vector3 dollyDir;
    Vector3 dollyDirAdjusted;
    float distance;

    private void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDist);
        RaycastHit hit;

        if(Physics.Linecast(transform.parent.position,desiredCameraPos,out hit))
        {
            distance = Mathf.Clamp((hit.distance * 0.75f), minDist, maxDist);

        }
        else
        {
            distance = maxDist;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
    
}
