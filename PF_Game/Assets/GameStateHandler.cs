using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    movement,pov
}

public class GameStateHandler : MonoBehaviour
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
    [SerializeField] GameState currentGameState = new GameState();
    [SerializeField] GameState startingGameState = new GameState();

    MultiModeCamera mmCam;

    //need an enum for game state

    // Start is called before the first frame update
    private void Awake()
    {
        SetGameState(startingGameState);
        mmCam = GameObject.FindObjectOfType<MultiModeCamera>();
    }
    void Start()
    {
        userInputEnabled = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Toggle game mode
            switch (currentGameState)
            {
                case GameState.movement:
                    SetGameState(GameState.pov);
                    break;
                case GameState.pov:
                    SetGameState(GameState.movement);
                    break;
                default:
                    break;
            }

            mmCam.UpdateGameState(currentGameState);
        }
    }

    public void SetGameState(GameState newState)
    {
        currentGameState = newState;
    }
    public GameState GetGameState()
    {
        return currentGameState;
    }
  
}
