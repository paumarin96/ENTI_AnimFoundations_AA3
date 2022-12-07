using System;
using System.Runtime.CompilerServices;
using UnityEngine;


public class ShootingPhysics : MonoBehaviour
{
    public Transform ball;
    public Transform target;
    public IK_Scorpion Scorpion;
    public float minTime = 1;
    public float maxTime = 3.5f;
    public const float acceleration = -9.81f;
    private float totalAnimationTime;
    private float timer = 0.0f;
    private Transform _startTransform;
    
    private bool _shoot;
    private Vector3 finalPos;
    private Vector3 initialPos;
    private Vector3 initialVel;

    public void OnEnable()
    {
        Scorpion.OnStartWalk += ResetTimer;
    }
    public void OnDisable()
    {
        Scorpion.OnStartWalk -= ResetTimer;
    }

    private void Start()
    {
        _startTransform = transform;
    }

    void ResetTimer()
    {       
        Debug.Log("Cum2");
        _shoot = false;
        timer = 0.0f;
        transform.position = _startTransform.position;
     
       
        initialPos = transform.position;
    }
    public void Shoot(float force)
    {
        totalAnimationTime = force.Remap(0, 100, maxTime, minTime);
        
        _shoot = true;
        initialPos = _startTransform.position;

        //find initial velocity
        initialVel.x = (target.position.x - initialPos.x) / totalAnimationTime;
        initialVel.y = ((target.position.y - initialPos.y) - (0.5f * acceleration * totalAnimationTime * totalAnimationTime)) / totalAnimationTime ;
        initialVel.z = (target.position.z - initialPos.z) / totalAnimationTime;

        Debug.Log(initialVel);
    }

    void Update()
    {
        if (!_shoot)
            return;

        timer += Time.deltaTime;

        if (timer >= totalAnimationTime)
            _shoot = false;

        finalPos.x = initialPos.x + initialVel.x * timer;
        finalPos.y = initialPos.y + initialVel.y * timer + (acceleration * timer * timer)/2;
        finalPos.z = initialPos.z + initialVel.z * timer;
        


        ball.position = finalPos; 

    }

    private void OnDrawGizmos()
    {
        float step = 100 / totalAnimationTime;
        for (int i = 0; i < 100; i++)
        {
            
            //Gizmos.DrawSphere();
        }
    }
}
