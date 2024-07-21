using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelaunatorSharp;
using UnityEngine;

public static class KLVLGEN
{
    // METHODS
    // Create KLVLOBJ_Level Objects
    public static KLVLOBJ_Level CreateLevel(string setName, float setWidth = 10, float setDepth = 10)
    {
        KLVLOBJ_Level Level = KLVLOBJ_Level.New();
        Level.GameObject().name = setName;
        Level.Dimensions(setWidth, setDepth);
        return Level;
    }

    // Create KLVLOBJ_Room Objects
    public static KLVLOBJ_Room CreateRoom(float setWidth, float setDepth, KLVLOBJ_Level setLevel = null)
    {
        KLVLOBJ_Room Room = KLVLOBJ_Room.New();
        Room.Dimensions(setWidth, setDepth);

        if (setLevel != null)
        {
            Room.GameObject().transform.parent = setLevel.GameObject().transform;
        }

        return Room;
    }

    public static KLVLOBJ_Room CreateRoom(Vector2 setDimensions, KLVLOBJ_Level setLevel = null)
    {
        return KLVLGEN.CreateRoom(setDimensions.x, setDimensions.y, setLevel);
    }

    // Create KLVLOBJ_Region Objects
    public static KLVLOBJ_Region CreateRegion(float setWidth, float setDepth, KLVLOBJ_Region setParent = null)
    {
        KLVLOBJ_Region Region = KLVLOBJ_Region.New();
        Region.Dimensions(setWidth, setDepth);

        if (setParent != null)
        {
            Region.GameObject().transform.parent = setParent.GameObject().transform;
        }

        return Region;
    }

    public static KLVLOBJ_Region CreateRegion(float setWidth, float setDepth, GameObject setParent = null)
    {
        KLVLOBJ_Region Region = KLVLOBJ_Region.New();
        Region.Dimensions(setWidth, setDepth);

        if (setParent != null)
        {
            Region.GameObject().transform.parent = setParent.transform;
        }

        return Region;
    }

    // Create KLVLOBJ_Pathway Objects
    public static KLVLOBJ_Pathway CreatePathway(KLVLOBJ_Region setR0, KLVLOBJ_Region setR1, GameObject setParent = null)
    {
        KLVLOBJ_Pathway Pathway = KLVLOBJ_Pathway.New(setParent);

        Pathway.R0(setR0);
        Pathway.R1(setR1);

        return Pathway;
    }

    public static KLVLOBJ_Pathway CreatePathway(KLVLOBJ_Region setR0, KLVLOBJ_Region setR1, KLVLOBJ_Region setParent = null)
    {
        if (setParent != null)
        {
            return KLVLGEN.CreatePathway(setR0, setR1, setParent.GameObject());
        }
        else
        {
            return KLVLGEN.CreatePathway(setR0, setR1, (GameObject) null);
        }
    }

    // Create a KLVLOBJ_Configuration Instances
    public static KLVLOBJ_Configurator CreateConfiguration()
    {
        return new KLVLOBJ_Configurator();
    }

    // Create a KLVLGEN_Pipeline Instances
    public static KLVLGEN_Pipeline CreatePipeline(KLVLOBJ_Level Level, KLVLOBJ_Configuration Configuration)
    {
        return new KLVLGEN_Pipeline(Level, Configuration);
    }

    // Create KLVLOBJ_Connection Instances
    public static List<KLVLOBJ_Connection> CreateConnections(IEnumerable<KLVLOBJ_Region> Regions)
    {
        var Connections = new List<KLVLOBJ_Connection>();
        var PointToRegionMap = new Dictionary<IPoint, KLVLOBJ_Region>();

        // Map Regions to IPoints
        foreach (var Region in Regions)
        {
            var RegionPosition = Region.Position();
            var Point = new Point(RegionPosition.x, RegionPosition.z);
            PointToRegionMap[Point] = Region;
        }

        // Perform Delaunay triangulation
        var DelaunayTriangulation = new Delaunator(PointToRegionMap.Keys.ToArray());

        // Create connections based on triangulated edges
        foreach (var Edge in DelaunayTriangulation.GetEdges())
        {
            KLVLOBJ_Region R0 = PointToRegionMap[Edge.P];
            KLVLOBJ_Region R1 = PointToRegionMap[Edge.Q];
            float Distance = Vector3.Distance(R0.Position(), R1.Position());

            var Connection = new KLVLOBJ_Connection(R0, R1, Distance);
            Connections.Add(Connection);
        }

        return Connections;
    }

    // ADDITIONAL FUNCTIONALITY
    // Round (int) Number to the nearest multiple of (int) Rounding
    public static int RoundInt(int Number, int Rounding)
    {
        return ((Number + Rounding / 2) / Rounding) * Rounding;
    }

    public static Vector3 GridLock3(Vector3 Vector)
    {
        float BiasedX = Mathf.Floor(Vector.x) + ((Vector.x % 1.0f) >= 0.5f ? 1 : 0);
        float RoundedY = Mathf.Floor(Vector.y);
        float BiasedZ = Mathf.Floor(Vector.z) + ((Vector.z % 1.0f) > 0.5f ? 1 : 0);

        return new Vector3(BiasedX, RoundedY, BiasedZ);
    }

    public static int Area(this Vector2Int Vector)
    {
        return Vector.x * Vector.y;
    }
}
