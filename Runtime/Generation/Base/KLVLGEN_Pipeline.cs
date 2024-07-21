// DEPENDANCIES
using System;
using System.Collections.Generic;
using UnityEngine;

// INTERFACES
// This is used to expose Level and Configuration to Lambda functions
public interface IStepContext
{
    // VARIABLES
    KLVLOBJ_Level Level { get; }
    KLVLOBJ_Configuration Configuration { get; }

    // Mimic the Output function found in KLVLGEN_Step
    public void Output(string Message) => Debug.Log("     " + Message);
}

// CLASSES
public class KLVLGEN_Pipeline : IStepContext
{
    // VARIABLES
    private List<KLVLGEN_Step> Steps = new List<KLVLGEN_Step>();

    public KLVLOBJ_Level Level { get; private set; }
    public KLVLOBJ_Configuration Configuration { get; private set; }

    // CONSTRUCTORS
    public KLVLGEN_Pipeline(KLVLOBJ_Level setLevel, KLVLOBJ_Configuration setConfiguration)
    {
        this.Level = setLevel;
        this.Configuration = setConfiguration;
    }

    // SETTERS
    public void SetLevel(KLVLOBJ_Level setLevel) 
    {
        this.Level = setLevel;
        RefreshSteps();
    }

    // METHODS
    public void AddStep(KLVLGEN_Step Step)
    {
        // Pass a reference to the Level
        Step.Setup(this.Level, this.Configuration);
        this.Steps.Add(Step);
    }

    // Action<T> is a delegate that doesn't return a value
    // The idea of this method is to allow code injections "on the fly"
    // Rather than having to create an entire KLVLGEN_Step Derivative
    public void AddCustomStep(Action<IStepContext> Logic)
    {
        // The Logic written for the step needs to be captured in a lambda function
        // This stops it from being called immediately (Deferred Execution)
        Action StepLogic = () => Logic(this);

        // Add step as normal
        // KLVLGEN_DynamicStep derives from KLVLGEN_Step
        this.AddStep(new KLVLGEN_DynamicStep(StepLogic));
    }

    public void Execute()
    {
        Debug.Log("[Pipeline Start]");
        int TotalStepCount = this.Steps.Count;
        int CurrentStepCount = 1;

        foreach (var Step in this.Steps)
        {
            Debug.Log($"Step {CurrentStepCount} of {TotalStepCount}: {Step.Name}");
            Step.Execute();
            Step.Cleanup();

            CurrentStepCount++;
        }

        Debug.Log("[Pipeline End]");
    }

    private void RefreshSteps()
    {
        foreach (var Step in Steps)
        {
            Step.Setup(this.Level, this.Configuration);
        }
    }
}
