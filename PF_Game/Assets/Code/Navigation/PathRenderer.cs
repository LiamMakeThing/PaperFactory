using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gets path from pathfinder. 
/// Creates mesh line to connect nodes
/// </summary>
/// 
//[ExecuteInEditMode] //Todo remove editor mode. Only for debug
public class PathRenderer : MonoBehaviour
{
    Vector3[] newVerts;
    List<Node> path; //todo. Replace this with a pull from the pathfinder
    [SerializeField] float verticalOffset = 0.1f;
    Vector2[] newUV;
    int[] newTris;
    Mesh pathMesh;
    List<Vector3> borderVerts;
    //List<Vector3> originalPathPoints;
    List<Vector3> bevelledPathPoints;
    [SerializeField] bool drawGizmos;
    
    

    [SerializeField] float lineWidth = 0.25f;
    [SerializeField] float bevelSize = 0.1f;
    // Start is called before the first frame update
    private void Awake()
    {
        pathMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = pathMesh;
    }

    public void RemovePath()
    {
        pathMesh.Clear();
    }

    public void UpdatePathRenderer(List<Node> pathNodesFromPathfinder)
    {
      
        
        
      
        path = pathNodesFromPathfinder;

        DefinePathPoints();
        GetBorderVerts();
        SortTriangles();
        UpdateMesh();
    }

    void DefinePathPoints()
    {
        //for bevelling corners. First Node needs one after it, last node needs one before it, other nodes need both. points are added at search distance along forward directions, previous forward and next forward.
        //add original path points to a list, for each one, add the relavant new bevel points to a new list in the correct increasing index.
        bevelledPathPoints = new List<Vector3>();
        //for each point in path position, get its direction, add forward and backward facing points, add to bevelled list in proper index
        for (int i = 0; i < path.Count; i++)
        {
            if(i == 0)
            {
                //first point
                //only search forward. Insert point at index +1
                /*get direction.
                 * get location at point location + forward direction * search distance
                 *get location at point location - previous point direction * search distance 
                 * 
                 */
                Vector3 posA = path[i].GetGridPosition() + new Vector3(0,verticalOffset,0);
                Vector3 posB = path[i+1].GetGridPosition() + new Vector3(0, verticalOffset, 0); 
                Vector3 bevelPointB = posA + (GetLineDirection(posA, posB) * bevelSize);

                bevelledPathPoints.Insert(i, posA);
                bevelledPathPoints.Add(bevelPointB);
            }
            else if (i == path.Count - 1)//last point
            {
                //don't have to add bevel points. Just add last.
                bevelledPathPoints.Insert(bevelledPathPoints.Count, path[i].GetGridPosition() + new Vector3(0, verticalOffset, 0));
            }
            else//all other points
            {
                //TODO filter out inline points to reduce complexity of mesh.
                //search forward and backward. Insert point at index -1 and index +1
                Vector3 posA = path[i - 1].GetGridPosition() + new Vector3(0, verticalOffset, 0); ;
                Vector3 posB = path[i].GetGridPosition() + new Vector3(0, verticalOffset, 0); ;
                Vector3 posC = path[i + 1].GetGridPosition() + new Vector3(0, verticalOffset, 0); ;

                Vector3 bevelPointA = posB - (GetLineDirection(posA, posB) * bevelSize);
                Vector3 bevelPointB = posB + (GetLineDirection(posB, posC) * bevelSize);

                bevelledPathPoints.Add(bevelPointA);
          
                bevelledPathPoints.Add(bevelPointB);
            }
        }
    }
    public Vector3 GetLineDirection(Vector3 posA, Vector3 posB)
    {
        Vector3 dir = (posB - posA).normalized;
        return dir;
    }
    void GetBorderVerts()
    {
        borderVerts = new List<Vector3>();
        for (int i = 0;i<bevelledPathPoints.Count;i++)
        {
            
            Vector3 posA;
            Vector3 posB;
            if (i == bevelledPathPoints.Count-1)
            {
                //last two points
                int lastIndex = bevelledPathPoints.Count-1;
                posB = bevelledPathPoints[lastIndex];
                posA = bevelledPathPoints[lastIndex - 1];
                borderVerts.Add(posB + (Quaternion.Euler(0, 90, 0) * GetLineDirection(posA, posB) * lineWidth));
                borderVerts.Add(posB - (Quaternion.Euler(0, 90, 0) * GetLineDirection(posA, posB) * lineWidth));
            }
            else
            {
                //startPoints
                posA = bevelledPathPoints[i];
                posB = bevelledPathPoints[i + 1];
                borderVerts.Add(posA + (Quaternion.Euler(0, 90, 0) * GetLineDirection(posA, posB) * lineWidth));
                borderVerts.Add(posA - (Quaternion.Euler(0, 90, 0) * GetLineDirection(posA, posB) * lineWidth));
            }
        }
    }
    void SortTriangles()
    {
        newVerts = borderVerts.ToArray();

        List<int> triangles = new List<int>();
        int numTriangles = (bevelledPathPoints.Count - 1)*2;
        //Triangles side A
        for(int i = 0; i < numTriangles / 2; i++)
        {
            //triangles direction 1
        
            triangles.Add(i*2+1);
            
            triangles.Add((i+1)*2);
            triangles.Add(i * 2);

            //triangles direction 2
            triangles.Add(i*2+1);
            
            triangles.Add(i*2+3);
            triangles.Add((i + 1) * 2);
        }

        ////Triangles side B
        //
        newTris = triangles.ToArray();

    }
    void UpdateMesh()
    {

        RemovePath();
        pathMesh.vertices = newVerts;
        pathMesh.uv = newUV;
        pathMesh.triangles = newTris;
    }


    /*
    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            foreach (Vector3 pos in borderVerts)
            {
                Vector3 vertLocalPos = pos + transform.position;
                UnityEditor.Handles.DrawWireDisc(vertLocalPos, Vector3.up, 0.1f);
            }
            foreach (Vector3 pos in bevelledPathPoints)
            {
                Vector3 vertLocalPos = pos + transform.position;
                UnityEditor.Handles.DrawWireCube(vertLocalPos, new Vector3(0.1f, 0.1f, 0.1f));

            }
        }
    }
    */
}
