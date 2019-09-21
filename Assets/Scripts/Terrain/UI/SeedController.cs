using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedController : UIController
{
    private InputField inputField;

    private int currentValue;
    private int minValue = 0;
    private int maxValue = 999999;

    private void Awake()
    {
        inputField = GetComponent<InputField>();

        inputField.onEndEdit.AddListener(delegate { OnEndEdit(); });
    }

    public override object GetData()
    {
        return currentValue;
    }

    public override void SetData(object o)
    {
        int value = (int)o;

        currentValue = EvaluateInputFieldValue(value);
        inputField.text = currentValue.ToString();
    }

    private void OnEndEdit()
    {
        int value = int.MaxValue;

        if (int.TryParse(inputField.text, out value))
        {
            if (value != currentValue)
            {
                currentValue = EvaluateInputFieldValue(value);

                EventManager.InvokeOnHeightMapSettingsChanged();
            }
        }

        inputField.text = EvaluateInputFieldValue(value).ToString();
    }

    private int EvaluateInputFieldValue(int value)
    {
        if (value == int.MaxValue)
        {
            return currentValue;
        }
        else if (value > maxValue)
        {
            return maxValue;
        }
        else if (value < minValue)
        {
            return minValue;
        }
        else
        {
            return value;
        }
    }
}
