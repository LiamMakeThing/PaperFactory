using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the input layer to set destinations with cursor.
/// Will expand to allow:
/// -path linking
/// -waypoints
/// -undo
/// -confirmation
/// -Effective AP range
/// </summary>
/// 

public class NavGridInteractionLayer : MonoBehaviour
{
    public bool isMovementMode=false;
    [SerializeField] Node clickedNode;

    Node currentHoverNode;
    
    [SerializeField]PathBuilder pathBuilder;
    Node endNode;
    [SerializeField] Node startNode;
    [SerializeField]UnitBase activeUnit;
    [SerializeField]UnitBase currentUnit;


    [SerializeField] Camera navCam;
 

    [SerializeField] List<Node> tempPath = new List<Node>();

    InputStateHandler inputStateHandler;

    private void Awake()
    {
        navCam = Camera.main;
        pathBuilder = GetComponent<PathBuilder>();
        inputStateHandler = GameObject.FindObjectOfType<InputStateHandler>();
    }
    //SET UNIT FROM TURN TRACKER. UPDATE FUNCTION WILL CHANGE BEHAVIOUR BASED ON THIS VALUE.
    public void UpdateCurrentUnit(UnitBase unit)
    {
        //turn tracker is required. Should include it in prefab.
        activeUnit = unit;
        pathBuilder.MakeNewPath(activeUnit);
        isMovementMode = true;
    }

    
    

    void Update()
    {

        if (inputStateHandler.GetGameState() == GameState.movement)
        {

            //ray cast from camera center instead of mouse position.
            RaycastHit hit;
            if (Physics.Raycast(navCam.transform.position, navCam.transform.forward, out hit, 100.0f)){
                if (hit.transform.GetComponent<Node>())
                {
                    currentHoverNode = hit.transform.GetComponent<Node>();
                    if (endNode != currentHoverNode || endNode == null)//ensure mouseover stuff only occurs once, when the mouseover target changes
                    {
                        
                        endNode = currentHoverNode;
                        pathBuilder.UpdateLivePath(endNode);
                    }
                    if (Input.GetButtonDown("LeftClick"))//set the new target when clicked
                    {
                        pathBuilder.AddPath();
                    }
                }
            }
            if (Input.GetButtonDown("RightClick"))
            {
                pathBuilder.RemovePath();
                pathBuilder.UpdateLivePath(endNode);
            }
            if (Input.GetButtonDown("Jump"))
            {
                print("Telling navagent to move on path");
                pathBuilder.CommitMovement();

                
            }
        }
    }

    //TODO. Commit path function. This moves player and sets iSNewPathStarted to false. Allowing a new path to be made.
  
  

                        
    void PrintPath()
    {

    }
  
}
