﻿using System.Collections;
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
    PatrolPoint closestPoint;
    PatrolRoute closestRoute;
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
        FindClosestPatrol();        
        if (!patrol.GetPatrolState())
        {
            patrol.StartNewPatrol(closestRoute,unit.GetAvailableAP());
        }else if (patrol.GetPatrolState())
        {
            patrol.ContinuePatrol(unit.GetAvailableAP());
        }
    }
    void FindClosestPatrol()
    {
        /*Find all patrol points
         * Get Closest
         * Find the parent patrol
         * Start patrol
         */
        PatrolPoint[] allPatrolPoints = GameObject.FindObjectsOfType<PatrolPoint>();
        float closestPointDistance = 1000.0f;

        if (allPatrolPoints.Length != 0)
        {
            if (allPatrolPoints.Length == 1)
            {
                closestPoint = allPatrolPoints[0];
            }
            else if (allPatrolPoints.Length > 1)
            {
                foreach (PatrolPoint pp in allPatrolPoints)
                {
                    float dist = Vector3.Distance(transform.position, pp.transform.position);
                    if (dist < closestPointDistance)
                    {
                        closestPointDistance = dist;
                        closestPoint = pp;
                    }
                }
            }
            closestRoute = closestPoint.GetParentRoute();
        }
        
    }



}
