// DEPENDANCIES
using System.Collections.Generic;
using UnityEngine;

// CLASSES
public class KLVLOBJ_Level : KLVLOBJ_Region
{
    // LOCAL CLASSES
    public class Visuals
    {
        // VARIABLES
        public bool Enabled = true;
        public bool Rooms = true;
        public bool RoomsPicked = true;
        public bool RoomsDiscarded = true;
        public bool ConnectionsPicked = true;
        public bool ConnectionsDiscarded = true;
        public bool Pathways = true;
    }

    // VARIABLES
    public Visuals Debug = new Visuals();

    // These are intended to be modified as they're passed down the Pipeline
    public List<KLVLOBJ_Room> Rooms = new List<KLVLOBJ_Room>();
    public List<KLVLOBJ_Room> RoomsPicked = new List<KLVLOBJ_Room>();
    public List<KLVLOBJ_Room> RoomsDiscarded = new List<KLVLOBJ_Room>();

    public List<KLVLOBJ_Connection> Connections = new List<KLVLOBJ_Connection>();
    public List<KLVLOBJ_Connection> ConnectionsPicked = new List<KLVLOBJ_Connection>();
    public List<KLVLOBJ_Connection> ConnectionsDiscarded = new List<KLVLOBJ_Connection>();

    public List<KLVLOBJ_Pathway> Pathways = new List<KLVLOBJ_Pathway>();

    // INSTANCE FACTORY
    public new static KLVLOBJ_Level New(GameObject setParent = null)
    {
        // Create a new GameObject and add KLVLOBJ_Level Component
        KLVLOBJ_Level Level = New<KLVLOBJ_Level>(setParent);

        return Level;
    }

    // METHODS
    public void SetCellSize(float setValue)
    {
        GameObject().transform.localScale = new Vector3(setValue, 1, setValue);
    }

    void OnDrawGizmos()
    {
        if (this.Debug.Enabled == true)
        {
            // Rooms
            if (this.Debug.Rooms) GizmoRooms();
            if (this.Debug.RoomsDiscarded == true) GizmoDiscardedRooms();
            if (this.Debug.RoomsPicked) GizmoPickedRooms();

            // Connections
            if (this.Debug.ConnectionsDiscarded == true) GizmoConnectionsDiscarded();
            if (this.Debug.ConnectionsPicked) GizmoConnectionsPicked();

            // Pathways
            if (this.Debug.Pathways) GizmoPathways();
        }
    }

    private void GizmoRooms()
    {
        foreach (KLVLOBJ_Room Room in Rooms)
        {
            Vector3 RoomPosition = Room.Position();
            Vector3 RoomSize = new Vector3(Room.Width(), 1, Room.Depth());

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(RoomPosition, RoomSize);
        }
    }

    private void GizmoPickedRooms()
    {
        foreach (KLVLOBJ_Room Room in RoomsPicked)
        {
            Vector3 RoomPosition = Room.Position();
            Vector3 RoomSize = new Vector3(Room.Width(), 1, Room.Depth());

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(RoomPosition, RoomSize);
        }
    }

    private void GizmoDiscardedRooms()
    {
        foreach (KLVLOBJ_Room Room in RoomsDiscarded)
        {
            Vector3 RoomPosition = Room.Position();
            Vector3 RoomSize = new Vector3(Room.Width(), 1, Room.Depth());

            Gizmos.color = Color.grey;
            Gizmos.DrawWireCube(RoomPosition, RoomSize);
        }
    }

    private void GizmoPathways()
    {
        foreach (KLVLOBJ_Pathway Pathway in this.Pathways)
        {
            if (Pathway.R0() != null && Pathway.R1() != null)
            {
                if (Pathway.Point0().x == Pathway.Point2().x || Pathway.Point0().z == Pathway.Point2().z)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(Pathway.Point0(), Pathway.Point2());
                }
                else
                {
                    var Points = new Vector3[4]
                    {
                        Pathway.Point0(),
                        Pathway.Point1(),
                        Pathway.Point1(),
                        Pathway.Point2()
                    };
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLineList(Points);
                }

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(Pathway.Point0(), new Vector3(0.5f,1,0.5f));
                Gizmos.DrawCube(Pathway.Point2(), new Vector3(0.5f,1,0.5f));
            }
        }
    }

    private void GizmoConnectionsPicked()
    {
        foreach (KLVLOBJ_Connection Connection in this.ConnectionsPicked)
        {
            if (Connection.R0() != null && Connection.R1() != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(Connection.R0().Position(), Connection.R1().Position());
            }
        }
    }

    private void GizmoConnectionsDiscarded()
    {
        foreach (KLVLOBJ_Connection Connection in this.ConnectionsDiscarded)
        {
            if (Connection.R0() != null && Connection.R1() != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(Connection.R0().Position(), Connection.R1().Position());
            }
        }
    }
}

