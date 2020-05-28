using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores locally the state of the node.
/// Has it been visited
/// Is it traversable
/// Where is it on the grid/what are it's coordinates
/// </summary>
public class Node : MonoBehaviour
{
    const int gridSize = 1;
    [SerializeField] Vector3 gridPosition;
    public bool isExplored;
    public Node exploredFrom;
    [SerializeField] bool isInPath;
    [SerializeField] GameObject pathIndicator;

    public bool isLink;
    //public float linkDestinationElevation;
    [SerializeField] List<Vector3> linkedNeighbourCoords = new List<Vector3>();
    MeshRenderer tileOverlay;

    public bool isWalkable;
    bool isVisibleByUnit;



    [Header("A* Relevant stuff")]
    public int gCost; //distance to target.
    public int hCost;//distance from start.

    public int fCost {
        get
        {
            return gCost + hCost;
        }
    }//total cost.


    public Vector3 GetGridPosition()
    {
        //Round positions To Grid and return new location
        gridPosition.x = Mathf.RoundToInt(transform.position.x / gridSize) * gridSize;
        gridPosition.y = Mathf.RoundToInt(transform.position.y / gridSize) * gridSize;
        gridPosition.z = Mathf.RoundToInt(transform.position.z / gridSize) * gridSize;

        //return new Vector3(gridPosition.x, gridPosition.y,gridPosition.z);
        return transform.position;
    }
    public List<Vector3> GetLinkedNeighbourCoords()
    {
        return linkedNeighbourCoords;
    }
    public void SetLinkedNeighbourCoords(List<Vector3> neighbourCoords)
    {
        linkedNeighbourCoords = neighbourCoords;
    }

    public void SetShowPathIndicator(bool state)
    {
        pathIndicator.SetActive(state);
        
    }
    private void Start()
    {
        string labelText = "[" + transform.position.x + "," + transform.position.y + "," + transform.position.z + "]";
        gameObject.name = "Node:" + labelText;
        tileOverlay = transform.Find("TileOverlay").GetComponent<MeshRenderer>();
        
        
    }

    public void SetDetectionLevel(bool state)
    {
        isVisibleByUnit = state;
        tileOverlay.enabled = state;
    }
    public bool GetDetectionLevel()
    {
        return isVisibleByUnit;
    }
    

    /*
    private void OnDrawGizmos()
    {
        //draw the bounds based on grid size to visually represent node without a mesh
        Vector3 tileSize = new Vector3(gridSize,0.0f,gridSize);
        UnityEditor.Handles.DrawWireCube(transform.position, tileSize);

    }
    */
    private void OnMouseEnter()
    {
        
    }
    private void OnMouseExit()
    {
        
    }
}
