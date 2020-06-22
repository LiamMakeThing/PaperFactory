using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Staggro
{
    neutral,
    stealthy,
    aggro
}
public class SteathAggroController : MonoBehaviour
{
    [SerializeField] Staggro staggroState;
    Transform playerMesh;
    // Start is called before the first frame update
    void Start()
    {
        playerMesh = transform.Find("PlayerMesh");
        staggroState = Staggro.neutral;
        UpdateStaggro();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateStaggro(); 
        }
    }
    void UpdateStaggro()
    {
        switch (staggroState)
        {
            case Staggro.stealthy:
                staggroState = Staggro.neutral;
                playerMesh.localScale = new Vector3(1.0f,1.0f,1.0f);
                break;
            case Staggro.neutral:
                staggroState = Staggro.aggro;
                playerMesh.localScale = new Vector3(1.0f, 2.0f, 1.0f);
                break;
            case Staggro.aggro:
                staggroState = Staggro.stealthy;
                playerMesh.localScale = new Vector3(1.0f, 0.5f, 1.0f);
                break;
        }
    }
}
