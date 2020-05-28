using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    PathfinderAStar pathfinder;
    PathRequester pathRequester;
    NavAgent navAgent;

    AICore parentAICore;
    [SerializeField]bool destinationReached;



    [SerializeField] List<Vector3> patrolPoints;
    [SerializeField] int nextPatrolPointIndex;
    [SerializeField] Vector3 curDestination;
    List<Node> currentPath;
    bool isPatroling;
    int availableAP;
    int patrolPointCount;

    private void Awake()
    {
        pathfinder = GameObject.FindObjectOfType<PathfinderAStar>();
        pathRequester = GameObject.FindObjectOfType<PathRequester>();
        navAgent = GetComponent<NavAgent>();
        parentAICore = GetComponent<AICore>();
    }
    private void Start()
    {
        
        
    }
    // Start is called before the first frame update
    public void StartNewPatrol(List<Vector3> waypoints,int unitAP)
    {
        isPatroling = true;
        patrolPoints = waypoints;
        //From their current position, they need to find the closest waypoint. 
        //Get its index, that is the first destination point.
        nextPatrolPointIndex = FindClosestPatrolPointIndex();
        curDestination = patrolPoints[nextPatrolPointIndex];

        //patrol has been initiated. Unit needs to being loop of moving to next waypoint.
        //If  not at a waypoint, get path to next waypoint from pathfinder.If path is longer than AP, means wont get there, Truncate path to the end of available AP. Get it cleaned up by path requester. Tell nav agent to move there up to its move speed. End turn
        //If not at a waypoint, get path to next waypoint ... if path is =to or less than available AP, means you will arrive there this turn. clean up path, tell nav agent to move. Set Destination reached flag to true.
        //If destination reached flag is true, Set destination to next waypoint. Get path, get it cleaned up, tell nav agent to move up to its move speed.
        ContinuePatrol(unitAP);
    }
    public bool GetPatrolState()
    {
        return isPatroling;
    }
    public void ContinuePatrol(int unitAP)
    {
        availableAP = unitAP;
        patrolPointCount = patrolPoints.Count - 1;
        if (destinationReached)
        {
            if (nextPatrolPointIndex+1 > patrolPointCount)
            {
                nextPatrolPointIndex = 0;
                
            }
            else
            {
                nextPatrolPointIndex = nextPatrolPointIndex + 1;
            }
            curDestination = patrolPoints[nextPatrolPointIndex];
            destinationReached = false;
        }
        ProceedToWaypoint(curDestination);

    }
    void ProceedToWaypoint(Vector3 destination)
    {
        List<Node> fullPath = pathfinder.GetPath(transform.position, destination);
        List<Node> budgetAwarePath = new List<Node>();
        if(fullPath.Count <= availableAP+1)
        {
            budgetAwarePath = fullPath;
            destinationReached = true;
        }
        else
        {
            budgetAwarePath = fullPath.GetRange(0, availableAP+1);
        }
        List<Vector3> currentPath = pathRequester.CleanUpPath(budgetAwarePath);
        Debug.Log(availableAP);
        navAgent.MoveNavAgent(currentPath);
        
        

    }
    int FindClosestPatrolPointIndex()
    {//find as the crow flies or pathfinding? Probably pathfinding cause they'll have to walk there anyway.

        //Shortest path would be the shortest list path from pathfinder. In a loop, request a path to each patrol point, compare their lengths.

        int pathLength = 1000;
        int patrolPointIndex = 0;
        for(int i = 0; i < patrolPoints.Count; i++)
        {
            Vector3 tempDestination = patrolPoints[i];
            
            List<Node> tempPath = pathfinder.GetPath(this.transform.position, tempDestination);
            int tempDistance = tempPath.Count;

            if (tempDistance < pathLength)
            {
                pathLength = tempDistance;
                patrolPointIndex = i;
                currentPath = tempPath;

            }

        }

        
        return patrolPointIndex;
    }


    private void OnDrawGizmos()
    {
        foreach(Vector3 pos in patrolPoints)
        {
            if(pos == curDestination)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
            
            Gizmos.DrawSphere(pos, 0.25f);

        }
    }

}
