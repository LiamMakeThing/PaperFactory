using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{

    List<Node> cachedPath = new List<Node>();
    
    private void Awake()
    {
        
    }
    public void UpdatePath(List<Node> path)
    {
        if (cachedPath.Count > 0)
        {
            foreach (Node n in cachedPath)
            {
                n.SetShowPathIndicator(false);
            }
        }
        
        cachedPath = path;
        for(int i = 0; i < cachedPath.Count-1; i++)
        {
            cachedPath[i].SetShowPathIndicator(true);
        }
        

        
//Move some blobs on a path



    }

}
