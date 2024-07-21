using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KLVLOBJ_Room : KLVLOBJ_Region
{
    // INSTANCE FACTORY
    public new static KLVLOBJ_Room New(GameObject setParent = null)
    {
        // Create a new GameObject and add KLVLOBJ_Room Component
        KLVLOBJ_Room Room = New<KLVLOBJ_Room>(setParent);

        return Room;
    }
}
