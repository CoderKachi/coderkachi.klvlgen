// DEPENDANCIES
using UnityEngine;

// CLASSES
public class KLVLOBJ_Connection
{
    // VARIABLES
    protected KLVLOBJ_Region _R0;
    protected KLVLOBJ_Region _R1;

    // GETTERS
    public KLVLOBJ_Region R0() => this._R0;
    public KLVLOBJ_Region R1() => this._R1;
    public float Distance() => Vector2.Distance(this.R0().Position(), this.R0().Position());

    // SETTERS
    public void R0(KLVLOBJ_Region setR0) => this._R0 = setR0;
    public void R1(KLVLOBJ_Region setR1) => this._R1 = setR1;

    public KLVLOBJ_Connection(KLVLOBJ_Region setR0, KLVLOBJ_Region setR1, float setDistance)
    {
        this.R0(setR0);
        this.R1(setR1);
    }
}
