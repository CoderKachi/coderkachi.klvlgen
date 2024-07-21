// CLASSES
public class K_PathwayGeneration : KLVLGEN_Step
{
    // CONSTRUCTORS
    public K_PathwayGeneration()
    {
        this.Name = "Pathway Generation";
    }

    // PATHWAY GENERATION
    public override void Execute()
    {
        this.Status = "In Progress";
        this.Output("Creating pathways...");

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