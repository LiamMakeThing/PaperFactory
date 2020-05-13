using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    movement,pov,strategic
}

public class InputStateHandler : MonoBehaviour
{/// <summary>
/// Take input from user and set states.
/// State: Movement 
/// State: SHOOTER/POV
/// State: Strategic Camera
/// 
/// 
/// FLOW:
/// TURN MANAGER CAN PUSH TO THIS. UNIT SWITCHING CAN UPDATE STATES.
/// OTHER CLASSES REFERENCE THE ENUM STATE AND FILTER THEIR OWN INPUT THROUGH THEIR INTERACTION LAYER
/// </summary>
    public bool userInputEnabled;
    [SerializeField] GameState gameState = new GameState();
    
    //need an enum for game state
        
        // Start is called before the first frame update
    void Start()
    {
        userInputEnabled = true;
        gameState = GameState.movement;
    }

    public void SetGameState(GameState newState)
    {
        gameState = newState;
    }
    public GameState GetGameState()
    {
        return gameState;
    }
  
}
