using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VisGroupTransitionType { Partial, Full};
public class VisGroup : MonoBehaviour
{
    [SerializeField] int floorValue;
    [SerializeField] bool hasSlices;
    [SerializeField] bool curVisState;
    [SerializeField] bool omitFromOccluder;
    LevelKitBase[] childrenKitAssets = new LevelKitBase[0];
    List<Transform> topSlices = new List<Transform>();
    List<Transform> baseSlices = new List<Transform>();
    VisGroupTransitionType transition;
    float lerpSpeed = 15.0f;
    Vector3 scaleA;
    Vector3 scaleB;


    /// <summary>
    /// VisGroupTransitionType (Partial vs Full) comes from outside source. Whether to leave the bottom visible or not after the transition. 
    /// Important for elevation control where it should all be hidden vs the occluder where the base should remain visible.
    /// hasSlices is specific to the vis group. Whether or not the kit assets contained within hae top slices to transition or not. Railings for example, do not. They are just a base.
    /// 
    /// Order comes into toggle vis.
    /// Depending on the transition type and if the visgroup has slices or not the couroutine to scale down/up and hide/show kit assets are called. 
    /// 
    /// 
    /// 
    /// </summary>

    private void Awake()
    {
        //Set default vis to on.
        curVisState = true;
    }
    private void Start()
    {
        CollectChildrenKitAssets();
    }
    void CollectChildrenKitAssets()
    {
        childrenKitAssets = GetComponentsInChildren<LevelKitBase>();
        if (hasSlices)
        {
            //find base, and top (if applicable)
            foreach (LevelKitBase kitAsset in childrenKitAssets)
            {
                if (hasSlices)
                {
                    topSlices.Add(kitAsset.GetTopSlice());
                }
                baseSlices.Add(kitAsset.GetBaseSlice());
            }
        }
    }
    private void Update()
    {
        //Temp input.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ToggleVis(VisGroupTransitionType.Full);
        }
    }
    public void SetVis(bool state,VisGroupTransitionType transitionType)
    {
        curVisState = state;//toggle the end state

        if (curVisState)//depending on the end state, set the starting end ending scale values to lerp between
        {
            scaleA = new Vector3(1.0f, 0.0f, 1.0f);
            scaleB = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else if (!curVisState)
        {
            scaleA = new Vector3(1.0f, 1.0f, 1.0f);
            scaleB = new Vector3(1.0f, 0.0f, 1.0f);
        }

        transition = transitionType;//store the transition

        StartCoroutine("FadeSequence");//Start the transition sequence
    }
    IEnumerator FadeSequence()
    {
        ToggleKitAssets(true);//enable all kit assets. Regardless of the end state, they should be on when we start. 


        if(transition == VisGroupTransitionType.Full)
        {
            if (hasSlices)
            {
                yield return StartCoroutine("ScaleSlice");
            }
            ToggleKitAssets(curVisState);
            yield return null;
        }else if(transition == VisGroupTransitionType.Partial)
        {
            if (hasSlices)
            {
                
                yield return StartCoroutine("ScaleSlice");

            }
            yield return null;
        }
    }

    IEnumerator ScaleSlice()
    {
        //increase from 0 to 1 over time. Use this as a weight to blend the scales for each child object with a lerp
        float lerpAlpha = 0.0f;


        while (lerpAlpha < 1.0f)
        {

            lerpAlpha = lerpAlpha + Time.deltaTime * lerpSpeed;
            /*lerpAlpha = lerpAlpha * 10.0f;
            lerpAlpha = Mathf.Ceil(lerpAlpha);
            lerpAlpha = lerpAlpha / 10.0f;
            */
            
            foreach (Transform T in topSlices)
            {
                T.localScale = Vector3.Lerp(scaleA, scaleB, lerpAlpha);
            }
            yield return null;
        }
    }
    void ToggleKitAssets(bool state)
    {
        foreach (LevelKitBase kitAsset in childrenKitAssets)
        {
            kitAsset.ToggleKitAssetVisibility(state);
        }
    }
    public int GetFloorValue()
    {
        return floorValue;
    }
    public bool GetOmitState()
    {
        return omitFromOccluder;
    }
}
