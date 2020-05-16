using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathRequester : MonoBehaviour
{
    [SerializeField] Vector3 startPosition;
    [SerializeField] Vector3 endPosition;
    PathfinderAStar pathFinder;
    PathRenderer pathRenderer;
    List<Vector3> finalPath = new List<Vector3>();
    CurrentUnitHandler unitHandler;
    [SerializeField] Transform destinationCursor;
    bool isDestinationCursorActive;
    Dictionary<int, Vector3> bridgePositions = new Dictionary<int, Vector3>();
    // Start is called before the first frame update

    private void Awake()
    {
        pathFinder = GetComponent<PathfinderAStar>();
        pathRenderer = GetComponent<PathRenderer>();
        unitHandler = GameObject.FindObjectOfType<CurrentUnitHandler>();

    }

    // Update is called once per frame

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Vector3 tempPos = hit.transform.position;
            if (hit.transform.GetComponent<Node>())
            {
                
                UpdateDestinationCursor(tempPos, true);

                if (Input.GetButtonDown("LeftClick"))
                {         
                    if (endPosition != tempPos)
                    {
                        endPosition = tempPos;
                        RequestPath();
                    }
                    
                }
            }else if (!hit.transform.GetComponent<Node>())
            {
                if (isDestinationCursorActive)
                {
                    UpdateDestinationCursor(tempPos, false);
                }
                
            }
        }
     
    }
    void RequestPath()
    {
        Unit currentUnit = unitHandler.GetCurrentUnit();
        
        startPosition = currentUnit.transform.position;

        List<Node> rawPath =pathFinder.GetPath(startPosition, endPosition);

        finalPath = new List<Vector3>();
        finalPath = CleanUpPath(rawPath);

        currentUnit.GetComponent<NavAgent>().MoveNavAgent(finalPath);
    }

    List<Vector3> CleanUpPath(List<Node> rawPath) // TAKES IN REQUESTED PATH FROM PATHFINDER. EVALUATES FOR ELEVATION...AND ANYTHING ELSE, AND PLOTS ALL POINTS BEFORE COMMITTING THE MOVE TO THE NAV AGENT.
    {
   
        //CONVERT PATH TO VECTOR LIST, instead of nodes

        List<Vector3> cleanPath = ConvertPathListType(rawPath);
        bridgePositions = new Dictionary<int, Vector3>();

        //identify new positions first. then add them. 
        for (int i = 0; i < cleanPath.Count; i++)
        {
            
            
            if (i + 1 < cleanPath.Count)
            {
                Vector3 currentPos = cleanPath[i];
                Vector3 nextPos = cleanPath[i + 1];
                

                if (nextPos.y>currentPos.y)
                {
                    //moving up
                    Vector3 bridgePos = new Vector3(currentPos.x, nextPos.y, currentPos.z);
                    int indexOffset = bridgePositions.Count;
                    bridgePositions.Add(i + indexOffset, bridgePos);
                    
                    
                }
                else if (nextPos.y<currentPos.y)
                {
                    
                        //moving down
                    Vector3 bridgePos = new Vector3(nextPos.x, currentPos.y, nextPos.z);
                    int indexOffset = bridgePositions.Count;
                    bridgePositions.Add(i + indexOffset, bridgePos);


                }

            }
            
        }
        //combine 
        foreach (int key in bridgePositions.Keys)
        {
            cleanPath.Insert(key+1, bridgePositions[key]);
        }

        return cleanPath;
    }

    List<Vector3> ConvertPathListType(List<Node> nodeList)//IN ORDER TO INSERT NEW POSITIONS IN THE PATH WE NEED TI CONVERT IT TO VECTOR3S INSTEAD OF NODES.
    {
        List<Vector3> positionList = new List<Vector3>();
        foreach (Node n in nodeList)
        {
            positionList.Add(n.GetGridPosition());
        }
        return positionList;
    }
 

    void UpdateDestinationCursor(Vector3 pos, bool activeState)
    {
        destinationCursor.position = pos;
        destinationCursor.gameObject.SetActive(activeState);
        isDestinationCursorActive = activeState;
    }

    private void OnDrawGizmos()
    {
     //DRAW THE CLEANED UP PATH WITH POINTS AND LINES.
     for (int i = 0; i < finalPath.Count; i++)
        {
            Gizmos.DrawSphere(finalPath[i], 0.25f);
            if ((i + 1) < finalPath.Count)
            {
                Gizmos.DrawLine(finalPath[i], finalPath[i + 1]);
            }
        }
        foreach (int key in bridgePositions.Keys)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(bridgePositions[key], 0.25f);
        }
    }


}
