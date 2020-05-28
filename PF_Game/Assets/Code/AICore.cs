using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AICore : MonoBehaviour
{
    PathfinderAStar pathfinder;
    PathRequester pathRequester;
    [SerializeField] LayerMask pathfindingLayer;
    
    NavAgent navAgent;
    Patrol patrol;
    [SerializeField] List<Vector3> patrolPoints;
    Unit unit;
    /// <summary>
    /// How the AI unit reacts to sensor stimuli, manages its perception level etc. Acts as a hub, connecting behaviours.
    /// </summary>
    // Start is called before the first frame update
    private void Awake()
    {
        pathfinder = GameObject.FindObjectOfType<PathfinderAStar>();
        pathRequester = GameObject.FindObjectOfType<PathRequester>();
        navAgent = GetComponent<NavAgent>();
        patrol = GetComponent<Patrol>();
        unit = GetComponent<Unit>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeTurn()
    {
        
        
        if (!patrol.GetPatrolState())
        {
            patrol.StartNewPatrol(patrolPoints,unit.GetAvailableAP());
        }else if (patrol.GetPatrolState())
        {
            patrol.ContinuePatrol(unit.GetAvailableAP());
        }
    }


    
}
