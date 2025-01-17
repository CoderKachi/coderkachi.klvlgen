// DEPENDANCIES
using System.Collections.Generic;

// CLASSES
public class K_RoomSelector : KLVLGEN_Step
{
    // CONSTRUCTORS
    public K_RoomSelector()
    {
        this.Name = "Room Selection";
    }

    // ROOM SELECTION
    public override void Execute()
    {
        this.Status = "In Progress";

        // Check if there are enough rooms to pick from
        if (Level.Rooms.Count < Configuration.Room.Minimum)
        {
            this.Output("Not enough rooms available...");
            this.Status = "Failed";
            return;
        }

        // Randomly move Rooms from Level.Rooms to Level.RoomsPicked
        List<KLVLOBJ_Room> RoomsToPick = new List<KLVLOBJ_Room>(Level.Rooms);
        for (int i = 0; i < Configuration.Room.Minimum; i++)
        {
            int RandomIndex = Configuration.RandomGenerator.Next(RoomsToPick.Count);
            KLVLOBJ_Room PickedRoom = RoomsToPick[RandomIndex];
            RoomsToPick.RemoveAt(RandomIndex);
            Level.RoomsPicked.Add(PickedRoom);
        }

        // Move all remaining rooms to RoomsDiscarded
        foreach (var Room in RoomsToPick)
        {
            Level.RoomsDiscarded.Add(Room);
        }
        Level.Rooms.Clear();

        this.Output($"{Configuration.Room.Minimum} rooms selected!");
        this.Status = "Complete";
    }
}
