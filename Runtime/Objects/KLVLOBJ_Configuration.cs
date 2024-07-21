using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLVLOBJ_Configuration
{
    public class RoomGeneration
    {
        // DYNAMIC
        public int MaxWidth;
        public int MinWidth;
        public int MaxDepth;
        public int MinDepth;

        // STATIC
        public List<Vector2Int> DimensionsList;

        public int Rounding;
        public int Minimum;
        public int Maximum;
    }

    public class ConnectionGeneration
    {
        public float RecycleRate = 0.5f;
    }

    public class PathwayGeneration
    {
        public float RecycleChance = 0.25f;
    }

    public RoomGeneration Room = new RoomGeneration();
    public ConnectionGeneration Connection = new ConnectionGeneration();
    public PathwayGeneration Pathway = new PathwayGeneration();

    // The practical minimum is the lowest value a Room's width or depth can be. It assumes a wall takes up 1 Unit² (1x1), so the smallest room must be 9 Units² (3x3).
    // Of course if users can set this to lower values if walls take up less than 1 whole Unit²
    public int PracticalMinimum = 3;

    // Use of ? allows Seed to be nullable
    public int? Seed = null;
    public System.Random RandomGenerator = new System.Random();

    // CONSTRUCTORS
    public KLVLOBJ_Configuration()
    {
        // ROOM GENERATION VALUES
        Room.MaxWidth = 16;
        Room.MinWidth = 6;
        Room.MaxDepth = 16;
        Room.MinDepth = 6;

        Room.DimensionsList = new List<Vector2Int>();

        Room.Rounding = 1;
        Room.Minimum = 4;
        Room.Maximum = 999;

        // CONNECTION GENERATION VALUES
        Connection.RecycleRate = 0.5f;

        // PATHWAY GENERATION VALUES
        Pathway.RecycleChance = 0.25f;
    }
}

