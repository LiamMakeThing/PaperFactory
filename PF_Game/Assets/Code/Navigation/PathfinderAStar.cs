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

    //Data structures for skip neighbours
    List<Vector3> skipNeighbours = new List<Vector3>();

    //Clockwise starting at forward and ending at forward left.
    Vector3 [] searchGrid = new[] {
        new Vector3(0f,0f,1f),//0 Forward
        new Vector3(1f,0f,1f),//1 FR
        new Vector3(1f,0f,0f),//2 Right
        new Vector3(1f,0f,-1f),//3 BR
        new Vector3(0f,0f,-1f),//4 Back
        new Vector3(-1f,0f,-1f),//5 BL
        new Vector3(-1f,0f,0f),//6 Left
        new Vector3 (-1f,0f,1f)//7 FL
    };
    


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
                            //check to makes sure node not already there.
                            if (!grid.ContainsKey(pos))
                            {
                                Node newNode = GameObject.Instantiate(nodeTemplate, pos, Quaternion.identity, gridRootTransform);
                                grid.Add(newNode.GetGridPosition(), newNode);
                            }
                            

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
        /*PSEUDO
         * Check to see if the node is a link. If it is, pull neighbours from that.
         * If it is NOT a link, continue with rest of check
         * Diagonals are not available unless one of their flanking cardinals is available.
         * To check for this, we generate a list of viable cardinal candidates.
         * Loop-First check to see if any of the 4 cardinals are in the dictionary. 
         * If yes, check to see if it is obstructed by a wall or something with a raycast.
         * If not, it is a neighbour. Add it to the neighbour list and set the direction flag to be true.
         * 
         * New loop for the 4 diagonals
         * outside of the loop we run a block of ifs to check the diagonals.
         * Is diagonal in grid?
         * If yes, are both parent cardinals available?
         * if yes, is there an obstacle on it? (raycast)
         * if no, it is a neighbour.
         * 
         * 
         */


        List<Node> neighbours = new List<Node>();
        if (searchCenter.isLink)
        {
            List<Vector3> linkedNodeCoords = searchCenter.GetLinkedNeighbourCoords();
            foreach (Vector3 linkedCoord in linkedNodeCoords)
            {
                neighbours.Add(grid[linkedCoord]);
            }

        }
        Vector3 searchCenterPosition = searchCenter.GetGridPosition();
        Vector3 rayCastSource = searchCenterPosition + new Vector3(0, 0.25f, 0);
        //iterate through all cardinals. index 0,2,4,6 of search grid array
        for (int c = 0; c < 8; c = c + 2)
        {
            Vector3 candidate = searchCenterPosition + searchGrid[c];
            if (grid.ContainsKey(candidate))
            {
                //Cardinal is in dictionary, check for obstacles.

                if (!CheckForObstructions(rayCastSource, searchGrid[c]))
                {
                    //if there is no obstacle, add to neighbours
                    neighbours.Add(grid[candidate]);
                }
            }
        }
        //Iterate through diagonals
        for (int d = 1; d < 8; d = d + 2)
        {
            Vector3 candidate = searchCenterPosition + searchGrid[d];
            if (grid.ContainsKey(candidate))
            {
                switch (d)
                {
                    case 1://FR
                        if (grid.ContainsKey(searchGrid[0]) && grid.ContainsKey(searchGrid[2]))
                        {
                            if (!CheckForObstructions(rayCastSource, searchGrid[d]))
                            {
                                neighbours.Add(grid[candidate]);
                            }

                        }
                        break;
                    case 3://BR
                        if (grid.ContainsKey(searchGrid[4]) && grid.ContainsKey(searchGrid[2]))
                        {
                            if (!CheckForObstructions(rayCastSource, searchGrid[d]))
                            {
                                neighbours.Add(grid[candidate]);
                            }

                        }
                        break;
                    case 5://BL
                        if (grid.ContainsKey(searchGrid[4]) && grid.ContainsKey(searchGrid[6]))
                        {
                            if (!CheckForObstructions(rayCastSource, searchGrid[d]))
                            {
                                neighbours.Add(grid[candidate]);
                            }

                        }
                        break;
                    case 7://FL
                        if (grid.ContainsKey(searchGrid[0]) && grid.ContainsKey(searchGrid[6]))
                        {
                            if (!CheckForObstructions(rayCastSource, searchGrid[d]))
                            {
                                neighbours.Add(grid[candidate]);
                            }

                        }
                        break;
                }
            }
        }

        return neighbours;
    }
    bool CheckForObstructions(Vector3 source,Vector3 direction)
    {
        bool isObstructed = Physics.Raycast(source, direction, gridSize);
        
        return isObstructed;
    }
}
