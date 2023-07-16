using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    // Abstract object. Used to spawn in Item Manifests (gameobjects with distinct behaviours)

    public enum Type
    {
        // Major Objects
        sauna,

        // Interactables
        writing,

        // Resources
        water, saunaStone, woodLog
    }

    public Type type;
    public Orientation whichWallIsItOn;

}
