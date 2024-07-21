// DEPENDANCIES
using UnityEngine;

// CLASSES
public abstract class KLVLGEN_Step
{
    // PROPERTIES
    public string Name { get; protected set; }
    public string Status { get; protected set; }

    public KLVLOBJ_Level Level;
    public KLVLOBJ_Configuration Configuration;

    // METHODS
    public virtual void Setup(KLVLOBJ_Level Level, KLVLOBJ_Configuration Configuration)
    {
        this.Level = Level;
        this.Configuration = Configuration;
    }

    public abstract void Execute();

    public virtual void Cleanup() {}

    // To help make debugging easier
    public void Output(string Message)
    {
        Debug.Log("     " + Message);
    }
}
