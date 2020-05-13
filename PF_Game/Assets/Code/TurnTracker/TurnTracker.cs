using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTracker : MonoBehaviour
{
    [Header("Rounds contain a Turn for each unit.")]
    [SerializeField] int curRound;
    [SerializeField] int maxTurnsInRound;
    [SerializeField] int curTurn;
    [Header("Units in level.")]
    UnitBase[] allUnits;
    [SerializeField] List<UnitBase> initiativeOrder = new List<UnitBase>();
    List<UnitBase> hasNotTakenTurn = new List<UnitBase>();
    List<UnitBase> hasTakenTurn = new List<UnitBase>();
    [SerializeField] UnitBase activeUnit;

    CurrentUnitHandler currentUnitHandler;

    private void Awake()
    {
        currentUnitHandler = GameObject.FindObjectOfType<CurrentUnitHandler>();
        print("Turn tracker set up");
    }

    public UnitBase GetActiveUnit()
    {
        return activeUnit;
    }
    void Start()
    {
        
        
        StartNewEncounter();
    }
    void StartNewEncounter()
    {
        //Get All Units
        CollectUnits();
        maxTurnsInRound = allUnits.Length;
        SortUnitsByIntitiative();
        hasNotTakenTurn = initiativeOrder;
        UnitNewRoundRefresh();
        BeginTurn();
        //Get number of units. That is Max Turns.
        //Set current round to 0, current turn to 0,
        //sort units by initiative
    }
    void CollectUnits()
    {
        allUnits = FindObjectsOfType<UnitBase>();
    }

    void SortUnitsByIntitiative()
    {
       //add units to intitiative list
       foreach(UnitBase unit in allUnits)
        {
            initiativeOrder.Add(unit);
        }
        initiativeOrder.Sort(SortFunc);
        initiativeOrder.Reverse();

        foreach (UnitBase unit in initiativeOrder)
        {
            int turnOrder = initiativeOrder.IndexOf(unit);
            unit.UpdateTurnLabelValue(turnOrder);
        }
    }
    //vanilla list.sort() needs to know what to sort. The function below is how to sort based on initiative score.
    private int SortFunc(UnitBase a, UnitBase b)
    {
        int aInit = a.GetInitiativeScore();
        int bInit = b.GetInitiativeScore();
        if (aInit<bInit)
        {
            return -1;
        }
        else if (aInit > bInit)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    void BeginTurn()
    {
        //Set active unit from next unit in the list of units to complete their turn. Always 0, as the previous index is removed, moving the list up.
        activeUnit = hasNotTakenTurn[0];

        //currentUnitHandler.SetCurrentUnit(activeUnit);

    }
    void AdvanceTurn()
    {
        if (curTurn + 1 >= maxTurnsInRound)
        {
            curTurn = 0;
            AdvanceRound();
        }
        else
        {
            curTurn++;
        }
        BeginTurn();
    }
    void AdvanceRound()
    {
        print("NewRound");
        hasTakenTurn.Clear();
        //look for dead guys, collect units, sort by initiative etc set lists
        CollectUnits();
        maxTurnsInRound = allUnits.Length;
        SortUnitsByIntitiative();
        hasNotTakenTurn = initiativeOrder;
        UnitNewRoundRefresh();
        
        BeginTurn();
        //Message New Round.
        //Fire off any new game events that occur over time.
    }
    public void EndTurn()
    {
        //remove active unit from hasNotTakenTurn
        hasTakenTurn.Add(activeUnit);
        hasNotTakenTurn.Remove(activeUnit);
        //Add them to hasTakenTurn
        //Attempt to Advance Turn
        AdvanceTurn();
    }
    void UnitNewRoundRefresh()
    {
        foreach(UnitBase unit in allUnits){
            unit.BeginNewRound();
        }
    }

}
