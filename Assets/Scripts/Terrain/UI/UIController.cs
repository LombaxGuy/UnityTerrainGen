using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIController : MonoBehaviour
{
    public abstract void SetData(object o);
    public abstract object GetData();
}
