using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public void MoveEnemies(List<Enemy> enemies)
    {
        foreach (var e in enemies)
        {
            MoveEnemy(e);
        }
    }

    private void MoveEnemy(Enemy e)
    {
        switch (e.mode)
        {
            case Enemy.Mode.patrol:
                MoveToRandomRoom(e);
                break;
            // =======================
            case Enemy.Mode.hunt:

                break;
        }
    }

    private void MoveToRandomRoom(Enemy e)
    {
        // Choose a random neighbour that isn't the LastRoom

        List<Room> neighbours = ListNeighbours(e.currentRoom);

        if (neighbours.Count == 1)
        {
            // Move to only option
            Debug.Log("Choosing the only neighbour!");
            MoveToRoom(e, e.currentRoom, neighbours[0]);
            return;
        }

        // Otherwise, choose one that isn't the last one
        for (int i = 0; i < neighbours.Count; i++)
        {


            // Check it's not the previous room
            if (neighbours[i] != e.lastRoom)
            {
                // Move there
                Debug.Log("LastRoom: " + e.lastRoom.type + " || CurrentRoom: " + e.currentRoom.type);
                MoveToRoom(e, e.currentRoom, neighbours[i]);
                return;
            }
        }
    }

    private List<Room> ListNeighbours(Room r)
    {
        List<Room> neighbours = new List<Room>();

        if (r.north.neighbour != null) { neighbours.Add(r.north.neighbour); }
        if (r.east.neighbour != null) { neighbours.Add(r.east.neighbour); }
        if (r.south.neighbour != null) { neighbours.Add(r.south.neighbour); }
        if (r.west.neighbour != null) { neighbours.Add(r.west.neighbour); }

        // Shuffle
        neighbours = RoomShuffler.ShuffleRooms(neighbours);
        return neighbours;
    }

    private void MoveToRoom(Enemy e, Room fromRoom, Room toRoom)
    {
        toRoom.monster = e;
        fromRoom.monster = null;

        e.currentRoom = toRoom;
        e.lastRoom = fromRoom;
    }
}
