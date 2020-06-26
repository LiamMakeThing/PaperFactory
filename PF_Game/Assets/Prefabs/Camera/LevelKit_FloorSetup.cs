using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelKit_FloorSetup : MonoBehaviour
{
    [SerializeField] Vector2Int floorTileSize = new Vector2Int(4, 4);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {

            return;
        }
        
        UpdateTileSize();
        
    }
    void UpdateTileSize()
    {
        transform.Find("TileScale").localScale = new Vector3(floorTileSize.x,0.25f,floorTileSize.y);
        

    }
}
