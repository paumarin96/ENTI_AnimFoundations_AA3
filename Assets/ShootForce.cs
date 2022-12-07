using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class ShootForce : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public float speed = 3;
    private float _counter;
    
    private int _direction  = 1;
    private bool _startSlider = false;

    public  delegate void Shoot(float force);

    public static event Shoot OnShoot;

    public void StartForceSlider()
    {
        _startSlider = true;
    }
    
    public void StopForceSlider()
    {
        _startSlider = false;
        if (OnShoot != null)
            OnShoot(slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_startSlider)
            return;
        if (_counter >= 100 && _direction == 1)
        {
            _direction = -1;
        } else if (_counter <= 0 && _direction == -1)
        {
            _direction = 1;
        }
        _counter += Time.deltaTime * _direction * speed;

        slider.value = Mathf.Clamp(_counter, 0, 100);
    }
}
