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

    //ActiveTurnUnit
    [SerializeField] Unit currentlyFocusedUnit;
    //This are different. You can focus on any unit, assuming it is the players turn. The only unit who can take action however is the activeturnunit.
    

    private void Awake()
    {
        unitHandler = GetComponent<CurrentUnitHandler>();
        CollectUnits();
    }


    private void Start()
    {
        UpdateCurrentlyFocusedUnit(unitsByInit[0]);
    }

    void CollectUnits()
    {
        //Grab all units. Stash them in lists based on faction.
        //need to ensure there are no entries of the same value. S
        allUnits = GameObject.FindObjectsOfType<Unit>();
        unitsByInit = new List<Unit>(allUnits);
        unitsByInit.Sort(SortUnitsByInitiativeFunc);

        

        
    }
    private void Update()
    {
        //INPUT Hit enter and turn ends. 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UserInputAdvanceTurn();
        }
    }

    public List<Unit> GetInitiativeSortedUnits()
    {
        return unitsByInit;
    }

    public void UserInputAdvanceTurn()
    {
        if (turnFaction == Faction.Player)
        {
            AdvanceTurn();
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
 
            
    }

   public void UpdateCurrentlyFocusedUnit(Unit newFocusUnit)
    {
        if (currentlyFocusedUnit != newFocusUnit||currentlyFocusedUnit==null)
        {
            currentlyFocusedUnit = newFocusUnit;
            unitHandler.SetCurrentlyFocusedUnit(currentlyFocusedUnit);
        }
    }
        


    


    private int SortUnitsByInitiativeFunc(Unit unitA, Unit unitB)
    {
        int initA = unitA.GetInitiative();
        int initB = unitB.GetInitiative();
        if (initA>initB)
        {
            return 1;
        }
        else if (initA < initB)
        {
            return -1;
        }
        else
        {
            return 0;
        }
        
    }



}
