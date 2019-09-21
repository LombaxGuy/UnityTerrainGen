using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pluggable AI/Actions/Example")]
public class ExampleAction : Action
{
    public override void Act(StateController controller)
    {
        PerformSomeAction();
    }

    private void PerformSomeAction()
    {
        Debug.Log("An action was performed.");
    }
}
