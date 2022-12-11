using System;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
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
    private Vector3 _startPosition;
    
    private bool _shoot;
    private Vector3 finalPos;
    private Vector3 initialPos;
    private Vector3 initialVel;

    private Vector3 eulerOldPos;
    private Vector3 eulerOldVel;
    private Vector3 eulerAccel;
    private Vector3 eulerForces;

    private Vector3 w;
    [SerializeField] private float mass = 1;


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
        _startPosition = transform.position;
    }

    void ResetTimer()
    {       
        
        _shoot = false;
        timer = 0.0f;
        transform.position = _startPosition;
     
        initialPos = transform.position;
    }
    public void Shoot(float force)
    {
        totalAnimationTime = force.Remap(0, 100, maxTime, minTime);
        
        _shoot = true;
        initialPos = _startPosition;

        //find initial velocity
        initialVel.x = (target.position.x - initialPos.x) / totalAnimationTime;
        initialVel.y = ((target.position.y - initialPos.y) - (0.5f * acceleration * totalAnimationTime * totalAnimationTime)) / totalAnimationTime ;
        initialVel.z = (target.position.z - initialPos.z) / totalAnimationTime;
        

        
        var torque = Vector3.Cross(ball.position + ball.right * -0.5f, 1 * (target.position - initialPos).normalized);
        w = Time.deltaTime * (torque / (2.0f / 5.0f * mass * (0.5f * 0.5f))); 
        Debug.Log(w);
       
        

        initialVel = target.position - initialPos;
        eulerOldVel = Vector3.forward;//initialVel * 2;
        
        Debug.Log(w);

    }

    void Update()
    {
        if (!_shoot)
            return;

        timer += Time.deltaTime;

        if (timer >= totalAnimationTime)
        {
            //ResetTimer();
        }
            

        // finalPos.x = initialPos.x + initialVel.x * timer;
        // finalPos.y = initialPos.y + initialVel.y * timer + (acceleration * timer * timer)/2;
        // finalPos.z = initialPos.z + initialVel.z * timer;

        Vector3 magnusForce = 2.0f * (Vector3.Cross(w, eulerOldVel));
        //Debug.Log(magnusForce);
        //Vector3 targetForce = (target.position - initialPos) * 10;
        eulerForces = magnusForce;// + Vector3.down * (2 * 9.81f);
        eulerAccel = eulerForces / 1.0f;

        Vector3 eulerVel = eulerOldVel + eulerAccel * Time.deltaTime;
        finalPos = ball.position + eulerVel * Time.deltaTime;

        eulerOldVel = eulerVel;
        

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
