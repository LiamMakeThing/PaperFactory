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
        Gizmos.color = linkWireColor;
        //Handles.color = linkWireColor;
        foreach (Vector3 p in links)
        {
            
            Gizmos.DrawLine(p, transform.position);
            Gizmos.DrawWireCube(p, new Vector3(1.0f, 0.1f, 1.0f));
            
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(1.0f, 0.1f, 1.0f));

    }

}
