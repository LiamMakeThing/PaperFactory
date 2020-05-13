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

    //is given a path and follows it.

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
        Debug.Log("Path Received. Beginning movement");
        yield return StartCoroutine(FollowPath());
        Debug.Log("Path Complete");
    }
    IEnumerator FollowPath()
    {
        targetNodeIndex = 0;
        Vector3 currentNodePos = posPathToFollow[0];
        

        while (true)
        {
            if(transform.position == currentNodePos)
            {



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
            
            yield return null;
        }

    }
 
}
