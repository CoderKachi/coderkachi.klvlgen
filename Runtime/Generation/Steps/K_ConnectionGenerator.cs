using System.Collections.Generic;
using UnityEngine;

public class K_ConnectionGenerator : KLVLGEN_Step
{
    public K_ConnectionGenerator()
    {
        this.Name = "Connection Generation";
    }

    // STEP 4
    // CONNECTION GENERATION
    public override void Execute()
    {
        this.Status = "In Progress";
        this.Output("Creating Connections...");
        MinimumConnections();
        this.Output("Recyling discarded Connections...");
        ConnectionRecycling();
        this.Status = "Complete";
    }

    private void MinimumConnections()
    {
        // Create connections between all the Rooms that were picked
        // This is done using Delaunay Triangulation
        var Connections = KLVLGEN.CreateConnections(Level.RoomsPicked);
        Level.Connections = Connections;

        // Create a list of "Minimum" connections using a minimum spanning tree
        // Prim's Algothim will be used here
        var MinimumConnections = new List<KLVLOBJ_Connection>();
        var InMST = new HashSet<KLVLOBJ_Region>();
        var PriorityQueue = new SortedList<float, KLVLOBJ_Connection>(new DuplicateKeyBypass<float>());

        if (Level.RoomsPicked.Count > 0)
        {
            var StartRoom = Level.RoomsPicked[0];
            InMST.Add(StartRoom);

            // Initial edges
            foreach (var Connection in Connections)
            {
                if (Connection.R0() == StartRoom || Connection.R1() == StartRoom)
                {
                    PriorityQueue.Add(Connection.Distance(), Connection);
                }
            }

            while (PriorityQueue.Count > 0)
            {
                var MinEdge = PriorityQueue.Values[0];
                PriorityQueue.RemoveAt(0);

                var NextNode = !InMST.Contains(MinEdge.R0()) ? MinEdge.R0() : (!InMST.Contains(MinEdge.R1()) ? MinEdge.R1() : null);

                if (NextNode != null)
                {
                    MinimumConnections.Add(MinEdge);
                    InMST.Add(NextNode);

                    // Add new edges
                    foreach (var Connection in Connections)
                    {
                        if ((Connection.R0() == NextNode || Connection.R1() == NextNode) && !InMST.Contains(Connection.R0() == NextNode ? Connection.R1() : Connection.R0()))
                        {
                            PriorityQueue.Add(Connection.Distance(), Connection);
                        }
                    }
                }
            }
        }

        Level.ConnectionsPicked = MinimumConnections;
        this.Output($"{Connections.Count} Connections created!");
        this.Output($"{Level.ConnectionsPicked.Count} Connections selected!");

        Level.ConnectionsDiscarded = new List<KLVLOBJ_Connection>();
        foreach (var Connection in Connections)
        {
            if (!MinimumConnections.Contains(Connection))
            {
                Level.ConnectionsDiscarded.Add(Connection);
            }
        }
        this.Output($"{Level.ConnectionsDiscarded.Count} Connections discarded!");
    }

    private void ConnectionRecycling()
    {
        int Total = (int)(Level.ConnectionsDiscarded.Count * Configuration.Connection.RecycleRate);
        for (int i = 0; i < Total && Level.ConnectionsDiscarded.Count > 0; i++)
        {
            // Randomly select an index to recycle
            int RandomIndex = Configuration.RandomGenerator.Next(Level.ConnectionsDiscarded.Count);
            KLVLOBJ_Connection ConnectionToRecycle = Level.ConnectionsDiscarded[RandomIndex];

            // Move the selected connection to the picked list
            Level.ConnectionsPicked.Add(ConnectionToRecycle);
            Level.ConnectionsDiscarded.RemoveAt(RandomIndex);
        }

        this.Output($"{Total} Connections recycled!");
    }
    
}

class DuplicateKeyBypass<TKey> : IComparer<TKey> where TKey : System.IComparable
{
    // SortedList's default behaviour doesn't allow duplicate keys
    // This is a problem as Connections may (and probably will) have the same distance as each other
    // This can be bnypassed by pretending 1 of the keys are greater than the other when equal
    public int Compare(TKey X, TKey Y)
    {
        int Result = X.CompareTo(Y);
        return Result == 0 ? 1 : Result;
    }
}
