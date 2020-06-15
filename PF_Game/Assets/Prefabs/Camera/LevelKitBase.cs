using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelKitBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject topSlice;

    [SerializeField] GameObject bottomSlice;
    [SerializeField] bool hasSlices;
    GameObject visHandle;
    [SerializeField] float toggleVisSpeed = 10.0f;

    private void Awake()
    {

        if (hasSlices)
        {
            topSlice = transform.Find("Top").gameObject;

            bottomSlice = transform.Find("Bottom").gameObject;
        }
    }
    
    public void  ToggleVis(bool state, bool partial)
    {
        //Fire off a coroutine. state for direction of fade, partial for if its the whole thing or just the top. To be used by the elevation manager.
        if (hasSlices&&partial)
        {
            visHandle = topSlice;
            //topSlice.SetActive(state);
            
        }
        else
        {
            visHandle = gameObject;
            //gameObject.SetActive(state);
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
                targetScale = new Vector3(0.0f, 0.0f, 0.0f);
            }
            while (curScale != targetScale)
            {
                //inTransit = true;
                curScale = visHandle.transform.localScale;

                visHandle.transform.localScale = Vector3.Slerp(visHandle.transform.localScale, targetScale, timeCount);

                timeCount = timeCount + Time.deltaTime * toggleVisSpeed;
                yield return new WaitForSeconds(0.01f);
            }


        

    }
}
