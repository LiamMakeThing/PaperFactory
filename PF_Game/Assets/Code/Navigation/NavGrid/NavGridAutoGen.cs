using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
///  Creates box of custom bounds.
///  Box divides into matrix of node size
///  node positions trace down. If they hit a walkable env object, store the position. If nothing is hit, forget it.
///  At run time, the Grid Dictionary is populated by the stored positions as keys and a spawned node as the value.
///  Can use multipl of these to generate a level of patches?
/// </summary>
[ExecuteInEditMode]


public class NavGridAutoGen : MonoBehaviour
{
    int layerMask;
    [SerializeField] Vector2Int gridScale = new Vector2Int(3, 3);
    [SerializeField] List<Vector3> gridPositions2D = new List<Vector3>();
    [SerializeField] List<Vector3> nodePositions = new List<Vector3>();
    [SerializeField] int gridSize = 1;
    [SerializeField] bool activeCheck;
    [SerializeField] float searchRange = 100.0f;
    [SerializeField] Color nodePosColor = new Color(0.2f,0.6f,0.6f);
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }
        
        layerMask =~  LayerMask.GetMask("Unit");
        transform.position = RoundPosition(transform.position);
        ClearNodePositions();
        CreateNewNodePositions();
        CheckForViableNodePosition();
    }
    Vector3 RoundPosition(Vector3 postionToRound)
    {
        
        int posX = Mathf.RoundToInt(postionToRound.x / gridSize) * gridSize;
        int posY = Mathf.RoundToInt(postionToRound.y / gridSize) * gridSize;
        int posZ = Mathf.RoundToInt(postionToRound.z / gridSize) * gridSize;
        Vector3 roundedTransformPosition = new Vector3(posX, posY, posZ);
        return roundedTransformPosition;
    }

    void ClearNodePositions()
    {
        nodePositions.Clear();
        nodePositions = new List<Vector3>();
        gridPositions2D.Clear();
        gridPositions2D = new List<Vector3>();
    }
    void CreateNewNodePositions()
    {
        for(int x = 0; x < gridScale.x; x++)
        {
            for(int y = 0; y < gridScale.y; y++)
            {
                Vector3 newPos = new Vector3(x-gridScale.x/2, 0, y-gridScale.y/2) + RoundPosition(transform.position);
                gridPositions2D.Add(newPos);
            }
        }
    }
    void CheckForViableNodePosition()
    {
        if (activeCheck)
        {
            foreach(Vector3 pos in gridPositions2D)
            {
                //line trace down.
                RaycastHit hit;
                // BIT SHIFT LAYER 8 BY 1 TO ENSURE LAYER 8 RETURNS A HIT AND ONLY LAYER 8. MEANS THE BIT IN THE 8TH POSITION IS 1 INSTEAD OF 0.
                
                if (Physics.Raycast(pos,Vector3.down,out hit,searchRange,layerMask))
                {
                    if(hit.transform.tag == "Walkable")
                    {
                        Vector3 nodePos = RoundPosition(hit.point);
                        nodePositions.Add(nodePos);
                    }
                    
                   
                }
            }
        }
    }
    public List<Vector3> GetNodePositions()
    {
        return nodePositions;
    }
    private void OnDrawGizmosSelected()
    {
        float padding = 0.1f;
        Vector3 gizmoPosWithOffset = transform.position + new Vector3(-0.5f, 0.0f, -0.5f);
        Gizmos.DrawWireCube(gizmoPosWithOffset, new Vector3(gridScale.x, 0, gridScale.y));
        foreach(Vector3 pos in gridPositions2D)
        {
           // Handles.DrawWireCube(pos, new Vector3(gridSize-padding,0.0f, gridSize - padding));
        }
        Gizmos.color = nodePosColor;
        foreach (Vector3 pos in nodePositions)
        {
            Gizmos.DrawWireCube(pos, new Vector3(gridSize - padding, 0.0f, gridSize - padding));
        }
    }
   
}
