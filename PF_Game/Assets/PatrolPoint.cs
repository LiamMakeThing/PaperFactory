using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    PatrolRoute parentRoute;
    private void Awake()
    {
        parentRoute = transform.parent.GetComponent<PatrolRoute>();
    }
    // Start is called before the first frame update
    public PatrolRoute GetParentRoute()
    {

        return parentRoute;
    }
}
