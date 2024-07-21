// DEPENDANCIES
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// CLASSES
public class KLVLOBJ_Pathway : MonoBehaviour
{
    // VARIABLES
    protected KLVLOBJ_Region _Region0;
    protected KLVLOBJ_Region _Region1;
    protected int _Size = 3;
    protected Vector3 _Point0;
    protected Vector3 _Point1;
    protected Vector3 _Point2;

    // GETTERS
    public KLVLOBJ_Region R0() => this._Region0;
    public KLVLOBJ_Region R1() => this._Region1;
    public Vector3 Point0() => this._Point0;
    public Vector3 Point1() => this._Point1;
    public Vector3 Point2() => this._Point2;
    public GameObject GameObject() => this.gameObject;
    public int Size() => this._Size;

    // SETTERS
    public void R0(KLVLOBJ_Region setR0) => this._Region0 = setR0;
    public void R1(KLVLOBJ_Region setR1) => this._Region1 = setR1;
    public void Point0(Vector3 setPoint0) => this._Point0 = setPoint0;
    public void Point1(Vector3 setPoint1) => this._Point1 = setPoint1;
    public void Point2(Vector3 setPoint2) => this._Point2 = setPoint2;
    public void Size(int setSize) => this._Size = setSize;

    // CONSTRUCTOR
    public static KLVLOBJ_Pathway New(GameObject setParent = null)
    {
        // Create a new GameObject and add KLVLOBJ_Pathway component
        GameObject PathwayGameObject = new GameObject("KLVLOBJ_Pathway");
        KLVLOBJ_Pathway Pathway = PathwayGameObject.AddComponent<KLVLOBJ_Pathway>();

        if (setParent != null)
        {
            PathwayGameObject.transform.parent = setParent.transform;
        }

        return Pathway;
    }

    // METHODS
    public void Generate(IEnumerable<KLVLOBJ_Region> Regions, List<KLVLOBJ_Pathway> Pathways, int MinimumForLine = 3)
    {
        // ERROR CHECKING
        if (this.R0() == null || this.R1() == null)
        {
            Debug.LogError("Regions R0 and R1 must be set!");
            return;
        }

        // CALCULATE
        // Positions and Dimensions of Region 0
        Vector3 PositionR0 = this.R0().Position();
        Vector2 DimensionsR0 = this.R0().Dimensions();
        float HalfWidthR0 = this.R0().Width() / 2.0f;
        float HalfDepthR0 = this.R0().Depth() / 2.0f;

        // Positions and Dimensions of Region 1
        Vector3 PositionR1 = this.R1().Position();
        Vector2 DimensionsR1 = this.R1().Dimensions();
        float HalfWidthR1 = this.R1().Width() / 2.0f;
        float HalfDepthR1 = this.R1().Depth() / 2.0f;

        // Positions of Points
        Vector3 StartPosition = PositionR0;
        Vector3 EndPosition = PositionR1;
        Vector3 MidPosition = PositionR0 + PositionR1 / 2.0f;

        // Overlaps for X and Z axis
        float MinOverlapX = Mathf.Max(PositionR0.x - HalfWidthR0, PositionR1.x - HalfWidthR1);
        float MaxOverlapX = Mathf.Min(PositionR0.x + HalfWidthR0, PositionR1.x + HalfWidthR1);
        float OverlapX = MaxOverlapX - MinOverlapX;

        float MinOverlapZ = Mathf.Max(PositionR0.z - HalfDepthR0, PositionR1.z - HalfDepthR1);
        float MaxOverlapZ = Mathf.Min(PositionR0.z + HalfDepthR0, PositionR1.z + HalfDepthR1);
        float OverlapZ = MaxOverlapZ - MinOverlapZ;

        // Checking if a straight connection can be made in X or Z Axis
        bool ConnectStraightX = OverlapX >= MinimumForLine;
        bool ConnectStraightZ = OverlapZ >= MinimumForLine;

        if (ConnectStraightX) 
        {
            // Connect horizontally (X Axis)
            StartPosition = new Vector3(MinOverlapX + OverlapX / 2, 0, PositionR0.z);
            EndPosition = new Vector3(MinOverlapX + OverlapX / 2, 0, PositionR1.z);
            
            // Adjust to perimeter
            StartPosition.z = Mathf.Min(PositionR0.z + HalfDepthR0, PositionR1.z + HalfDepthR1);
            EndPosition.z = Mathf.Max(PositionR0.z - HalfDepthR0, PositionR1.z - HalfDepthR1);
        } 
        else if (ConnectStraightZ) 
        {
            // Connect vertically (Z Axis)
            StartPosition = new Vector3(PositionR0.x, 0, MinOverlapZ + OverlapZ / 2);
            EndPosition = new Vector3(PositionR1.x, 0, MinOverlapZ + OverlapZ / 2);

            // Adjust to perimeter
            StartPosition.x = Mathf.Min(PositionR0.x + HalfWidthR0 , PositionR1.x + HalfWidthR1);
            EndPosition.x = Mathf.Max(PositionR0.x - HalfWidthR0 , PositionR1.x - HalfWidthR1);
            MidPosition = (StartPosition + EndPosition) / 2.0f;
        }
        else
        {
            // L Pathway
            float WidthOffsetR0 = HalfWidthR0 * ((PositionR0.x > PositionR1.x) ? -1 : 1); // Is R0 to the Right of R1? Yes / No
            float LengthOffsetR1 = HalfDepthR1 * ((PositionR1.z > PositionR0.z) ? -1 : 1); // Is R0 Behind R1?

            StartPosition += new Vector3(WidthOffsetR0, 0, 0);
            EndPosition += new Vector3(0, 0, LengthOffsetR1);
            MidPosition = new Vector3(EndPosition.x, 0, StartPosition.z);

            // Check if the L Pathway intersects with the provided regions
            var IntersectedRegions = this.LIntersect(StartPosition, MidPosition, EndPosition, Regions);
            if (IntersectedRegions.Count > 0)
            {
                // Flip it
                StartPosition = new Vector3(PositionR0.x, 0, PositionR0.z + HalfDepthR0 * ((PositionR0.z > PositionR1.z) ? -1 : 1));
                EndPosition = new Vector3(PositionR1.x + HalfWidthR1 * ((PositionR1.x > PositionR0.x) ? -1 : 1), 0, PositionR1.z);
                MidPosition = new Vector3(StartPosition.x, 0, EndPosition.z);
            }

            // Adjustment
            int FavouredOffset = MinimumForLine / 2;
            int Offset = MinimumForLine - FavouredOffset; 
        }

        // APPLY
        this.Point0(KLVLGEN.GridLock3(StartPosition));
        this.Point1(KLVLGEN.GridLock3(MidPosition));
        this.Point2(KLVLGEN.GridLock3(EndPosition));
    }

