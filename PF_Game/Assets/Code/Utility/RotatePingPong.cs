using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePingPong : MonoBehaviour
{
    [SerializeField] float maxRot;
    Vector3 dirA;
    Vector3 dirB;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        dirA = transform.eulerAngles + new Vector3(0.0f, -maxRot, 0.0f);
        dirB = transform.eulerAngles + new Vector3(0.0f, maxRot, 0.0f);

    }

    // Update is called once per frame
    void Update()
    {
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.eulerAngles = Vector3.Lerp(dirA, dirB, time);
    }
}
