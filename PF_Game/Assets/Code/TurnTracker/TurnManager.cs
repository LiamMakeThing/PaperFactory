using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    [SerializeField] Faction turnFaction;
    [SerializeField] Unit[] allUnits;
    

    [SerializeField] List<Unit> unitsByInit = new List<Unit>();
    [SerializeField] List<Unit> alliedUnitsByInit = new List<Unit>();
    int curUnitFocusIndex; //the index in unitsByInit of the currentlly focussed unit
    int currentTurnIndex;
    int currentRoundIndex;

    UI_UnitCarousel unitCarousel;
    CurrentUnitHandler unitHandler;

    //ActiveTurnUnit
    [SerializeField] Unit currentlyFocusedUnit;
    //This are different. You can focus on any unit, assuming it is the players turn. The only unit who can take action however is the activeturnunit.
    

    private void Awake()
    {
        unitHandler = GetComponent<CurrentUnitHandler>();
        unitCarousel = GameObject.FindObjectOfType<UI_UnitCarousel>();
        CollectUnits();
    }


    private void Start()
    {
        UpdateCurrentlyFocusedUnit(alliedUnitsByInit[0]);
    }

    void CollectUnits()
    {
        //Grab all units. Stash them in lists based on faction.
        //need to ensure there are no entries of the same value. S
        allUnits = GameObject.FindObjectsOfType<Unit>();
        unitsByInit = new List<Unit>(allUnits);
        unitsByInit.Sort(SortUnitsByInitiativeFunc);

        for (int i = 0; i<unitsByInit.Count;i++)
        {
            unitsByInit[i].SetOrderNum(i);
        }

        //grab all allied Units
        foreach(Unit unit in unitsByInit)
        {
            if (unit.GetFaction() == Faction.Player)
            {
                alliedUnitsByInit.Add(unit);
            }
        }


        

        
    }
    private void Update()
    {
        //INPUT Hit enter and turn ends. 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UserInputAdvanceTurn();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleAlliedTunitFocus();
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
    void CycleAlliedTunitFocus()
    {
        //When called, if the current unit is already an ally, find the next ally. Otherwise, if the current focus unit is an enemy, just get the first ally.
        int numAllies = alliedUnitsByInit.Count;
        int curAllyIndex;
        //search the unitsbyinitive for the next allied unit
        if (currentlyFocusedUnit.GetFaction() == Faction.Player)
        {
            curAllyIndex = alliedUnitsByInit.IndexOf(currentlyFocusedUnit);

            if (curAllyIndex == numAllies-1)
            {
                curAllyIndex = 0;
            }else
            {
                curAllyIndex = curAllyIndex + 1;
            }
        }
        else
        {
            curAllyIndex = 0;
        }
        Unit nextAlly = alliedUnitsByInit[curAllyIndex];
        UpdateCurrentlyFocusedUnit(nextAlly);
    }

   public void UpdateCurrentlyFocusedUnit(Unit newFocusUnit)
    {
        if (currentlyFocusedUnit != newFocusUnit||currentlyFocusedUnit==null)
        {
            currentlyFocusedUnit = newFocusUnit;
            unitHandler.SetCurrentlyFocusedUnit(currentlyFocusedUnit);
            curUnitFocusIndex = unitsByInit.IndexOf(currentlyFocusedUnit);
            unitCarousel.UpdateFocus(curUnitFocusIndex);

        }
    }
        
    public int GetNumActiveUnits()
    {
        return unitsByInit.Count;
    }
    public Unit GetUnitInitAtIndex(int index)
    {
        return unitsByInit[index];
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
