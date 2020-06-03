using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class NavLink : MonoBehaviour
{
    [SerializeField] List<Vector3> links = new List<Vector3>();
    [SerializeField] Color linkWireColor = new Color(0.4f,0.8f,1.0f,1.0f);
    Transform[] childrenTargets;
    // Start is called before the first frame update
    void Start()
    {
        
    
    }

    // Update is called once per frame
    void Update()
    {
        //Look for children targets, add them to links
        childrenTargets = GetComponentsInChildren<Transform>();
        AddLinkPositionsToList(childrenTargets);


      

    }
    public void DisableColliders()
    {
        this.GetComponent<Collider>().enabled = false;
        Collider[] childrenLinks = GetComponentsInChildren<Collider>();
        foreach (Collider c in childrenLinks)
        {
            c.enabled = false;
        }
    }
    void AddLinkPositionsToList(Transform[] tArray)
    { //ensure this object (the parent) is not in the list by starting the iteration at 1. GetComponentsInChildren searches the root as well so we have to strip it out.
        links = new List<Vector3>();
        for (int i = 1; i<tArray.Length;i++)
        {
            links.Add(tArray[i].position);
        }
    }
    public List<Vector3> GetLinkPositions()
    {

        return links;
    }
    private void OnDrawGizmos()
    {
        //Draw line connetcing nodes.

        //Handles.color = linkWireColor;
        /*for each link, check which is higher, the hub or the link. If 
        which ever one is lower, take the x and z pos and the y pos of the higher one. 
        Use this new vector as the coordinate for the joint sphere.
        Draw a line from the hub to the joint sphere and the joint sphere to the link
        */
        Gizmos.color = linkWireColor;

        bool hasJoint = false;
        foreach (Vector3 p in links)
        {
            Vector3 jointPos = new Vector3();
            if (p.y > transform.position.y)
            {
                hasJoint = true;
                jointPos.x = transform.position.x;
                jointPos.y = p.y;
                jointPos.z = transform.position.z;
            }else if (p.y < transform.position.y)
            {
                hasJoint = true;
                jointPos.x = p.x;
                jointPos.y = transform.position.y;
                jointPos.z = p.z;
            }
            else
            {
                hasJoint = false;
            }
            if (hasJoint)
            {
                Gizmos.DrawSphere(jointPos, 0.25f);
                Gizmos.DrawLine(transform.position, jointPos);
                Gizmos.DrawLine(jointPos, p);
            }else if (!hasJoint)
            {
                Gizmos.DrawLine(transform.position, p);
            }
            
            Gizmos.DrawWireCube(p, new Vector3(1.0f, 0.1f, 1.0f));
            
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(1.0f, 0.1f, 1.0f));

    }

}
