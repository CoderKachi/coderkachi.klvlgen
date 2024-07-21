using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class K_RoomGenerator : KLVLGEN_Step
{
    public K_RoomGenerator()
    {
        this.Name = "Room Generation";
    }

    // STEP 1
    // ROOM GENERATION
    public override void Execute()
    {
        this.Status = "In Progress";

        // Calculate how many rooms could fit in the level if they were as large as possible
        int SectionsWidth = Mathf.FloorToInt(Level.Width() / Configuration.Room.MaxWidth);
        int SectionsDepth = Mathf.FloorToInt(Level.Depth() / Configuration.Room.MaxDepth);
        int MaxRooms = SectionsWidth  * SectionsDepth;

        // Create the rooms
        this.Output("Creating Rooms...");
        for (int i = 0; i < MaxRooms; i++)
        {
            // Randomly select Dimension (Vector2)
            int RandomIndex = Configuration.RandomGenerator.Next(Configuration.Room.DimensionsList.Count);

            // Create the Room
            var CurrentRoom = KLVLGEN.CreateRoom(Configuration.Room.DimensionsList[RandomIndex], Level);
            CurrentRoom.MoveTo(new Vector3(0, 0, 0));

            // Add to the list
            Level.Rooms.Add(CurrentRoom);
        }
        this.Output($"{Level.Rooms.Count} Rooms created!");
        this.Status = "Complete";
    }
}
