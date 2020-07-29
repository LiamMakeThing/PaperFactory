using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_UnitTile : MonoBehaviour
{
    [SerializeField] Unit unitRef;
    
    [SerializeField] TextMeshProUGUI initiativeLabel;
    TurnManager turnMananger;
    [SerializeField] RectTransform btnRectTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
       initiativeLabel = GetComponentInChildren<TextMeshProUGUI>();
       turnMananger = GameObject.FindObjectOfType<TurnManager>();
       

    }
    public void SetUnitReference(Unit unit)
    {
        unitRef = unit;
        int initValue = unitRef.GetInitiative();
        initiativeLabel.SetText(initValue.ToString());
        //unitRef.GetInitiative();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void UserInputSetUnitFocus()
    {
        turnMananger.UpdateCurrentlyFocusedUnit(unitRef);
    }
    public void SetFocus(bool state)
    {
        if (state)
        {
            btnRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100.0f);
        }else if(state == false)
        {
            btnRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 75.0f);
        }
    }
}
