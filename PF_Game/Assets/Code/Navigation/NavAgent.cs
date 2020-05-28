using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : MonoBehaviour
{
    //List<Node> nodePathToFollow;
    List<Vector3> posPathToFollow;
    //[SerializeField] List<Node> tempManualPath = new List<Node>();
    [SerializeField]float moveSpeed = 0.10f;
    int targetNodeIndex;
    PathfinderAStar pathfinder;
    public Vector3 facingDirection;
    //is given a path and follows it.

    private void Awake()
    {
        pathfinder = GameObject.FindObjectOfType<PathfinderAStar>();
    }
    private void Start()
    {
        
    }



    public void MoveNavAgent(List<Vector3> path)
    {
        //nodePathToFollow = tempManualPath;
        //Convert node list to vector list.

        posPathToFollow = path;
        StartCoroutine(BeginFollowingPath());
    }
 

    IEnumerator BeginFollowingPath()
    {
        //Path Received. Beginning movement
        yield return StartCoroutine(FollowPath());
        //Path Complete
    }
    IEnumerator FollowPath()
    {
        targetNodeIndex = 0;
        Vector3 currentNodePos = posPathToFollow[0];
        

        while (true)
        {
            if(transform.position == currentNodePos)
            {
                //we have arrived at a location. At this point it will be either a node or an inserted bridge point. We need to check the former to see if it is visible by other units by accessing the node via the navgrid dictionary.
                if (pathfinder.GetNodeFromGridPosition(currentNodePos) != null)
                {
                    Node currentNode = pathfinder.GetNodeFromGridPosition(currentNodePos);
                    if (currentNode.GetDetectionLevel())
                    {
                     //TODO. Unit is visible. Should probably do something about that
                    }
                    currentNode.SetShowPathIndicator(false);
                }
                

                targetNodeIndex++;
                
                //if next position is higher than this position, move up. Insert a new position first then continue

                //continuing.

                if (targetNodeIndex>=posPathToFollow.Count)
                {
                    
                    yield break;

                }

                currentNodePos = posPathToFollow[targetNodeIndex];
                
            }
            transform.position = Vector3.MoveTowards(transform.position, currentNodePos, moveSpeed * Time.deltaTime);

            facingDirection = Vector3.Normalize(transform.position - currentNodePos);
            
            yield return null;
        }

    }
 
}
