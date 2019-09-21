using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DefaultSliderController : UIController
{
    private Slider slider;

    private float currentValue;

    public UnityEvent invokeOnValueChanged;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public override object GetData()
    {
        return currentValue;
    }

    public override void SetData(object o)
    {
        float value = (float)o;

        currentValue = value;
        slider.value = currentValue;
    }

    private void OnValueChanged()
    {
        if (currentValue != slider.value)
        {
            currentValue = slider.value;

            invokeOnValueChanged.Invoke();
        }
    }
}
