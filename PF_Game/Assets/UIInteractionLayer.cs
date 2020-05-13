using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractionLayer : MonoBehaviour
{

    // Update is called once per frame
    [SerializeField] Transform menuGameObject;
    bool state;
    private void Awake()
    {
        
    }
    private void Start()
    {
        state = true;
        menuGameObject.gameObject.SetActive(state);
        
    }
    void Update()
    {
        //Hold down LSHIFT to see controls.
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            //if menu is not open, open it.
            ToggleUIVisibility();
        }

    }
    void ToggleUIVisibility()
    {
        state = !state;
        menuGameObject.gameObject.SetActive(state);
    }
}
