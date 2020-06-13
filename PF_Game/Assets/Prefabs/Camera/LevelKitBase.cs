using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelKitBase : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject topSlice;

    [SerializeField] GameObject bottomSlice;

    private void Awake()
    {
        topSlice = transform.Find("Top").gameObject;
        bottomSlice = transform.Find("Bottom").gameObject;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleVis(false, true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ToggleVis(true, true);
        }
    }
    public void  ToggleVis(bool state, bool partial)
    {
        //Fire off a coroutine. state for direction of fade, partial for if its the whole thing or just the top. To be used by the elevation manager.
        topSlice.SetActive(state);
    }
}
