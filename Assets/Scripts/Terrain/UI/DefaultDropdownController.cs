using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DefaultDropdownController : UIController
{
    private Dropdown dropdown;

    [SerializeField]
    private int currentValue;

    public UnityEvent invokeOnValueChanged;

    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();

        dropdown.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public override object GetData()
    {
        return currentValue;
    }

    public override void SetData(object o)
    {
        int value = (int)o;

        currentValue = value;
        dropdown.value = currentValue;
    }

    private void OnValueChanged()
    {
        if (currentValue != dropdown.value)
        {
            currentValue = dropdown.value;

            invokeOnValueChanged.Invoke();
        }
    }
}
