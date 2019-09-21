using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    private static string name = "EventManager";

    public static event System.Action OnHeightMapSettingsChanged;
    public static event System.Action OnFalloffMapSettingsChanged;

    public static void InvokeOnHeightMapSettingsChanged()
    {
        if (OnHeightMapSettingsChanged != null)
        {
            Debug.Log(name + ": Event 'OnHeightMapSettingsChanged' invoked.");
            OnHeightMapSettingsChanged.Invoke();
        }
        else
        {
            Debug.Log(name + ": Event 'OnHeightMapSettingsChanged' was not invoked because nothing subscribes to it.");
        }
    }

    public static void InvokeOnFalloffMapSettingsChanged()
    {
        if (OnFalloffMapSettingsChanged != null)
        {
            Debug.Log(name + ": Event 'OnFalloffMapSettingsChanged' invoked.");
            OnFalloffMapSettingsChanged.Invoke();
        }
        else
        {
            Debug.Log(name + ": Event 'OnFalloffMapSettingsChanged' was not invoked because nothing subscribes to it.");
        }
    }
}
