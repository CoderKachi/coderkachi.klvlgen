// DEPENDANCIES
using System.Collections.Generic;
using UnityEngine;

// CLASSES
public class KLVLOBJ_Region : MonoBehaviour
{
    // VARIABLES
    [SerializeField] protected Vector3 Size = Vector3.one;
    [SerializeField] protected Vector3 _Position = Vector3.zero;

    // GETTERS
    public float Width() => this.Size.x;
    public float Depth() => this.Size.z;
    public float Area() => this.Size.x * this.Size.z;
    public Vector3 Position() => this._Position;
    public Vector2 Dimensions() => new Vector2(this.Size.x, this.Size.z);
    public Vector3 Pivot()
    {
        float HalfWidth = this.Width() / 2.0f;
        float HalfDepth = this.Depth() / 2.0f;

        // Return the position of the Top-Left corner relative to the Unity Engine (Z and -X)
        return new Vector3
        (
            gameObject.transform.position.x - HalfWidth,
            gameObject.transform.position.y,
            gameObject.transform.position.z + HalfDepth
        );
    }

    public bool HasCollider() => this.Collider() != null;
    public GameObject GameObject() => this.gameObject;
    public BoxCollider Collider() => this.GetComponent<BoxCollider>();

    // SETTERS
    public void Width(float setWidth) => SetDimensions(new Vector2(setWidth, this.Size.z));
    public void Depth(float setDepth) => SetDimensions(new Vector2(this.Size.x, setDepth));
    public void Dimensions(Vector2 setSize) => SetDimensions(new Vector2(setSize.x, setSize.y));
    public void Dimensions(float setX, float setY) => SetDimensions(new Vector2(setX, setY));
    public void Parent(GameObject setParent) => this.GameObject().transform.parent = setParent.transform;

    // INSTANCE FACTORY
    public static T New<T>(GameObject setParent = null) where T : KLVLOBJ_Region
    {
        // Create a new GameObject 
        GameObject NewGameObject = new GameObject(typeof(T).Name);

        // Add Components
        NewGameObject.AddComponent<BoxCollider>();

        // Add Script (KLVLOBJ_Region)
        T NewRegion = NewGameObject.AddComponent<T>();

        // Set Parent
        if (setParent != null)
        {
            NewGameObject.transform.parent = setParent.transform;  // Set the parent of the GameObject directly
        }

        return NewRegion;
    }

    // To allow intuitive creation of Region objects
    public static KLVLOBJ_Region New(GameObject setParent = null)
    {
        return KLVLOBJ_Region.New<KLVLOBJ_Region>(setParent);
    }

    // UPDATES
    // Call to push updates to the World
    protected virtual void GeometryChanged()
    {
        this.Collider().size = this.Size;
        this.gameObject.transform.localPosition = this._Position;

        this.UpdateCollider();
    }

    // Force update the collider
    protected void UpdateCollider()
    {
        this.Collider().enabled = false;
        this.Collider().enabled = true;
    }

    // TRIGGERS
    // Trigger on changes made in the Inspector
    protected void OnValidate()
    {
        this.GeometryChanged();
    }

    // METHODS
    // Route all changes to Size here to prevent having to call GeometryChanged() everywhere
    protected void SetDimensions(Vector2 setSize)
    {
        this.Size = new Vector3(Mathf.Round(setSize.x), 1, Mathf.Round(setSize.y));
        this.GeometryChanged();
    }

    public void MoveTo(Vector3 setPosition, bool AutoAdjust = true)
    {
        // This method isn't always accurate due to the nature of Grid based systems
        // Odd dimensions require half unit adjustments
        // Adjustments always favour the Bottom-Right (-Z and +X)
        float offsetX = (this.Width() % 2 == 1 && AutoAdjust) ? 0.5f : 0;  // Add 0.5 Units to the X Axis
        float offsetZ = (this.Depth() % 2 == 1 && AutoAdjust) ? -0.5f : 0;  // Subtract 0.5 Units to the Z Axis

        if (AutoAdjust)
        {
            setPosition = new Vector3
            (
                Mathf.Round(setPosition.x), 
                Mathf.Round(setPosition.y),
                Mathf.Round(setPosition.z)
            );
        }

        this._Position = new Vector3
        (
            setPosition.x + offsetX,
            setPosition.y,
            setPosition.z + offsetZ
        );
        this.GeometryChanged();
    }

    public void MoveBy(Vector3 Offset)
    {
        this._Position += Offset;
        this.GeometryChanged();
    }

    public void PivotTo(Vector3 setPosition)
    {
        // Difference between Pivot and setPosition
        Vector3 difference = setPosition - this.Pivot();

        this._Position = new Vector3
        (
            gameObject.transform.position.x + difference.x,
            gameObject.transform.position.y,
            gameObject.transform.position.z + difference.z
        );
        this.GeometryChanged();
    }

    // This method returns a list of regions that intersect with this region
    public List<KLVLOBJ_Region> IsIntersecting(IEnumerable<KLVLOBJ_Region> IncludeList, float Tolerance = 0.01f)
    {
        List<KLVLOBJ_Region> Intersections = new List<KLVLOBJ_Region>();

        // Store original size
        // The idea is to not exclude Regions that are simply touching
        Vector3 OriginalSize = this.Collider().size;

        // Reduce the collider size temporarily
        this.Collider().size = new Vector3
        (
            OriginalSize.x - Tolerance,
            OriginalSize.y - Tolerance,
            OriginalSize.z - Tolerance
        );

        UpdateCollider();

        foreach (KLVLOBJ_Region OtherRegion in IncludeList)
        {
            if (OtherRegion != this) // Ensure not to compare the region with itself
            {
                Collider OtherCollider = OtherRegion.GetComponent<BoxCollider>();
                if (this.Collider().bounds.Intersects(OtherCollider.bounds))
                {
                    Intersections.Add(OtherRegion);
                }
            }
        }

        this.Collider().size = OriginalSize;
        UpdateCollider();

        return Intersections;
    }

    public bool IsIntersectingLine(Vector3 StartPoint, Vector3 EndPoint)
    {
        // Calculate direction and length
        Vector3 Direction = EndPoint - StartPoint;
        float Distance = Direction.magnitude;
        Direction.Normalize();

        // Collect all hits
        RaycastHit[] Hits = Physics.RaycastAll(StartPoint, Direction, Distance);

        // Check if this Region was hit
        foreach (var Hit in Hits)
        {
            if (Hit.collider == this.Collider())
            {
                return true;
            }
        }

        return false;
    }
}
