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
        //rooms = rooms.OrderByDescending(x => x.depth).ToList();
        rooms = ShuffleRooms(rooms);

        int amountOfWater = 5;

        int itemsPlaced = 0;
        // Start spawning items
        foreach (Room r in rooms)
        {
            if (itemsPlaced >= amountOfWater) { break; }

            #region V1 - TOTAL RANDOMIZATION
            Orientation orientation;
            if (ItemManager.Instance.TryGetRandomValidWall(r, out orientation))
            {
                //ItemManager.Instance.CreateAndAddItem(Item.Type.water, r, orientation);
                ItemManager.Instance.CreateAndAddItem(GetRandomItem(), r, orientation);
                itemsPlaced++;
            }
            #endregion
        }
        Debug.Log("Items placed: " + itemsPlaced);
    }
    #endregion

    #region Item Randomization
    private Item.Type GetRandomItem(float waterChance = 1f, float woodChance = 1f, float stoneChance = 1f)
    {
        float woodLikelyhood = waterChance;
        float stoneLikelyhood = waterChance + woodChance;
        float total = stoneLikelyhood + stoneChance;

        float rand = Random.Range(0f, total);

        if (rand >= stoneLikelyhood)
        {
            // Spawn STONE
            return Item.Type.saunaStone;
        }
        else if (rand >= woodLikelyhood)
        {
            // Spawn WOOD
            return Item.Type.woodLog;
        }
        else
        {
            // Spawn WATER
            return Item.Type.water;
        }
    }
    #endregion

    #region Room Shuffling
    private List<Room> ShuffleRooms(List<Room> rooms)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Room temp = rooms[0];
            int randomIndex = Random.Range(0, rooms.Count);
            rooms[randomIndex] = temp;

            Debug.Log("Placed room nro: " + i + " into slot nro: " + randomIndex);
        }
        return rooms;
    }
    #endregion
}
