using System;
using System.Collections;
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
    Vector3 upForce;
    [SerializeField] private MagnusEffectSlider _magnusSlider;
    
    private Vector3 w;
    [SerializeField] private float mass = 1;


    public void OnEnable()
    {
        Scorpion.OnStartWalk += ResetBall;
    }
    public void OnDisable()
    {
        Scorpion.OnStartWalk -= ResetBall;
    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    void ResetBall()
    {       
        
        _shoot = false;
        timer = 0.0f;
        transform.position = _startPosition;
     
        initialPos = transform.position;
    }
    public void Shoot(float force)
    {
        force = force.Remap(0, 100, 0, 10);
        
        _shoot = true;
        initialPos = _startPosition;

        var magnusForceMultiplier = _magnusSlider.value;
        
        var torque = Vector3.Cross(ball.right * -0.5f, magnusForceMultiplier * (target.position - initialPos).normalized);
        w = torque;

        upForce = Vector3.up * Mathf.Lerp(0.0f, 15.0f, magnusForceMultiplier);

        initialVel = target.position - initialPos;
        eulerOldVel = initialVel * 0.5f * force;

        StartCoroutine(ResetTimer());

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
        
        Debug.Log(magnusForce);
        //Vector3 targetForce = (target.position - initialPos) * 10;
        eulerForces = magnusForce + upForce + Vector3.down * (0.5f * 9.81f);
        eulerAccel = eulerForces / 1.0f;

        Vector3 eulerVel = eulerOldVel + eulerAccel * Time.deltaTime;
        finalPos = ball.position + eulerVel * Time.deltaTime;

        eulerOldVel = eulerVel;
        

        ball.position = finalPos; 

    }

    private IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(3.0f);
        ResetBall();

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
