using UnityEngine;


public class ShootingPhysics : MonoBehaviour
{
    public Transform ball;

    public Vector3 direction;
    private bool _shoot;
    
    public void Shoot(Vector3 Direction, float force)
    {
        _shoot = true;
        
        
    }
    void Update()
    {
      if(!_shoot)
          return;
      
      //ball.position = new Vector3(,) //mrua

}
