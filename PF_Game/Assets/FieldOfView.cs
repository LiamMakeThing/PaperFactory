using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FieldOfView : MonoBehaviour
{

    public float viewRadius = 10.0f;
    [Range(0, 360)]
    public float viewAngle;

    [SerializeField] LayerMask obstacleMask;
    [SerializeField] LayerMask targetMask;

    public List<Transform> visibleTargets;
    // Start is called before the first frame update


    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.25f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();

        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius,targetMask);

        for (int i = 0;i<targetsInViewRadius.Length;i++)
        {

            Transform target = targetsInViewRadius[i].transform;
            //get the direction to the target
            
            Vector3 directionToTaget = (target.position - transform.position).normalized;
            //compare the direction to the target to the view direction. If it is less than half the view angle, its within the fov.
            if (Vector3.Angle(transform.forward, directionToTaget) < viewAngle / 2)
            {
                //get the distance to the target
                float distToTarget = Vector3.Distance(transform.position, target.position);
                //if a raycast from here to the target doesnt hit anything on the obstacle mask, then it can see the target.
                if (!Physics.Raycast(transform.position, directionToTaget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }

        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees*Mathf.Deg2Rad),0.0f,Mathf.Cos(angleInDegrees*Mathf.Deg2Rad));
    }

}
