using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K_PathwayGeneration : KLVLGEN_Step
{
    public K_PathwayGeneration()
    {
        this.Name = "Pathway Generation";
    }

    // STEP 5
    // PATHWAY GENERATION
    public override void Execute()
    {
        this.Status = "In Progress";
        this.Output("Creating Pathways...");

        foreach (KLVLOBJ_Connection Connection in Level.ConnectionsPicked)
        {
            // Create a Pathway for each Connection
            KLVLOBJ_Pathway Pathway = KLVLGEN.CreatePathway(Connection.R0(), Connection.R1(), Level);
            Level.Pathways.Add(Pathway);
            Pathway.Generate(Level.RoomsPicked, Level.Pathways);
        }

        this.Status = "Complete";
        this.Output("Pathways created successfully!");
    }
}