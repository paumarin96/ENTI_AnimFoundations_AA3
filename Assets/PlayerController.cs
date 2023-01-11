using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public IK_Scorpion _scorpion;

    public Rigidbody rb;

    public float speed;

    public GameObject gameCamera;
    public GameObject freeCamera;
    public GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !_scorpion.isPlayerControlling)
        {
            _scorpion.isPlayerControlling = true;
            freeCamera.SetActive(true);
            gameCamera.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if(Input.GetKeyDown(KeyCode.P) && _scorpion.isPlayerControlling)
        {
            _scorpion.isPlayerControlling = false;
            freeCamera.SetActive(false);
            gameCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        Vector3 inputs = Vector3.zero;
        if (_scorpion.isPlayerControlling)
        {
          inputs  = new Vector3(Input.GetAxis("Horizontal"), 0,Input.GetAxis("Vertical"));
        }
        


        Vector3 camDirection = -new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);

        Quaternion camForwardRot = Quaternion.AngleAxis(mainCamera.transform.rotation.eulerAngles.y + 180, Vector3.up);
        
       // _scorpion.Body.forward =  camDirection;
       _scorpion.Body.rotation =  _scorpion.GetBodyRotation(camDirection);
        
        rb.velocity = (_scorpion.Body.right * -inputs.x + _scorpion.Body.forward * -inputs.z) * (Time.deltaTime * speed);
            
        
    }
}
