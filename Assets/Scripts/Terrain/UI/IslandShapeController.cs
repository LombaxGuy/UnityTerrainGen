using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IslandShapeController : UIController
{
    private Dropdown dropDown;

    private int currentIndex;

    private void Awake()
    {
        dropDown = GetComponent<Dropdown>();

        dropDown.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public override object GetData()
    {
        return (FalloffShape)currentIndex;
    }

    public override void SetData(object o)
    {
        int shapeIndex = (int)o;

        currentIndex = shapeIndex;
        dropDown.value = currentIndex;
    }

    private void OnValueChanged()
    {
        if (currentIndex != dropDown.value)
        {
            currentIndex = dropDown.value;

            EventManager.InvokeOnFalloffMapSettingsChanged();
        }
    }
}
