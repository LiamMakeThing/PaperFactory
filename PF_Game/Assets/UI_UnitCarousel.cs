using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UnitCarousel : MonoBehaviour
{
    /*DUTIES:
     * Populate units and iniative order from the turn manager. As units come online, either through discovery or events, update the carousel to insert them where needed based on the intiative list in the turn manager.
     * update the position of each unit when turn ticks over
     * when current unit changes in currentunit handler, tell the corresponding unit tile to update its visual treatment.
     */

    CurrentUnitHandler curUnitHandler;
    TurnManager turnManager;
    Unit curUnit;
    [SerializeField] List<Unit> curActiveUnits = new List<Unit>();
    int numActiveUnits;
    [SerializeField] UI_UnitTile unitTilePrefab;
    float horizontalOffset;
    [SerializeField] float padding = 5.0f;
    
    RectTransform carouselPanelRect;
    [SerializeField] Transform unitTileContainer;
    Vector2 unitTileSize = new Vector2(50,80);

    private void Awake()
    {
        //get current unit handler and turn manager
        curUnitHandler = GameObject.FindObjectOfType<CurrentUnitHandler>();
        turnManager = GameObject.FindObjectOfType<TurnManager>();
        horizontalOffset = unitTileSize.x;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeCarousel();   
    }
    void InitializeCarousel()
    {
        print("Pulling List");
        
        PullUnitList();

        //resize carousel panel

        
        
        for (int i = 0; i<numActiveUnits; i++)
        {
            float edgeCenterOffset = horizontalOffset / 2 + padding; //starting offset to move everything over from edge
            Vector3 pos = new Vector3(transform.position.x-(horizontalOffset+padding)*i -edgeCenterOffset, transform.position.y-(unitTileSize.y/2+padding), transform.position.z);
            var newUnitTile = Instantiate(unitTilePrefab, pos, Quaternion.identity, unitTileContainer);
            newUnitTile.SetUnitReference(curActiveUnits[i]);

        }
    }
    void PullUnitList()
    {
        curActiveUnits = turnManager.GetInitiativeSortedUnits();
        numActiveUnits = curActiveUnits.Count;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
