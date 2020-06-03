using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnitLabels : MonoBehaviour
{
    Unit unit;
    private void Start()
    {
        
        
    }
    // Update is called once per frame
    void Update()
    {
        if (unit == null)
        {
            unit = GetComponent<Unit>();
        }
        gameObject.name = unit.GetFaction().ToString() + ": " + unit.GetName();
    }
}
