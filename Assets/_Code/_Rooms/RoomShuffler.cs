using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    static public List<Room> OrderByLast(List<Room> rooms)
    {
        // First check that the list isn't empty
        if (rooms.Count == 0) { return rooms; }

        // Order so that the furthest rooms are last
        rooms = rooms.OrderByDescending(x => x.depth).ToList();

        return rooms;
    }

    static public List<Room> OrderBySmallest(List<Room> rooms)
    {
        // First check that the list isn't empty
        if (rooms.Count == 0) { return rooms; }

        // Order so that the furthest rooms are last
        rooms = rooms.OrderBy(x => x.depth).ToList();

        return rooms;
    }
}
