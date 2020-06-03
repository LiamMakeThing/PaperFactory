using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewDirection : MonoBehaviour
{
    NavAgent navAgent;
    Vector3 cachedDirection;
    Vector3 navAgentFacingDirection;
    private void Awake()
    {
        navAgent = GetComponentInParent<NavAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        navAgentFacingDirection = navAgent.GetFacingDirection();
        if (navAgentFacingDirection != cachedDirection)
        {
            cachedDirection = navAgentFacingDirection;
            transform.forward = -cachedDirection;
        }
    }
}
