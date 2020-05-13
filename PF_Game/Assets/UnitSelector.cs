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
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.GetComponent<Unit>())
                {
                    Unit tempUnit = hit.transform.GetComponent<Unit>();
                    if (tempUnit!=currentUnit||currentUnit==null)
                    {
                        currentUnit = tempUnit;
                        unitHandler.SetCurrentUnit(currentUnit);
                    }
                }
            }

        }
    }
}
