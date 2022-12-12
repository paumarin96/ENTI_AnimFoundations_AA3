using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class MagnusEffectSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private float _speed = 1;
    public float value
    {
        get { return _slider.value; }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            _slider.value -= Time.deltaTime * _speed;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            _slider.value += Time.deltaTime * _speed;
        }

        _slider.value = Mathf.Clamp01(_slider.value);
    }
}
