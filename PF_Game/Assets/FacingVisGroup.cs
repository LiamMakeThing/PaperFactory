using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingVisGroup : MonoBehaviour
{
    /// <summary>
    /// Facing vis group collects all the "Top Slices" in its children and uses a coroutine to scale them all together instead of the previous method of sending each kitpiece a command to start their own coroutine.
    /// </summary>
    // Start is called before the first frame update
    [SerializeField] List<Transform> childrenKitTopSlices = new List<Transform>();
    

    [SerializeField] bool curFacingVisibility;
    Vector3 targetScale;
    [SerializeField] float lerpSpeed = 1.0f;
    Vector3 scaleA;
    Vector3 scaleB;
    [SerializeField] int floorValue;
    LevelKitBase[] childrenKitAssets = new LevelKitBase[0];


    // Update is called once per frame
    private void Awake()
    {
        curFacingVisibility = true;
    }
    private void Start()
    {
        CollectChildren();
    }

    void CollectChildren()
    {
        /*find all children with level kit base component
         *Find their transform "VisGroupHandle" they should have a method that returns it.
         * Make sure they are not there before adding them
        */
        childrenKitAssets = GetComponentsInChildren<LevelKitBase>();
        childrenKitTopSlices = new List<Transform>();
        foreach (LevelKitBase kitAsset in childrenKitAssets)
        {
            
            childrenKitTopSlices.Add(kitAsset.GetTopSlice());
            
        }

    }
    public int GetFloorValue()
    {
        return floorValue;
    }
    public void ToggleVis(bool hideWhenFinished)
    {

        curFacingVisibility = !curFacingVisibility;
        if (curFacingVisibility)
        {
            targetScale = new Vector3(1.0f,1.0f,1.0f);
        }else if (!curFacingVisibility)
        {
            targetScale = new Vector3(1.0f, 0.0f, 1.0f);
        }
        StartCoroutine("UpdateVis",hideWhenFinished);
  
    }
    IEnumerator UpdateVis(bool hideWhenFinished)
    {
        if (!curFacingVisibility)
        {
            scaleA = new Vector3(1.0f, 1.0f, 1.0f);
            scaleB = new Vector3(1.0f, 0.0f, 1.0f);
            //set the a and b to fade down
            if (hideWhenFinished)
            {
                yield return StartCoroutine("ScaleFacing");
                Debug.Log("Scale down. Hiding meshes");
                ToggleSliceVisibility(false);

            }
            else if (!hideWhenFinished)
            {
                StartCoroutine("ScaleFacing");
            }
        }
        else if (curFacingVisibility)
        {
            //enable meshes
            ToggleSliceVisibility(true);
            scaleA = new Vector3(1.0f, 0.0f, 1.0f);
            scaleB = new Vector3(1.0f, 1.0f, 1.0f);
            StartCoroutine("ScaleFacing");
            //enable the meshes
            //set the a and b to fade up
            //start the coroutine
        }
        

    }
    IEnumerator ScaleFacing()
    {
        //increase from 0 to 1 over time. Use this as a weight to blend the scales for each child object with a lerp
        float lerpAlpha = 0.0f;
        
        
        while (lerpAlpha<1.0f)
        {
            
            lerpAlpha = lerpAlpha + Time.deltaTime * lerpSpeed;
            /*lerpAlpha = lerpAlpha * 10.0f;
            lerpAlpha = Mathf.Ceil(lerpAlpha);
            lerpAlpha = lerpAlpha / 10.0f;
            */
            Debug.Log(lerpAlpha.ToString());
            foreach(Transform T in childrenKitTopSlices)
            {
                T.localScale = Vector3.Lerp(scaleA, scaleB, lerpAlpha);
            }
            yield return null;
        }
      
            //TODO-need to enable/disable visibility entirely when this is called from the elevation controller.
        
    }
    void ToggleSliceVisibility(bool state)
    {
        foreach ( LevelKitBase kitAsset  in childrenKitAssets)
        {
            kitAsset.ToggleKitAssetVisibility(state);
        }
    }
}
