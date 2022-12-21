using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBodyTargetAnimation : MonoBehaviour
{
    public float amplitude = 2.0f;

    public float frequency = 1.0f;

    private float _initialPosY;
    private Vector3 initialPosition;
    private Vector3 initialLocalPosition;

    public ShootingPhysics shootingPhysics;
    private Transform parent;
    public ShootForce shootForce;
    private float shootForceRemaped;
    private Vector3 _shootPoint;

    public AnimationCurve animationCurve;

    private float timer;

    private bool animationRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        _initialPosY = transform.localPosition.y;

    }

    public void StartAnimation()
    {
        initialPosition = transform.position;
        initialLocalPosition = transform.localPosition;
        parent = transform.parent;
        transform.SetParent(null);
        _shootPoint = shootingPhysics.transform.position + shootingPhysics.CalculateShootPoint();
        shootForceRemaped = shootForce.slider.value.Remap(0, 100, 0.3f, 1);
        animationRunning = true;
    }
    // Update is called once per frame
    
    //Lerp from unity modified to work with negative values
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
        
    }
    void Update()
    {
        var maxTime = 0.5f * shootForceRemaped;
        if (animationRunning)
        {
            timer += Time.deltaTime * maxTime;
            
            var curveTime = animationCurve.Evaluate(timer);
 
            transform.position = Lerp(initialPosition, _shootPoint, curveTime);
            transform.position = new Vector3(transform.position.x, _shootPoint.y, transform.position.z);
            
            if(curveTime >= 0.9f)
                shootingPhysics.Shoot(shootForce.slider.value);
            
            if (timer >= 1.0f)
            {
                timer = 0.0f;
                animationRunning = false;
                transform.position = Vector3.zero;
                transform.SetParent(parent);
                transform.localPosition = initialLocalPosition;
                return;
            }
        }
    

        if (animationRunning)
            return;
            
        
        transform.localPosition = 
                new Vector3(transform.localPosition.x, _initialPosY + Mathf.Sin(Time.time * frequency) * amplitude, transform.localPosition.z);
    }
}
