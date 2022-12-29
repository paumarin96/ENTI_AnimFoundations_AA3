using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IK_Scorpion _scorpion;

    private Rigidbody rb;

    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        _scorpion = GetComponent<IK_Scorpion>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !_scorpion.isPlayerControlling)
            _scorpion.isPlayerControlling = true;
        else
        {
            _scorpion.isPlayerControlling = false;
        }
        Vector2 inputs = new Vector3(Input.GetAxis("Horizontal"), 0,Input.GetAxis("Vertical"));
            rb.velocity = (inputs * (Time.deltaTime * speed));
        
    }
}
