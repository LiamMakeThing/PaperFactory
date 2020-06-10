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
    float elevationStep = 4.0f;
    //PresetElevations. TODO-Make this repeatable, dynamically scalable and abstracted out from here.
    //TODO: Get this handled and stored before the game runs with the editor with a separate class on the elevation manager. Should find and sort before game runs and provide feedback to level design when placing.

    [SerializeField] GameObject[] lvl0Objects;
    [SerializeField] bool lvl0VisState;

    [SerializeField] GameObject[] lvl1Objects;
    [SerializeField] bool lvl1VisState;

    [SerializeField] GameObject[] lvl2Objects;
    [SerializeField] bool lvl2VisState;






    // Start is called before the first frame update
    private void Awake()
    {
        ccCam = GameObject.FindObjectOfType<CameraController>();
     
        CollectMeshes();
    }

    void Start()
    {
        SetElevationLevel(0);
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
                Debug.Log("ElevationUpdate");
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

    void CollectMeshes()//Collects all meshes and sorts them into elevation lists based on what their y pos is. The number of lists is used to get how many elevations we have and set them.
    {

        /*get aall meshes
         * get its height. 
         * do we have a list for this elevation yet?
         * No? Make one
         * Yes? Add it
         * Update min and max elevation levels
         */
        //Collect everything
        lvl0Objects = GameObject.FindGameObjectsWithTag("Level0");
        Debug.Log(lvl0Objects.Length.ToString() + " meshes found in lvl0");
        lvl1Objects = GameObject.FindGameObjectsWithTag("Level1");
        Debug.Log(lvl1Objects.Length.ToString() + " meshes found in lvl1");
    }
    void UpdateMeshVisByElevationLevel(int level)
    {
        if(level == 0)
        {
            if (!lvl0VisState)
            {
                ToggleElevationVis(0, true);
                lvl0VisState = true;
            }
            if (lvl1VisState)
            {
                ToggleElevationVis(1, false);
                lvl1VisState = false;
            }
        }
        else if (level ==1)
        {
            if (!lvl1VisState)
            {
                ToggleElevationVis(1, true);
                lvl1VisState = true;
            }
            if (!lvl0VisState)
            {
                ToggleElevationVis(0, true);
                lvl0VisState = true;
            }
        }
    }
    void ToggleElevationVis(int level, bool state)
    {
        GameObject[] arrayToUse = new GameObject[0];
        if(level == 0)
        {
            arrayToUse = lvl0Objects;
           
        }else if(level == 1)
        {
            arrayToUse = lvl1Objects;
        }
        foreach (GameObject go in arrayToUse)
        {

            MeshRenderer[] meshR = go.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshR)
            {
                mr.enabled = state;
            }
            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            foreach(Collider c in colliders)
            {
                c.enabled = state;
            }
        }
    }
}
