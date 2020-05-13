using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Acts as a storage unit for paths.
/// Controls adding and removing of paths via the undo and redo.
/// Stores a master list and an active list
/// Commits active list to master list and pushes the length of that list a stack of integers.
/// Undoing, pops the top integer and removes that amount of nodes from the master list.
/// 
/// 
/// </summary>
/// 


    //GET AP AVAILABLE FROM ACTIVE UNIT. PASS IT TO PATHFINDER. IT SHOULD RETURN THE PATH THAT IT WAS ASKED OR, IF THE AP IS NOT HIGH ENOUGH, THE PATH UP UNTIL THE COST.

//Test one. Push and pop ints getting message from interaction layer
public class PathBuilder : MonoBehaviour
{
    [SerializeField] PathRenderer pathRendererStatic;
    [SerializeField] PathRenderer pathRendererDynamic;
    PathfinderAStar pathfinder;

    Stack<int> commitStack = new Stack<int>();
    [SerializeField] GameObject cursorWayPoint;
    Stack<GameObject> waypointStack = new Stack<GameObject>();
    [SerializeField] int totalTilesCounty;
    List<Node> livePath = new List<Node>();
    [SerializeField]List<Node> staticPath = new List<Node>();
    List<Node> adjustedAPPath = new List<Node>();
    Node lastLiveNode;

    Node livePathStartNode;
    [SerializeField] Node lastStaticNode;
    Node startNode;
    Node destinationNode;
    [SerializeField] Transform cursorDestination;
    [SerializeField] Transform cursorStartPosition;


    [SerializeField] float verticalOffset = 0.1f;
    [SerializeField] NavAgent navAgentToMove;
    UnitBase activeUnit;
    

    //pathbuilder. NEW PATH. Set start or wahtever to the players position. Set the AP limits etc. 

    private void Start()
    {
        pathfinder = GetComponent<PathfinderAStar>();
        cursorStartPosition.gameObject.SetActive(false);

    }

    public void MakeNewPath(UnitBase unit)
    {
        ResetAndClear();
        staticPath = new List<Node>();
        activeUnit = unit;
        navAgentToMove = activeUnit.GetComponent<NavAgent>();
        //startNode = navAgentToMove.GetCurrentNode();
        lastStaticNode = startNode;
        cursorStartPosition.position = startNode.GetGridPosition(); //startNode.GetGridPosition() + new Vector3(0, verticalOffset, 0);
        cursorStartPosition.gameObject.SetActive(true);
    }
  
    
    public void UpdateLivePath(Node endNode)
    {
        if (activeUnit.GetAvailableAP() > 0)
        {

        
            if (!cursorDestination.gameObject.activeSelf)
            {
                cursorDestination.gameObject.SetActive(true);
            }

            livePath.Clear();
            SetLivePathStartAndEnd(endNode);
            //livePath = pathfinder.GetPath(livePathStartNode, endNode);//pathfinder create path

            adjustedAPPath = EvaluatePathForAP(livePath);
            pathRendererDynamic.UpdatePathRenderer(adjustedAPPath);//update path renderer
        
            cursorDestination.position = lastLiveNode.GetGridPosition() + new Vector3(0, verticalOffset, 0);//destination cursor is always on the destination node.

        }
    }


    void SetLivePathStartAndEnd(Node endNode)
    {
        destinationNode = endNode; //Set start node from moused over node from Interaction Layer
        livePathStartNode = lastStaticNode;//set the start node for the live paht. Should be the last node in the static path or, if the static path is empty, the players current position.
    }
    List<Node> EvaluatePathForAP(List<Node> originalPath)
    {
        
        int availableAP = activeUnit.GetAvailableAP();
        List<Node> evaluatedAPPath = new List<Node>();
        if (originalPath.Count>availableAP)
        {
            evaluatedAPPath = originalPath.GetRange(0, availableAP+1);
        }
        else
        {
            evaluatedAPPath = originalPath;
        }
        lastLiveNode = evaluatedAPPath[evaluatedAPPath.Count - 1];
        return evaluatedAPPath;

    }



    public void AddPath()
    {
        if (commitStack.Count == 0)
        {
            //this is the first push so set the initial position to retreive it later.
            startNode = livePath[0];
        }
        List<Node> pathToAdd = adjustedAPPath;
        int pathToAddLength = pathToAdd.Count;
        commitStack.Push(pathToAddLength);
        staticPath.AddRange(pathToAdd);

        lastStaticNode = staticPath[staticPath.Count - 1];

        pathRendererStatic.UpdatePathRenderer(staticPath);

        PlaceWaypoint();
        int apCost = pathToAddLength - 1;
        AdjustAP(-apCost);
    }

    void AdjustAP(int apCost)
    {
        activeUnit.AdjustAvailableAP(apCost);
    }

    void PlaceWaypoint()
    {
        GameObject newWaypoint = Instantiate(cursorWayPoint, lastLiveNode.GetGridPosition() + new Vector3(0, verticalOffset + 0.01f, 0), Quaternion.identity);
        waypointStack.Push(newWaypoint);
    }
    public void RemovePath()
    {
        
        

        if (commitStack.Count > 1)
        {
            int pathToRemoveCount = commitStack.Pop();
            int threshold = staticPath.Count - pathToRemoveCount;
            staticPath.RemoveRange(threshold, staticPath.Count - threshold);
            lastStaticNode = staticPath[staticPath.Count -1];
        
            UpdateLivePath(destinationNode);
            pathRendererStatic.UpdatePathRenderer(staticPath);

            AdjustAP(pathToRemoveCount-1);
        }
        else if (commitStack.Count == 1)
        {
            int pathToRemoveCount = commitStack.Pop();
            int threshold = staticPath.Count - pathToRemoveCount;
            staticPath.RemoveRange(threshold, staticPath.Count - threshold);
            lastStaticNode = startNode;
            UpdateLivePath(destinationNode);
            pathRendererStatic.UpdatePathRenderer(staticPath);

            AdjustAP(pathToRemoveCount-1);
        }
        else if (commitStack.Count == 0)
        {
            print("No more paths in stack. Nothing to undo.");
            //Shut off movement?
        }
      
       
        GameObject waypointToRemove = waypointStack.Pop();
        Destroy(waypointToRemove);
    }
    public void CommitMovement(){

       // navAgentToMove.TraversePath(staticPath);
        pathRendererStatic.RemovePath();
    }

    public void NavAgentMovementComplete()
    {
        ResetAndClear();
    }

    //WIPE ALL OF IT. CALLED WITH NEW PATHS AND COMPLETION OF EXISTING PATHS
    void ResetAndClear()
    {
        ClearWaypoints();
        ClearStaticPath();
        ClearDynamicPath();
        //ClearCommitStack();
        DisableCursors();
    }
    void ClearDynamicPath()
    {
        pathRendererDynamic.RemovePath();
    }
    void ClearStaticPath()
    {
        
        pathRendererStatic.RemovePath();
        commitStack.Clear();
        staticPath.Clear();
    }
    void ClearWaypoints()
    {
        foreach (GameObject go in waypointStack)
        {
            Destroy(go);
        }
        waypointStack.Clear();
    }
    void DisableCursors()
    {
        cursorDestination.gameObject.SetActive(false);
        cursorStartPosition.gameObject.SetActive(false);
    }

}
