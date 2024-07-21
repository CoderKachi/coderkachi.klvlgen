using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLVLOBJ_Configurator
{
    // VARIABLES
    private KLVLOBJ_Configuration Config;

    // Look at KLVLOBJ_Configuration.PracticalMinimum for explanation
    private int PracticalMinimum;

    // CONSTRUCTOR
    public KLVLOBJ_Configurator()
    {
        Config = new KLVLOBJ_Configuration();
        PracticalMinimum = Config.PracticalMinimum;
    }

    // METHODS
    public KLVLOBJ_Configurator RoomConstraints(int MinWidth, int MinDepth, int MaxWidth, int MaxDepth)
    {
        // ERROR CHECK
        // Make sure Maxs are greater than Mins
        if (MaxWidth < MinWidth || MaxDepth < MinDepth)
        {
            Debug.LogError("Max(Width/Depth) must be greater than Min values!");
            MaxWidth = Math.Max(MaxWidth, MinWidth);
            MaxDepth = Math.Max(MaxDepth, MinDepth);
        }

        // Make sure there are no Mins or Maxs less than the PracticalMinimum
        if (MinWidth < PracticalMinimum || MaxWidth < PracticalMinimum || MinDepth < PracticalMinimum || MaxDepth < PracticalMinimum)
        {
            Debug.LogError("Min(Width/Depth) and Max(Width/Depth) must be equal or greater than " + PracticalMinimum.ToString() + "!");
            MinWidth = Math.Max(MinWidth, PracticalMinimum);
            MaxWidth = Math.Max(MaxWidth, PracticalMinimum);
            MinDepth = Math.Max(MinDepth, PracticalMinimum);
            MaxDepth = Math.Max(MaxDepth, PracticalMinimum);
        }

        Config.Room.MinWidth = MinWidth;
        Config.Room.MaxWidth = MaxWidth;
        Config.Room.MinDepth = MinDepth;
        Config.Room.MaxDepth = MaxDepth;
        return this;
    }

    public KLVLOBJ_Configurator RoomRounding(int setRounding)
    {
        // ERROR CHECK
        if (setRounding > Config.Room.MinWidth || setRounding > Config.Room.MinDepth)
        {
            Debug.LogError("Rounding should be less than or equal to the smallest RoomConstraint!");
            setRounding = Math.Min(Config.Room.MinWidth, Config.Room.MinDepth);
        }

        // Do not allow users to set rounding to 0
        Config.Room.Rounding = Math.Max(1, setRounding); 
        
        return this;
    }

    public KLVLOBJ_Configurator MinimumRooms(int setMinimum)
    {
        Config.Room.Minimum = setMinimum;
        return this;
    }

    public KLVLOBJ_Configurator AddDimensionsWithin(int MinWidth = 3, int MinDepth = 3, int MaxWidth = 3, int MaxDepth = 3)
    {
        int RoundingIterator = Math.Max(1, Config.Room.Rounding);

        for (int width = MinWidth; width <= MaxWidth; width += RoundingIterator)
        {
            for (int depth = MinDepth; depth <= MaxDepth; depth += RoundingIterator)
            {
                int roundedWidth = KLVLGEN.RoundInt(width, RoundingIterator);
                int roundedDepth = KLVLGEN.RoundInt(depth, RoundingIterator);

                Vector2Int newDimension = new Vector2Int(roundedWidth, roundedDepth);
                if (!Config.Room.DimensionsList.Contains(newDimension))
                {
                    this.AddDimension(newDimension.x, newDimension.y, false);
                }
            }
        }

        return this;
    }

    public KLVLOBJ_Configurator AddDimension(int Width, int Depth, bool Warn = true)
    {
        // ERROR CHECK
        // Make sure room is within constraints
        if (Width < Config.Room.MinWidth || Width > Config.Room.MaxWidth || Depth < Config.Room.MinDepth || Depth > Config.Room.MaxDepth)
        {
            if (Warn)
            {
                Debug.LogError("Dimensions out of bounds!");
            }
            return this;
        }

        // Check if divisible by the Rounding value
        if (Width != KLVLGEN.RoundInt(Width, Config.Room.Rounding) || Depth != KLVLGEN.RoundInt(Depth, Config.Room.Rounding))
        {
            if (Warn)
            {
                Debug.LogWarning("Dimension isn't divisible by the Rounding value...");
                // Continue anyway as this may be intended
            }
        }

        Config.Room.DimensionsList.Add(new Vector2Int(Width, Depth));
        return this;
    }

    public KLVLOBJ_Configurator ConnectionRecycleRate(float setRecycleRate)
    {
        // RecycleRate cannot be a negative value
        Config.Connection.RecycleRate = Math.Max(0, setRecycleRate); 
        return this;
    }

    public KLVLOBJ_Configurator PathwayRecycleChance(float setRecycleChance)
    {
        // RecycleChance cannot be a negative value
        Config.Pathway.RecycleChance = Math.Max(0, setRecycleChance); 

        return this;
    }

    public KLVLOBJ_Configurator Seed(int setSeed)
    {
        Config.Seed = setSeed; 
        Config.RandomGenerator = new System.Random(setSeed);
        return this;
    }

    public KLVLOBJ_Configurator Seed()
    {
        Config.Seed = null; 
        Config.RandomGenerator = new System.Random();
        return this;
    }

    // RUN LAST
    public KLVLOBJ_Configuration Build()
    {
        return Config;
    }
}
