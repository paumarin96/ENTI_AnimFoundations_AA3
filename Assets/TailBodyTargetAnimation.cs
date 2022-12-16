using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBodyTargetAnimation : MonoBehaviour
{
    public float amplitude = 2.0f;

    public float frequency = 1.0f;

    private float _initialPos;

    public ShootingPhysics shootingPhysics;
    // Start is called before the first frame update
    void Start()
    {
        _initialPos = transform.localPosition.y;
    }

    void StartAnimation()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.localPosition =
            new Vector3(transform.localPosition.x, _initialPos + Mathf.Sin(Time.time * frequency) * amplitude, transform.localPosition.z);
    }
}
