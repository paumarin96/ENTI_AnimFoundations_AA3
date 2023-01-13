using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class ShootingPhysics : MonoBehaviour
{

    [Header("UI")] 
    public Text rotationVelocityText;

    [Header("Debug ")] 
    public Transform initialForceArrow;
    public Transform instantaneousVelocityArrow;
    public Transform[] forcesArrows;
    public GameObject greyParticlesPrefab;
    public List<GameObject> greyParticles;
    public GameObject blueParticlesPrefab;
    public List<GameObject> blueParticles;
    private bool debugActive = false;
    
        
    [Header("Rest ")] 

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
    [SerializeField] private ShootForce _forceSlider ;
    
    
    private Vector3 w;
    [SerializeField] private float mass = 1;

    
    public void OnEnable()
    {
        Scorpion.OnStartWalk += ResetBall;
        for (int i = 0; i < 100; i++)
        {
            greyParticles.Add(Instantiate(greyParticlesPrefab, transform.position, Quaternion.identity));
            blueParticles.Add(Instantiate(blueParticlesPrefab, transform.position, Quaternion.identity));
        }
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
        var direction = (target.position - initialPos).normalized;
        var magnusForceMultiplier = _magnusSlider.value;

        initialForceArrow.transform.forward = direction;
        
        Vector3 point = CalculateShootPoint();
     
        var torque = Vector3.Cross(point * 0.5f, magnusForceMultiplier * direction);
        w = torque;

        int side =  Vector3.Dot(direction, Vector3.right) < 0 ? 1 : -1;
        string rotText = "Rotation velocity: " + (side * w.magnitude).ToString("F2") + " deg/s";
        rotationVelocityText.text = rotText;
        
        upForce = Vector3.up * Mathf.Lerp(0.0f, 15.0f, magnusForceMultiplier);

        initialVel = target.position - initialPos;
        eulerOldVel = initialVel * (0.5f * force);

        StartCoroutine(ResetTimer());

    }

    public Vector3 CalculateShootPoint()
    {
        var direction = (target.position - initialPos).normalized;
        var magnusForceMultiplier = _magnusSlider.value;
        var side = Vector3.Dot(direction, Vector3.right) < 0 ? 1 : -1;
        Vector3 point = new Vector3(side * Mathf.Sin(magnusForceMultiplier *  Mathf.PI/2),
            0, -Mathf.Cos(magnusForceMultiplier * Mathf.PI/2));


        

        return point;
    }

    void Update()
    {

        Vector3 gravity = Vector3.down * (0.5f * 9.81f);
        Vector3 magnusForce = 2.0f * (Vector3.Cross(w, eulerOldVel));
        
        Scorpion.SetTargetNormal(((ball.position + CalculateShootPoint()) - ball.position).normalized);

        if (Input.GetKeyDown(KeyCode.I))
        {
            debugActive = !debugActive;
            for (int i = 0; i < 100; i++)
            {
                greyParticles[i].SetActive(debugActive);
                blueParticles[i].SetActive(debugActive);
            }
            initialForceArrow.gameObject.SetActive(debugActive);
            instantaneousVelocityArrow.gameObject.SetActive(debugActive);
            for (int i = 0; i < forcesArrows.Length; i++)
            {
                forcesArrows[i].gameObject.SetActive(debugActive);
            }

        } 

        if (debugActive)
        {
            ShowDebugTrajectory(gravity);
            ShowDebugTrajectoryWithMagnus(gravity);
        }

        
        
        if (!_shoot)
            return;

        timer += Time.deltaTime;


        eulerForces = magnusForce +  gravity;
        eulerAccel = eulerForces / 1.0f;

        //Gravity force
        forcesArrows[0].transform.forward = gravity;
        //Magnus force
        forcesArrows[1].transform.forward = magnusForce;
  



        Vector3 eulerVel = eulerOldVel + eulerAccel * 0.01f;
        finalPos = ball.position + eulerVel * 0.01f;

        eulerOldVel = eulerVel;

        instantaneousVelocityArrow.transform.forward = eulerVel.normalized;

        ball.position = finalPos; 

    }

    private Vector3 debugEulerOldVel;
    private void ShowDebugTrajectory(Vector3 gravity)
    {
        var force = _forceSlider.slider.value.Remap(0, 100, 0, 10);

        var initialVelocity = target.position - _startPosition;
        debugEulerOldVel = initialVelocity * (0.5f * force);
        greyParticles[0].transform.position = _startPosition;
        for (int i = 1; i < 100; i++)
        {
        
            var debugEulerForces = gravity;
            var debugEulerAccel = debugEulerForces / 1.0f;

            Vector3 eulerDebugVel = debugEulerOldVel + debugEulerAccel * (i * 0.01f);
            var debugPointFinalPos =  greyParticles[i - 1].transform.position + eulerDebugVel * (i * 0.01f);

            debugEulerOldVel = eulerDebugVel;
            
            greyParticles[i].transform.position = debugPointFinalPos;
            


        }
    }    
    private Vector3 debugMagnusOldVel;
    private void ShowDebugTrajectoryWithMagnus(Vector3 gravity)
    {
        var force = _forceSlider.slider.value.Remap(0, 100, 0, 10);

        
        
        
        var initialVelocity = target.position - _startPosition;
        
        
        var direction = (target.position - _startPosition).normalized;
        var magnusForceMultiplier = _magnusSlider.value;

      
        
        Vector3 point = CalculateShootPoint();
     
        var torque = Vector3.Cross(point * 0.5f, magnusForceMultiplier * direction);
        w = torque;
        
        upForce = Vector3.up * Mathf.Lerp(0.0f, 15.0f, magnusForceMultiplier);
       
        debugMagnusOldVel = initialVelocity * (0.5f * force);
        blueParticles[0].transform.position = _startPosition;
        for (int i = 1; i < 100; i++)
        {
            Vector3 magnusForce = 2.0f * (Vector3.Cross(w, debugMagnusOldVel));
            var debugEulerForces = gravity  + magnusForce;
            var debugEulerAccel = debugEulerForces / 1.0f;

            Vector3 eulerDebugVel = debugMagnusOldVel + debugEulerAccel * (i * 0.01f);
            var debugPointFinalPos =  blueParticles[i - 1].transform.position + eulerDebugVel * (i * 0.01f);

            debugMagnusOldVel = eulerDebugVel;
            
            blueParticles[i].transform.position = debugPointFinalPos;
            


        }
    }

    private IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(3.0f);
        ResetBall();

    }
 
}
