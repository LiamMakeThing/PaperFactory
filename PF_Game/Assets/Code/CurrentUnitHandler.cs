﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// central location to get and set active and current unit information.
/// Just about everything pulls active unit information so this will contain it, expose it and have hooks to get and set. 
/// It will also handle error reporting due to active unit not being available.
/// It contains no references, other classes reference it.
/// </summary>
public class CurrentUnitHandler : MonoBehaviour
{
    [Header("CurrentUnit")]
    
    


    [Header("Defaults")]
    [SerializeField] Unit currentlySelectedUnit;


    //COMPONENTS THIS SCRIPT POKES. (OUTGOING)
    NavCamera navCamera;
    NavGridInteractionLayer navGridInteraction;

    

    private void Awake()
    {
        navCamera = GameObject.FindObjectOfType<NavCamera>();
        navGridInteraction = GameObject.FindObjectOfType<NavGridInteractionLayer>();
    }



    public Unit GetCurrentUnit()
    {
        if (currentlySelectedUnit != null)
        {
            return currentlySelectedUnit;
        }
        else
        {
            Debug.LogError("Current unit in CurrentUnitHandler not set. Returning null and will probably cause some bad shit in the component that requested it.");
            return null;
        }
    }
    public void SetCurrentUnit(Unit unit)
    {
        if (currentlySelectedUnit != unit)
        {
            currentlySelectedUnit = unit;
        }
    }




}
