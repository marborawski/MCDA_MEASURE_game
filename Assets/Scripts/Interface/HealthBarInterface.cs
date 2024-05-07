using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Lifebar interface
public class HealthBarInterface : MonoBehaviour
{
    private Slider slider;

// An object that creates a gradient displayed in the lifebar
    public Gradient gradient;

// Object displayed as the background of the lifebar
    public Image fill;

// Change the level of life
// h - new level of life
    public void SetHealth(float h)
    {
        if (slider != null)
        {
            slider.value = h;

            if (fill != null)
            {
                fill.color = gradient.Evaluate(h);
            }
        }
    }

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        if (fill != null)
        {
            fill.color = gradient.Evaluate(1);
        }
    }
}
