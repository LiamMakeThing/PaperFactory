using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if(UNITY_EDITOR)
[ExecuteInEditMode]
public class UnitIdentityEDITORONLY : MonoBehaviour
{/// <summary>
 /// pulls unit information and updates in editor.
 /// </summary>
 /// //
    // Start is called before the first frame update
    UnitBase unit;
    Transform unitMesh;
    MeshRenderer meshRenderer;
    Transform unitNameLabel;
    string unitName;
    Faction faction;
    
    private void Awake()
    {
        unit = GetComponent<UnitBase>();
        unitMesh = transform.Find("UnitMesh");
        unitNameLabel = transform.Find("Label_UnitName");
        meshRenderer = unitMesh.GetComponent<MeshRenderer>();
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUnitVisuals();
    }
    void UpdateUnitVisuals()
    {
        //Pull info.
        faction = unit.GetFaction();
        unitName = unit.unitName;
        gameObject.name = unitName;
        unitNameLabel.GetComponent<TextMesh>().text = unitName;
        
        if (faction == Faction.Enemy)
        {
            meshRenderer.sharedMaterial.color = Color.red;
        }
        else if (faction == Faction.Player)
        {
            meshRenderer.sharedMaterial.color = Color.blue;
        }
    }
}
#endif