using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    [SerializeField] Faction turnFaction;
    [SerializeField] Unit[] allUnits;

    [SerializeField] List<Unit> unitsByInit = new List<Unit>();
    int currentTurnIndex;
    int currentRoundIndex;

    CurrentUnitHandler unitHandler;
    

    private void Awake()
    {
        unitHandler = GetComponent<CurrentUnitHandler>();
        
    }


    private void Start()
    {
        CollectUnits();
    }

    void CollectUnits()
    {
        //Grab all units. Stash them in lists based on faction.
        //need to ensure there are no entries of the same value. S
        allUnits = GameObject.FindObjectsOfType<Unit>();
        unitsByInit = new List<Unit>(allUnits);
        unitsByInit.Sort(SortUnitsByInitiativeFunc);

        SetCurrentUnit();
        
    }
    private void Update()
    {
        //INPUT Hit enter and turn ends. 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(turnFaction == Faction.Player)
            {
                Debug.Log("Player ended their turn");
                AdvanceTurn();
            }
        }
    }
    void AdvanceTurn()
    {
        int maxTurnPerRound = unitsByInit.Count;
        if (currentTurnIndex + 1 >= maxTurnPerRound)
        {
            currentTurnIndex = 0;
            currentRoundIndex++;
        }
        else
        {
            currentTurnIndex++;
        }
        SetCurrentUnit();
        
    }

    void SetCurrentUnit()
    {
        Unit currentUnit = unitsByInit[currentTurnIndex];
        unitHandler.SetCurrentUnit(currentUnit);
    }
    private int SortUnitsByInitiativeFunc(Unit unitA, Unit unitB)
    {
        int initA = unitA.GetInitiative();
        int initB = unitB.GetInitiative();
        if (initA<initB)
        {
            return 1;
        }
        else if (initA > initB)
        {
            return -1;
        }
        else
        {
            return 0;
        }
        
    }

}