    public List<KLVLOBJ_Region> Intersect(Vector3 StartPoint, Vector3 EndPoint, IEnumerable<KLVLOBJ_Region> Regions)
    {
        List<KLVLOBJ_Region> HitRegions = new List<KLVLOBJ_Region>();

        Vector3 Direction = EndPoint - StartPoint;
        float Length = Direction.magnitude;
        Direction.Normalize();

        RaycastHit[] Hits = Physics.RaycastAll(StartPoint, Direction, Length);

        // HashSet to avoid duplicates
        HashSet<KLVLOBJ_Region> UniqueHitRegions = new HashSet<KLVLOBJ_Region>();

        foreach (RaycastHit Hit in Hits)
        {
            KLVLOBJ_Region Region = Hit.collider.GetComponent<KLVLOBJ_Region>();
            if (Region != null && Regions.Contains(Region) && Region != this.R0() && Region != this.R1())
            {
                UniqueHitRegions.Add(Region);
            }
        }

        // Convert the HashSet back to a List for the return type compatibility
        HitRegions.AddRange(UniqueHitRegions);
        return HitRegions;
    }

    public List<KLVLOBJ_Region> LIntersect(Vector3 StartPoint, Vector3 MidPoint, Vector3 EndPoint, IEnumerable<KLVLOBJ_Region> Regions)
    {
        // StartPoint to MidPoint and MidPoint to EndPoint
        List<KLVLOBJ_Region> Regions0 = Intersect(StartPoint, MidPoint, Regions);
        List<KLVLOBJ_Region> Regions1 = Intersect(MidPoint, EndPoint, Regions);

        // HashSet to store unique regions from both legs
        HashSet<KLVLOBJ_Region> UniqueRegions = new HashSet<KLVLOBJ_Region>(Regions0);

        // Add second leg regions, automatically removing duplicates
        foreach (var Region in Regions1)
        {
            UniqueRegions.Add(Region);
        }

        // Convert the HashSet back to a List for the return type compatibility
        return new List<KLVLOBJ_Region>(UniqueRegions);
    }
}
