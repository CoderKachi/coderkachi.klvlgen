using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// This class is for testing purposes and shows how to utilise the Level Generator
public class KLVLTRIGGER : MonoBehaviour
{
    void Start()
    {
        // Create Levels
        var Alpha = KLVLGEN.CreateLevel("Alpha"); // No Dimensions specified
        var Bravo = KLVLGEN.CreateLevel("Bravo", 160, 160);

        // Set the dimensions of Level after creation
        Alpha.Dimensions(50, 50);

        // Create Configuration
        var Configuration = KLVLGEN.CreateConfiguration()
            .RoomConstraints(16, 16, 24, 24)      // Maximum and minimum size a room should be
            .RoomRounding(1)                    // Round Dimensions to this value
            .AddDimensionsWithin(16, 16, 24, 24)  // Adds every possible Dimension that fits within these Constraints
            .MinimumRooms(7)                    // Sets the minimum amount of rooms desired
            .Seed(1337)                         // Sets the seed for the Random Generator
            .Build();                           // Returns a KLVLOBJ_COnfiguration Object

        // Create Pipeline
        var DefaultPipeline = KLVLGEN.CreatePipeline(Alpha, Configuration);

        // Different Levels can be used in the pipeline
        DefaultPipeline.SetLevel(Bravo);

        // Pipelines can be ordered in any way desired
        DefaultPipeline.AddStep(new K_RoomGenerator());

        // Custom steps can be added too, use Context to access the Level, Configuration and the Output method
        DefaultPipeline.AddCustomStep(Context => 
        {
            Context.Output("Hello, I'm a custom step!");
            Context.Output("Goodbye, I'm done now!");
        });
        
        DefaultPipeline.AddStep(new K_RoomDistributor());
        DefaultPipeline.AddStep(new K_RoomSelector());
        DefaultPipeline.AddStep(new K_ConnectionGenerator());
        DefaultPipeline.AddStep(new K_PathwayGeneration());

        // Execute the pipeline
        DefaultPipeline.Execute();

        // Alter debugging visuals
        Bravo.Debug.ConnectionsPicked = false;
        Bravo.Debug.ConnectionsDiscarded = false;
        //Bravo.Debug.Pathways = false;
        //Bravo.Debug.RoomsDiscarded = false;
    }
}
