using System.Collections;
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
    [SerializeField] Unit currentlyFocusedUnit;
    [SerializeField] Unit currentlyActiveTurnUnit;

    CameraController ccCam;
    GameStateHandler gameStateHandler;
    ElevationController elevationController;

    private void Awake()
    {
        ccCam = GameObject.FindObjectOfType<CameraController>();
        gameStateHandler = GameObject.FindObjectOfType<GameStateHandler>();
        elevationController = GameObject.FindObjectOfType<ElevationController>();
    }


    public Unit GetCurrentlyFocusedUnit()
    {
        if (currentlyFocusedUnit != null)
        {
            return currentlyFocusedUnit;
        }
        else
        {
            //Debug.LogError("Current unit in CurrentUnitHandler not set. Returning null and will probably cause some bad shit in the component that requested it.");
            return null;
        }
    }
    public Unit GetCurrentlyActiveTurnUnit()
    {
        if (currentlyActiveTurnUnit != null)
        {
            return currentlyActiveTurnUnit;
        }
        else
        {
            return null;
        }
    }
    public void SetCurrentlyActiveTurnUnit(Unit curTurnUnit)
    {
        if (currentlyActiveTurnUnit != curTurnUnit)
        {
            currentlyActiveTurnUnit = curTurnUnit;
        }
    }
    public void SetCurrentlyFocusedUnit(Unit unit)
    {
        if (currentlyFocusedUnit != unit)
        {
            currentlyFocusedUnit = unit;
            
            
            ccCam.CenterCamOnUnit(currentlyFocusedUnit);

            //update elevation controller
            int curUnitFloor = elevationController.GetLevelFromYPos(currentlyFocusedUnit.transform.position.y);
            int curElevationLevel = elevationController.GetElevationLevel();
            if (curElevationLevel != curUnitFloor)
            {

                elevationController.SetElevationLevel(curUnitFloor);
            }
            
            


        }
    }




}
