using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderAStar : MonoBehaviour
{
    Dictionary<Vector3, Node> grid = new Dictionary<Vector3, Node>();
    [SerializeField] List<Node> resolvedPath = new List<Node>();
    [SerializeField] Node nodeTemplate;
    enum NodeGenMode {CaptureExisting, SampleWalkable}
    [SerializeField] NodeGenMode nodeGenMode = new NodeGenMode();
    float gridSize = 1.0f;
    


    private void Awake()
    {
        LoadNodesIntoDictionary();
        //TODO Collect navLink positions, spawn them, pass through their neighbour nodes, set their isLink to true and add them to the dictionary as well. They are coming in as transforms so will just have to pull the position from each as we process it.

    }

    void LoadNodesIntoDictionary()
    {

        switch (nodeGenMode)
        {
            case NodeGenMode.CaptureExisting:
                var foundNodes = GameObject.FindObjectsOfType<Node>();
                foreach(Node n in foundNodes)
                {
                    grid.Add(n.GetGridPosition(), n);
                }

                break;
            case NodeGenMode.SampleWalkable:

                Transform gridRootTransform = transform.Find("Grid");
                var gridPatches = FindObjectsOfType<NavGridAutoGen>();

                foreach (NavGridAutoGen gridPatch in gridPatches)
                {
                    List<Vector3> patchPositions = gridPatch.GetNodePositions();
                    if (patchPositions.Count > 0)
                    {
                        foreach (Vector3 pos in patchPositions)
                        {
                            Node newNode = GameObject.Instantiate(nodeTemplate, pos, Quaternion.identity, gridRootTransform);
                            grid.Add(newNode.GetGridPosition(), newNode);

                        }
                    }
                }
                var navLinks = FindObjectsOfType<NavLink>();
                foreach (NavLink navLink in navLinks)
                {
                    Vector3 parentPos = navLink.transform.position;
                    List<Vector3> linkedPositions = navLink.GetLinkPositions();
                    
                    //Spawn node for parent pos and pass through the linkedPosition list as its neighbour(s)
                    Node parentLinkNode = GameObject.Instantiate(nodeTemplate, parentPos, Quaternion.identity, gridRootTransform);
                    parentLinkNode.isLink = true;
                    parentLinkNode.SetLinkedNeighbourCoords(linkedPositions);

                    grid.Add(parentLinkNode.GetGridPosition(), parentLinkNode);

                    //Colliders on nav links are required in editor to be picked up by auto gen but then need to be disabled on start.
                    navLink.DisableColliders();

                    //Spawn node(s) for linked pos and pass through the parentPos as their neighbour.LinkedNeighbours is nodes. Need to set these after spawning. Hmmm. No we just need to search by grid coordinate, not by node in search method.

                    foreach (Vector3 linkPos in linkedPositions)
                    {
                        Node linkedPosNode = GameObject.Instantiate(nodeTemplate, linkPos, Quaternion.identity, gridRootTransform);
                        linkedPosNode.isLink = true;
                        List<Vector3> linkedCoords = new List<Vector3>();
                        linkedCoords.Add(parentPos);
                        linkedPosNode.SetLinkedNeighbourCoords(linkedCoords);
                        grid.Add(linkedPosNode.GetGridPosition(), linkedPosNode);
                    }


                }
                break;
            default:
                break;
        }
        
           print("Grid: All nodes added. Nodes found:" + grid.Count);
    }
    public Node GetNodeFromGridPosition(Vector3 srcPosition)
    {
        //Round these values so they can be used outside with any position
        Vector3 key = new Vector3(
            (Mathf.Round(srcPosition.x / gridSize)*gridSize),
            (Mathf.Round(srcPosition.y / gridSize) * gridSize),
            (Mathf.Round(srcPosition.z / gridSize) * gridSize)
            );

        if (grid.ContainsKey(key))
        {
            Node nodeAtPos = grid[key];
            return nodeAtPos;
        }
        else
        {
            return null;
        }
        
    }
    public List<Node> GetPath(Vector3 startPos, Vector3 endPos)
    {
        Node startNode = GetNodeFromGridPosition(startPos);
        Node endNode = GetNodeFromGridPosition(endPos);
        //Create new sets. openset is the nodes we need to explore. Closed set is the nodes we have explored.
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //Start off by adding the start position to the openset. 
        openSet.Add(startNode);

        while (openSet.Count>0)//if there are neighbours to explore, keep exploring. Same as BFS.
        {
            Node currentNode = openSet[0];
            for (int i = 1;i<openSet.Count;i++)
            {
                if (openSet[i].fCost<currentNode.fCost||openSet[i].fCost==currentNode.fCost&&openSet[i].hCost<currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                resolvedPath = RetracePath(startNode,endNode);

                break;

            }
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.exploredFrom = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }

        }
        return resolvedPath;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currretNode = endNode;

        while (currretNode != startNode)
        {
            path.Add(currretNode);
            currretNode = currretNode.exploredFrom;
        }
        path.Add(startNode);
        path.Reverse();
        return path;

    }
    
    int GetDistance(Node nodeA, Node nodeB)
    {
        //costs
        int xDst = Mathf.Abs((int)nodeA.GetGridPosition().x - (int)nodeB.GetGridPosition().x);
        int zDst = Mathf.Abs((int)nodeA.GetGridPosition().z - (int)nodeB.GetGridPosition().z);

        if (xDst > zDst)
        {
            return 14 * zDst + 10 * (xDst - zDst);
        }else 
        {
            return 14 * xDst + 10 * (zDst-xDst);
        }
        
    }
    public List<Node> GetNeighbours(Node searchCenter)
    {
        List<Vector3> skipNeighbours = SearchCardinalDirections(searchCenter);

        List<Node> neighbours = new List<Node>();

        for(int x= -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                else
                {
                    int neighbourXPos = ((int)searchCenter.GetGridPosition().x) + x;
                    int neightbourYPos = ((int)searchCenter.GetGridPosition().z) + y;
                    int searchHeight = (int)searchCenter.GetGridPosition().y;

                    Vector3 neighbourCoordinate = new Vector3(neighbourXPos, searchHeight, neightbourYPos);
                    if (searchCenter.isLink)
                    {
                        List<Vector3> linkedNodeCoords = searchCenter.GetLinkedNeighbourCoords();
                        foreach(Vector3 linkedCoord in linkedNodeCoords)
                        {
                           neighbours.Add(grid[linkedCoord]);
                        }
                    }
                    if (grid.ContainsKey(neighbourCoordinate)&&!skipNeighbours.Contains(neighbourCoordinate))
                    {
                        neighbours.Add(grid[neighbourCoordinate]);
                    }

                }
            }
        }
        return neighbours;
    }
    List<Vector3> SearchCardinalDirections(Node searchCenter) //Seach NSEW of node. If there is not a node in those locations, add the corresponding diagonal neighbour coordinate to the skip list.
    {
        List<Vector3> skipNeighbours = new List<Vector3>();

        int searchCenterPosX = (int)searchCenter.GetGridPosition().x;
        int searchCenterPosY = (int)searchCenter.GetGridPosition().y;
        int searchCenterPosZ = (int)searchCenter.GetGridPosition().z;
        //check forward
        Vector3 searchCoordForward = new Vector3(searchCenterPosX, searchCenterPosY, searchCenterPosZ+1);
        //check backward
        Vector3 searchCoordBack = new Vector3(searchCenterPosX, searchCenterPosY, searchCenterPosZ-1);
        //check left
        Vector3 searchCoordLeft = new Vector3(searchCenterPosX-1, searchCenterPosY, searchCenterPosZ);
        //check right
        Vector3 searchCoordRight = new Vector3(searchCenterPosX + 1, searchCenterPosY, searchCenterPosZ);


        Vector3 forwardLeft = new Vector3(searchCenterPosX - 1,searchCenterPosY,searchCenterPosZ+1);
        Vector3 forwardRight = new Vector3(searchCenterPosX + 1, searchCenterPosY, searchCenterPosZ + 1);
        Vector3 backLeft = new Vector3(searchCenterPosX - 1, searchCenterPosY, searchCenterPosZ - 1);
        Vector3 backRight = new Vector3(searchCenterPosX + 1, searchCenterPosY, searchCenterPosZ - 1);
        
        if (!grid.ContainsKey(searchCoordForward))
        {
            skipNeighbours.Add(forwardLeft);
            skipNeighbours.Add(forwardRight);
        }
        if (!grid.ContainsKey(searchCoordBack))
        {
            skipNeighbours.Add(backLeft);
            skipNeighbours.Add(backRight);
        }
        if (!grid.ContainsKey(searchCoordLeft))
        {
            skipNeighbours.Add(forwardLeft);
            skipNeighbours.Add(backLeft);
        }
        if (!grid.ContainsKey(searchCoordRight))
        {
            skipNeighbours.Add(backRight);
            skipNeighbours.Add(forwardRight);
        }

        
        return skipNeighbours;
    }
}
