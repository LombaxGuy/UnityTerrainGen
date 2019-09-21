using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pluggable AI/Decisions/Example")]
public class ExampleDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        return SomeDecision();
    }

    private bool SomeDecision()
    {
        Debug.Log("A decision was made.");
        return true;
    }
}
