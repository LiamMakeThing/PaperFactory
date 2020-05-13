using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInteractionLayer : MonoBehaviour
{
    /// <summary>
    /// TurnManager serves up a unit. This class manages all input for what that unit can do. 
    /// contains turn actions
    /// Move
    /// Act
    /// Skip
    /// end turn
    /// </summary>

    
    TurnTracker turnTracker;
    
    

    // Start is called before the first frame update
    private void Awake()
    {
        turnTracker = GameObject.FindObjectOfType<TurnTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
            if (Input.GetKeyDown(KeyCode.Tab))
            {
            
                EndTurn();
            }
        
    }
    void EndTurn()
    {
        

        turnTracker.EndTurn();
    }

    
}
