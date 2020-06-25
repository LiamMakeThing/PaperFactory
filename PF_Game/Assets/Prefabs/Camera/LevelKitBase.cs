using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelKitBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform topSlice;
    [SerializeField] Transform bottomSlice;
    
    Transform visHandle;
    
    [SerializeField] bool isFloor;

    private void Awake()
    {
        topSlice = transform.Find("Top");
        bottomSlice = transform.Find("Bottom");
     
    }
    
 
    public Transform GetTopSlice()
    {
        visHandle = topSlice;
        return visHandle;
    }
    public void ToggleKitAssetVisibility(bool state)
    {
        topSlice.gameObject.SetActive(state);
        bottomSlice.gameObject.SetActive(state);
    }
    public bool GetIsFloor()
    {
        return isFloor;
    }
}
