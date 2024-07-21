using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class K_RoomDistributor : KLVLGEN_Step
{
    public K_RoomDistributor()
    {
        this.Name = "Room Distribution";
    }

    // STEP 2
    // ROOM DISTRIBUTION
    public override void Execute()
    {
        this.Status = "In Progress";
        this.Output("Resolving intersections...");
        this.ResolveIntersections();
        this.Output("Converging Rooms...");
        this.Convergence();
        this.Status = "Complete";
    }

    // STEP 2 (Part A)
    // INTERSECTION RESOLVING
    private void ResolveIntersections()
    {
        // Currently all the rooms are intersecting each other
        int SectionsWidth = Mathf.FloorToInt(Level.Width() / Configuration.Room.MaxWidth);
        int SectionsDepth = Mathf.FloorToInt(Level.Depth() / Configuration.Room.MaxDepth);

        int RoomIndex = 0;

        for (int W = 0; W < SectionsWidth; W++)
        {
            // Skip if no more rooms are available
            // This shouldn't really be happening though
            if (RoomIndex >= Level.Rooms.Count)
            {
                break;
            }

            for (int D = 0; D < SectionsDepth; D++)
            {
                // Calculate the center of the section
                Vector3 TargetPosition = new Vector3
                (
                    (W * Configuration.Room.MaxWidth) - (Level.Width() / 2.0f) + (Configuration.Room.MaxWidth / 2.0f),
                    0,
                    (D * Configuration.Room.MaxDepth) - (Level.Depth() / 2.0f) + (Configuration.Room.MaxDepth / 2.0f)
                );
                
                // Move the Room to TargetPosition
                var Room = Level.Rooms[RoomIndex];
                Room.MoveTo(TargetPosition);
                RoomIndex++;
            }
        }
    }

    // STEP 2 (Part B)
    // CONVERGENCE
    private void Convergence()
    {
        // It's highly likely that all the rooms are spread too far apart
        // So the idea is the converge them all towards the center of the level
        Vector3 Center = Vector3.zero; // Center of the level

        // First sort the Rooms so the ones cloest to the center have the lowest index
        Level.Rooms.Sort((RoomA, RoomB) =>
        {
            float DistA = (RoomA.Position() - Center).sqrMagnitude;
            float DistB = (RoomB.Position() - Center).sqrMagnitude;
            return DistA.CompareTo(DistB);
        });

        foreach (var Room in Level.Rooms)
        {
            // Attempt to center room on X-Axis
            var ReachedX = Math.Abs(Room.Position().z - Center.z) < 0.5;
            // Get the direction (-X or +X)
            Vector3 DirectionX = new Vector3
            (
                Math.Sign(Center.x - Room.Position().x),
                0,
                0
            );
            
            // Then move the Room until it intersects with another
            while (!Room.IsIntersecting(Level.Rooms.Cast<KLVLOBJ_Region>()).Any() && !ReachedX)
            {
                Room.MoveBy(DirectionX);
                ReachedX = Math.Abs(Room.Position().z - Center.z) < 0.5;
            }
            
            if (!ReachedX)
            {
                // Move back upon intersection
                Room.MoveBy(-DirectionX);
            }

            // Then do the same on the Z-Axis
            var ReachedZ = Math.Abs(Room.Position().z - Center.z) < 0.5;
            Vector3 DirectionZ = new Vector3
            (
                0,
                0,
                Math.Sign(Center.z - Room.Position().z)
            );

            while (!Room.IsIntersecting(Level.Rooms.Cast<KLVLOBJ_Region>()).Any())
            {
                Room.MoveBy(DirectionZ);
            }

            if (!ReachedZ)
            {
                // Move back upon intersection
                Room.MoveBy(-DirectionZ);
            }
        }
    }
}
