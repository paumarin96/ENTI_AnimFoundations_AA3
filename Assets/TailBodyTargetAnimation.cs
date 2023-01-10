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
    public IK_Scorpion scorpion;
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
        scorpion.SetLearningRate(shootForce.slider.value.Remap(0,100,25, 200));
        initialPosition = transform.position;
        transform.position = shootingPhysics.transform.position + shootingPhysics.CalculateShootPoint();
        animationRunning = true;
        StartCoroutine("ReturnToNormal");
    }

    private IEnumerator ReturnToNormal()
    {
        yield return new WaitForSeconds(2);
        transform.position = initialPosition;
        animationRunning = false;
    }
    

    void Update()
    {
        var maxTime = 0.5f * shootForceRemaped;
  
    

        if (animationRunning)
            return;
            
        
        transform.localPosition = 
                new Vector3(transform.localPosition.x, _initialPosY + Mathf.Sin(Time.time * frequency) * amplitude, transform.localPosition.z);
    }
}
