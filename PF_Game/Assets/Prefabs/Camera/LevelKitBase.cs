using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelKitBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform topSlice;
    [SerializeField] Transform baseSlice;
    [SerializeField] Transform floorSlice;
    Transform visHandle;
    
    [SerializeField] bool isFloor;

    private void Awake()
    {
        
        if (isFloor)
        {
            floorSlice = transform.Find("TileScale");
        }
        else
        {
            topSlice = transform.Find("Top");
            baseSlice = transform.Find("Base");
        }
     
    }
    
 
    public Transform GetTopSlice()
    {
        visHandle = topSlice;
        return visHandle;
    }
    public Transform GetBaseSlice()
    {
        if (isFloor)
        {
            visHandle = floorSlice;
        }
        else
        {
            visHandle = baseSlice;
        }
        
        return visHandle;
    }
    public void ToggleKitAssetVisibility(bool state)
    {
        if (!isFloor)
        {
            topSlice.gameObject.SetActive(state);
            baseSlice.gameObject.SetActive(state);
        }
        else
        {
            floorSlice.gameObject.SetActive(state);
        }

    }
   
}
