using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleInEditor : MonoBehaviour
{
    enum Shape { box, sphere}
    [SerializeField] Shape debugShape;
    [SerializeField] Color wireColor;
    [SerializeField] Vector3 size;

    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
    

        Gizmos.color = wireColor;

        switch (debugShape)
        {
            case Shape.box:
                Gizmos.DrawWireCube(transform.position, size);
                break;
            case Shape.sphere:
                Gizmos.DrawWireSphere(transform.position, size.x);
                break;
            default:
                break;
        }
        {
            
        }
    }
}
