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



    // Update is called once per frame
    private void Awake()
    {
        
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
        var children = GetComponentsInChildren<LevelKitBase>();
        foreach(LevelKitBase kitAsset in children)
        {
            childrenKitTopSlices.Add(kitAsset.GetVisHandle());
        }
    }
}
