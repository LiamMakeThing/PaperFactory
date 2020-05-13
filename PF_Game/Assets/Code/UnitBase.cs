using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [SerializeField] int maxAP;
    [SerializeField] int availableAP;
    
    public bool isActiveUnit;
    public string unitName;

    [Header("UnitStats")]
    [SerializeField] int baseInitiative;
    [SerializeField] int modifiedInitiative;

    [SerializeField] Faction faction;
    //Zimmer
    //Boppy
    //Kooby
    //Tza
    //Blerp
    //Slappy
    //Bubba
    //GuhGuhGuh
    //HooHeh
    //Hublah!
    Transform labelUnitName;
    [SerializeField]Transform labelTurnOrder;
    NavAgent navAgent;
    Transform labelAP;
    Node currentNode;
    public Transform povPivot;
    public Transform navPopOutPivot;

    // Start is called before the first frame update

    private void Awake()
    {

        labelUnitName = transform.Find("Label_UnitName");

        labelUnitName.GetComponent<TextMesh>().text = unitName;

        labelAP = transform.Find("Label_AP");
        navAgent = GetComponent<NavAgent>();

        povPivot = transform.Find("ViewTarget_PovCam");
        navPopOutPivot = transform.Find("ViewTarget_PopOutPivot");


    }
    void Start()
    {
    }

    // Update is called once per frame
    public Faction GetFaction()
    {
        return faction;
    }
    public Node GetCurrentNode()
    {
       // currentNode = navAgent.GetCurrentNode();
        return currentNode;
    }

    public int GetInitiativeScore(){
        //TODO possible intitative modifier based on status effects before returning intitiative. For now, just return base.
        modifiedInitiative = baseInitiative;
        return modifiedInitiative;
    }
    public int GetAvailableAP()
    {
        return availableAP;
        //TODO this will be the better storage location than nav agent. Path builder can pull from here instead.
    }
    public int GetMaxAP()
    {
        return maxAP;
    }
    public void AdjustAvailableAP(int ap)
    {
        availableAP += ap;
        UpdateAPLabel();
    }
    public void UpdateTurnLabelValue(int turnsRemaining)
    {
        labelTurnOrder.GetComponent<TextMesh>().text = turnsRemaining.ToString();
    }
    public void BeginNewRound()
    {
        availableAP = maxAP;
        UpdateAPLabel();
    }
    void UpdateAPLabel()
    {
        string apLabelText = "[" + availableAP.ToString() + "/" + maxAP.ToString() + "]";
        labelAP.GetComponent<TextMesh>().text = apLabelText;
    }
}
