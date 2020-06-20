using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class GridSnap : MonoBehaviour
{
    [SerializeField] float gridSnapSize = 0.5f;
    [SerializeField] float rotSnapSize = 90.0f;
    Vector3 snapPos;
    Vector3 snapRot;
    [SerializeField] bool snapRotEnabled;
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
        snapPos.x = Mathf.RoundToInt(transform.position.x / gridSnapSize) * gridSnapSize;
        snapPos.y = Mathf.RoundToInt(transform.position.y / gridSnapSize) * gridSnapSize;
        snapPos.z = Mathf.RoundToInt(transform.position.z / gridSnapSize) * gridSnapSize;
        transform.position = snapPos;

        //Add rotation snap. 90 for now
        if (snapRotEnabled)
        {

        
        snapRot.x = Mathf.RoundToInt(transform.eulerAngles.x / rotSnapSize) * rotSnapSize;
        snapRot.y = Mathf.RoundToInt(transform.eulerAngles.y / rotSnapSize) * rotSnapSize;
        snapRot.z = Mathf.RoundToInt(transform.eulerAngles.z / rotSnapSize) * rotSnapSize;
        transform.eulerAngles = snapRot;
        }
    }
}
