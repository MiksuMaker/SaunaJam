using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    #region Properties

    #endregion

    #region Setup
    public void SpawnItems()
    {
        // Get a list of rooms
        List<Room> rooms = RoomManager.Instance.roomsList;

        // Arrange according to Depth

        // Start spawning items
        foreach (Room r in rooms)
        {
            // Try to place something on the wall
            if (ItemManager.Instance.CheckValidity(r, Orientation.west))
            {
                ItemManager.Instance.CreateAndAddItem(Item.Type.water, r, Orientation.west);
            }
        }
    }
    #endregion

    #region Item Spawning

    #endregion
}
