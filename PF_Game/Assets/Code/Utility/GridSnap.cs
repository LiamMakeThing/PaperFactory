using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class GridSnap : MonoBehaviour
{
    const float gridSnapSize = 0.5f;
    Vector3 snapPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {

        snapPos.x = Mathf.RoundToInt(transform.position.x / gridSnapSize) * gridSnapSize;
        snapPos.y = Mathf.RoundToInt(transform.position.y / gridSnapSize) * gridSnapSize;
        snapPos.z = Mathf.RoundToInt(transform.position.z / gridSnapSize) * gridSnapSize;
        transform.position = snapPos;   
    }
}
