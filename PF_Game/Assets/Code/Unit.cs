using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Faction {Player, Ally, Enemy };

public class Unit : MonoBehaviour
{
    public Transform povTarget;
    public Transform movementTarget;
    [SerializeField] Faction faction;
    [SerializeField] int initiative;
    
    // Start is called before the first frame update
    public Faction GetFaction()
    {
        return faction;
    }
    public int GetInitiative()
    {
        return initiative;
    }

}
