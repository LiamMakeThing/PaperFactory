using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelKitBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform topSlice;

    [SerializeField] Transform bottomSlice;
    [SerializeField] bool hasSlices;
    Transform visHandle;
    [SerializeField] float toggleVisSpeed = 10.0f;
    [SerializeField] bool isFloor;

    private void Awake()
    {
        topSlice = transform.Find("Top");
        bottomSlice = transform.Find("Bottom");
     
    }
    
    public void  ToggleVis(bool state, bool partial)
    {
 
        if (state)
        {
            visHandle.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        }
        else if (!state)
        {
            visHandle.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        }
        
        //visHandle.SetActive(state);
        StopCoroutine("Fade");
        StartCoroutine("Fade", state);
    }

    IEnumerator Fade(bool state)
    {
    
            Vector3 targetScale = new Vector3();
            Vector3 curScale = visHandle.transform.localScale;
            float moveSpeed = toggleVisSpeed;
            float timeCount = 0.0f;
            if (state)
            {
                targetScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else if (!state)
            {
                targetScale = new Vector3(1.0f, 0.0f, 1.0f);
            }
            while (curScale != targetScale)
            {
                //inTransit = true;
                curScale = visHandle.transform.localScale;

                visHandle.transform.localScale = Vector3.Slerp(visHandle.transform.localScale, targetScale, timeCount);

                timeCount = timeCount + Time.deltaTime * toggleVisSpeed;
                yield return new WaitForSeconds(0.0075f);
            }

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
