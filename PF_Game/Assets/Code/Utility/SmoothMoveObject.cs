using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveObject : MonoBehaviour
{
    [SerializeField] float travelSpeed = 50.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("Mouse ScrollWheel")>0)
        {
            Debug.Log("Tab pressed");
            Vector3 destination = transform.position + new Vector3(0.0f, 3.0f, 0.0f);
            object[] parameters = new object[1] { destination };
            StopCoroutine("MoveObject");
            StartCoroutine("MoveObject", parameters);
        }
        if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            Debug.Log("Space pressed");
            Vector3 destination = transform.position + new Vector3(0.0f, -3.0f, 0.0f);
            object[] parameters = new object[1] { destination };
            StopCoroutine("MoveObject");
            StartCoroutine("MoveObject", parameters);
        }
        
    }
    IEnumerator MoveObject(object[] parameters)
    {
        Vector3 moveTarget = (Vector3)parameters[0];
        float timeCount = 0.0f;
   
        
        float distanceTolerance = 0.1f;
        float distanceToDestination = Vector3.Distance(transform.position, moveTarget);

        while (distanceToDestination > distanceTolerance)
        {
            
            distanceToDestination = Vector3.Distance(transform.position, moveTarget);
            transform.position = Vector3.Slerp(transform.position, moveTarget, timeCount);

            timeCount = timeCount + Time.deltaTime * travelSpeed;
            yield return new WaitForSeconds(0.01f);
        }

    }
     
}
