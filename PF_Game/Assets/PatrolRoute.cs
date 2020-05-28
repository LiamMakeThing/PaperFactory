using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PatrolRoute : MonoBehaviour
{
    
    Transform[] childrenTransforms;
    [SerializeField] List<Vector3> patrolPositions;
    [SerializeField] int numPatrolPoints;
    private void Update()
    {
        //grab all children, add them to a list of transforms.
        //a unit when looking for a patrol will find the closest of these, then load the rest of the patrol points in this hierarchy.
        //Visualize the patrol points with a line and dots.
        childrenTransforms = GetComponentsInChildren<Transform>();
        SetPatrolPositions(childrenTransforms);
    }
    void SetPatrolPositions(Transform[] tArray)
    { //ensure this object (the parent) is not in the list by starting the iteration at 1. GetComponentsInChildren searches the root as well so we have to strip it out.

        patrolPositions = new List<Vector3>();
        for (int i = 1; i < tArray.Length; i++)
        {
            patrolPositions.Add(tArray[i].position);
        }
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
   

        for (int i = 0; i < patrolPositions.Count-1; i++)
        {
            Vector3 posB = new Vector3();
            if (i + 1 > patrolPositions.Count)
            {
                posB = patrolPositions[0];
            }
            else
            {
                
                posB = patrolPositions[i+1];


            }
            Gizmos.DrawLine(patrolPositions[i], posB);
            Gizmos.DrawLine(patrolPositions[0], patrolPositions[patrolPositions.Count-1]);
            

        }
        foreach(Vector3 pos in patrolPositions)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
    
}
