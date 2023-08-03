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
        rooms = RoomShuffler.ShuffleRooms(rooms);


        // First, spawn those items that MUST be spawned
        //MustPlaceItem(rooms, 1, 1, 1, 0, 1);
        //MustPlaceItem(rooms, 10, 8, 6);
        MustPlaceItem(rooms, 10, 80, 6);


        int amountOfRandomItems = 5;
        int itemsPlaced = 0;

        // Start spawning items
        foreach (Room r in rooms)
        {
            if (itemsPlaced >= amountOfRandomItems) { break; }

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

    private void MustPlaceItem(List<Room> rooms, int mustWater = 0, int mustWood = 0, int mustStone = 0, int mustWrite = 0, int mustSauna = 0)
    {
        int[] musts = new int[] { mustWater, mustWood, mustStone, mustWrite, mustSauna };
        Item.Type[] types = new Item.Type[] { Item.Type.water, Item.Type.woodLog, Item.Type.saunaStone, Item.Type.writing, Item.Type.sauna };

        // Place them randomly among the provided rooms
        for (int i = 0; i < musts.Length; i++)
        {
            if (musts[i] == 0) { continue; }

            int successes = 0;
            foreach (var r in rooms)
            {
                Orientation orientation;
                if (ItemManager.Instance.TryGetRandomValidWall(r, out orientation))
                {
                    ItemManager.Instance.CreateAndAddItem(types[i], r, orientation);

                    // Check if enough
                    successes++;
                    if (successes >= musts[i]) { break; }
                }
            }

            // Check that all items spawned
            if (successes < musts[i])
            {
                Debug.LogWarning("Not all items of type " + types[i] + " spawned!");

                Debug.LogWarning("Successes: " + successes + ", Musts: " + musts[i]);
            }

            // Reshuffle rooms
            rooms = RoomShuffler.ShuffleRooms(rooms);
        }
    }
    #endregion

    #region Room Shuffling

    private void DebugAllRooms(List<Room> rooms)
    {
        //Debug.LogWarning("==== NEW DEBUG ROUND COMMENCING ====");
        foreach (var r in rooms)
        {
            Debug.Log(r.name);
        }
    }
    #endregion
}

static public class RoomShuffler
{
    static public List<Room> ShuffleRooms(List<Room> rooms)
    {
        // First check that the list isn't empty
        if (rooms.Count == 0) { return rooms; }

        for (int i = 0; i < rooms.Count; i++)
        {
            // Skip first one
            if (i == 0) { continue; }

            Room temp1 = rooms[i];
            int rand = Random.Range(1, rooms.Count);
            Room temp2 = rooms[rand];

            // Change places between the rooms
            rooms[i] = temp2;
            rooms[rand] = temp1;
        }
        return rooms;
    }
}