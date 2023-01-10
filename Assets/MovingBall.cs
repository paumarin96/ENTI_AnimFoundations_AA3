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
    private bool _shooted;

    private void OnEnable()
    {
        ShootForce.OnShoot += GetShootForce;
        _scorpion.OnStartWalk += ResetShootedFlag;
    }

    private void OnDisable()
    {
        ShootForce.OnShoot -= GetShootForce;
        _scorpion.OnStartWalk -= ResetShootedFlag;

    }

    // Start is called before the first frame update
    void Start()
    {
        _initialTransform = transform;
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
        if (!_shooted)
        {
            _myOctopus.NotifyShoot();
            _shootingPhysics.Shoot(_force);
            _shooted = true;
        }

    }

    private void ResetShootedFlag()
    {
        _shooted = false;
    }
}
