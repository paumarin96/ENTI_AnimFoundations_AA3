using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;

    Vector3 _dir;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
