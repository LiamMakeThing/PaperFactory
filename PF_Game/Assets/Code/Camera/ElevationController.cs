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
    // Start is called before the first frame update
    private void Awake()
    {
        ccCam = GameObject.FindObjectOfType<CameraController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            SetElevationLevel(curElevationLevel-1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            SetElevationLevel(curElevationLevel+1);
        }
    }

    public void SetElevationLevel(int level)
    {

        //when switching to new unit, should update elevation to units level.
        if(level>elevationLevels.x-1 && level < elevationLevels.y+1)
        {
            
            curElevationLevel = level;
            ccCam.UpdateElevationLevel(curElevationLevel, elevationStep);
            Debug.Log("ElevationUpdate");
        }
        
    }
    public int GetElevationLevel()
    {
        return curElevationLevel;
    }
}
