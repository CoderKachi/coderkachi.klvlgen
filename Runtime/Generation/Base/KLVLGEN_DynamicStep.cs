using System;
using UnityEngine;

// CLASS
public class KLVLGEN_DynamicStep : KLVLGEN_Step
{
    // VARIABLES
    // Ensure the Logic (Action) can't be changed after instantiation
    private readonly Action Logic;

    // CONSTRUCTOR
    public KLVLGEN_DynamicStep(Action setAction)
    {
        Logic = setAction;
        Name = "[Custom]";
    }

    // METHODS
    public override void Execute()
    {
        Logic();
    }
}
