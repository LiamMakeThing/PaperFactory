using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Faction {Player, Ally, Enemy };

public class Unit : MonoBehaviour
{
    
    public Transform stratCamTarget;
    [SerializeField] Faction faction;
    [SerializeField] int initiative;
    [SerializeField] int maxAP;
    [SerializeField] int availableAP;
    [SerializeField] string unitName;

    // Start is called before the first frame update
    private void Awake()
    {
        stratCamTarget = transform.Find("stratCamTarget");
        ResetAP();
    }

    public Faction GetFaction()
    {
        return faction;
    }
    public int GetInitiative()
    {
        return initiative;
    }
    public int GetAvailableAP()
    {
        return availableAP;
    }
    public void ResetAP()
    {
        availableAP = maxAP;
    }
    public string GetName()
    {
        return unitName;
    }

}
