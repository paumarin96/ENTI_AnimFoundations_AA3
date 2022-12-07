using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;
    [SerializeField]
    ShootingPhysics _shootingPhysics;

    [SerializeField] 
    private IK_Scorpion _scorpion;
    private float _force;
    
    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;

    Vector3 _dir;
    public Transform target;
    private Transform _initialTransform;

    private void OnEnable()
    {
        ShootForce.OnShoot += GetShootForce;
        _scorpion.OnStartWalk += ResetBall;
    }

    private void OnDisable()
    {
        ShootForce.OnShoot -= GetShootForce;
        _scorpion.OnStartWalk -= ResetBall;
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialTransform = transform;
    }

    void ResetBall()
    {
        Debug.Log("Cum");
        transform.position = _initialTransform.position;
        transform.rotation = _initialTransform.rotation;
        transform.localScale = _initialTransform.localScale;
    }
    void GetShootForce(float force)
    {
        _force = force;
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        //update the direction
        _dir = (target.position - transform.position).normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
        _shootingPhysics.Shoot(_force);
    }
}
