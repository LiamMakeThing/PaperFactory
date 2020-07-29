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


    TurnManager turnManager;
    int numActiveUnits;
    [SerializeField] UI_UnitTile unitTilePrefab;
    float horizontalOffset;
    [SerializeField] float padding = 5.0f;

    RectTransform carouselPanelRect;
    [SerializeField] Transform unitTileContainer;
    Vector2 unitTileSize = new Vector2(50, 80);
    [SerializeField] List<UI_UnitTile> unitTiles= new List<UI_UnitTile>();
    UI_UnitTile curFocusTile;

    private void Awake()
    {
        //get current unit handler and turn manager
        
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

        numActiveUnits = turnManager.GetNumActiveUnits();

        //resize carousel panel


        /*We need a stronger association between the unitsbyinit and the unit tiles. The tiles and the index of units by init should be the same. Instead of essentially copying and referencing two instances of the same list,
         * we should just reference the one in the turn manager. References to a specific unit are handled by their order/index in unitsbyinit. THis number is also stored within the unit class. "unitOrderNumber"
         * 
         */ 
        for (int i = 0; i < numActiveUnits; i++)
        {
            float edgeCenterOffset = horizontalOffset / 2 + padding; //starting offset to move everything over from edge
            Vector3 pos = new Vector3(transform.position.x - (horizontalOffset + padding) * i - edgeCenterOffset, transform.position.y - (unitTileSize.y / 2 + padding), transform.position.z);
            var newUnitTile = Instantiate(unitTilePrefab, pos, Quaternion.identity, unitTileContainer);
            newUnitTile.SetUnitReference(turnManager.GetUnitInitAtIndex(i));
            unitTiles.Add(newUnitTile);

        }
        
    }

    public void UpdateFocus(int index)
    {
        
        UI_UnitTile tempTile = unitTiles[index];
        if (curFocusTile != tempTile)
        {
            if (curFocusTile != null)
            {
                curFocusTile.SetFocus(false);
                
            }
        
            curFocusTile = tempTile;
            curFocusTile.SetFocus(true);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
