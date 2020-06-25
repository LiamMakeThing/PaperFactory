using System.Collections;
using System.Collections.Generic;
using UnityEngine;






public class ElevationController : MonoBehaviour
{
    /*mouse scroll to chnage floors
     * If floor changes, update meshes.
     */
    [SerializeField] int curElevationLevel;
    [SerializeField] Vector2Int elevationLevels = new Vector2Int();
    CameraController ccCam;
    float elevationStep = 6.0f;
    //PresetElevations. TODO-Make this repeatable, dynamically scalable and abstracted out from here.
    //TODO: Get this handled and stored before the game runs with the editor with a separate class on the elevation manager. Should find and sort before game runs and provide feedback to level design when placing.

    [SerializeField] List<FacingVisGroup> lvl0VisGroup = new List<FacingVisGroup>();
    [SerializeField] bool lvl0VisState;

    [SerializeField] List<FacingVisGroup> lvl1VisGroup = new List<FacingVisGroup>();
    [SerializeField] bool lvl1VisState;

    [SerializeField] List<FacingVisGroup> lvl2VisGroup = new List<FacingVisGroup>();
    [SerializeField] bool lvl2VisState;






    // Start is called before the first frame update
    private void Awake()
    {
        ccCam = GameObject.FindObjectOfType<CameraController>();
     
        
    }

    void Start()
    {
        CollectFacings();
        SetElevationLevel(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0.0f)
        {
            SetElevationLevel(curElevationLevel-1);
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0.0f)
        {
            SetElevationLevel(curElevationLevel+1);
        }
    }

    public int GetLevelFromYPos(float yPos)
    {
        int level = Mathf.FloorToInt(yPos / elevationStep);
        return level;
    }
    public void SetElevationLevel(int level)
    {
        if (level != curElevationLevel)
        {
            //when switching to new unit, should update elevation to units level.
            if (level > elevationLevels.x - 1 && level < elevationLevels.y + 1)
            {

                curElevationLevel = level;
                ccCam.UpdateElevationLevel(curElevationLevel, elevationStep);
                UpdateMeshVisByElevationLevel(curElevationLevel);
            }
        }
        
        
    }
    public int GetElevationLevel()
    {
        return curElevationLevel;
    }
    //Make a new data structuyre and have a list of them.
    //bool is the sate of visibility so we can check its state before forcing a list loop
    //list of meshes

    void CollectFacings()//Collects all meshes and sorts them into elevation lists based on what their y pos is. The number of lists is used to get how many elevations we have and set them.
    {

        /*get aall meshes
         * get its height. 
         * do we have a list for this elevation yet?
         * No? Make one
         * Yes? Add it
         * Update min and max elevation levels
         */
        //Collect everything

        //Collect all facings. For each one, sort them into floors based on their floor method.
        var allFacings = GameObject.FindObjectsOfType<FacingVisGroup>();
        foreach (FacingVisGroup visGroup in allFacings)
        {
            int floorValue = visGroup.GetFloorValue();
            switch (floorValue)
            {
                case 0:
                    lvl0VisGroup.Add(visGroup);
                    break;
                case 1:
                    lvl1VisGroup.Add(visGroup);
                    break;
                case 2:
                    lvl2VisGroup.Add(visGroup);
                    break;
                default:
                    lvl0VisGroup.Add(visGroup);
                    break;
            }
        }
        
    }


  /*
   * New data structure. Floor. Floor has a string name to match with tags, a bool to check state of vis and a list of levelkitbase.
   * There is a list of floors, the floors
     * 3. Overpopulated lists, some of which may not be used.
     * Scroll min max is set manually
     * New elevation level. CHeck the status of all lower levels. If they are off, turn them on. Check all the ones above, if they are on, turn them off
     */ 
    void UpdateMeshVisByElevationLevel(int level)
    {
        
        switch (level)
        {
            case 0:
                if (!lvl0VisState)//if lvl0 is off, turn it on
                {
                    ToggleElevationVis(0, true);
                    lvl0VisState = true;
                }
                if (lvl1VisState)//if lvl1 is on, turn it off
                {
                    ToggleElevationVis(1, false);
                    lvl1VisState = false;
                }
                if(lvl2VisState)//if lvl2 is on, turn if off
                {
                    ToggleElevationVis(2, false);
                    lvl2VisState = false;
                }
                break;
            case 1:
                if (!lvl0VisState)//if lvl0 is off, turn it on
                {
                    ToggleElevationVis(0, true);
                    lvl0VisState = true;
                }
                if (!lvl1VisState)//if lvl1 is off, turn it on
                {
                    ToggleElevationVis(1, true);
                    lvl1VisState = true;
                }
                if (lvl2VisState)//if lvl2 is on, turn if off
                {
                    ToggleElevationVis(2, false);
                    lvl2VisState = false;
                }
                break;
            case 2:
                if (!lvl0VisState)//if lvl0 is off, turn it on
                {
                    ToggleElevationVis(0, true);
                    lvl0VisState = true;
                }
                if (!lvl1VisState)//if lvl1 is off, turn it on
                {
                    ToggleElevationVis(1, true);
                    lvl1VisState = true;
                }
                if (!lvl2VisState)//if lvl2 is off, turn if on
                {
                    ToggleElevationVis(2, true);
                    lvl2VisState = true;
                }
                break;
            default:
                break;
        }
    }
    void ToggleElevationVis(int level, bool state)
    {
        List<FacingVisGroup> listToUse = new List<FacingVisGroup>();
       
        switch (level)
        {
            case 0:
                listToUse = lvl0VisGroup;
                break;
            case 1:
                listToUse = lvl1VisGroup;
                break;
            case 2:
                listToUse = lvl2VisGroup;
                break;
        }
        foreach(FacingVisGroup visGroup in listToUse)
        {
            visGroup.ToggleVis(true);
        }
    }
}
