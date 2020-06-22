using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    CurrentUnitHandler unitHandler;
    Unit currentUnit;
    private void Awake()
    {
        unitHandler = GetComponent<CurrentUnitHandler>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("LeftClick"))
        {
            Debug.Log("Left click");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log("Physics hit");
                Debug.Log(hit.transform.name);
                if (hit.transform.GetComponent<Unit>())
                {
                    Debug.Log("unit hit");
                    Unit tempUnit = hit.transform.GetComponent<Unit>();
                    
                    if (tempUnit.GetFaction() == Faction.Player)
                    {
                        Debug.Log("Player faction hit");
                        if (tempUnit != currentUnit)
                        {

                            currentUnit = tempUnit;
                            unitHandler.SetCurrentUnit(currentUnit);
                        }
                    }
                    
                }
            }

        }
    }
}
