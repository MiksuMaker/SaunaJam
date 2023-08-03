using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyMover : MonoBehaviour
{
    #region Movement
    public void UpdateEnemies(List<Enemy> enemies)
    {
        foreach (var e in enemies)
        {
            // Move
            MoveEnemy(e);

            // Try to detect Player
            DetectPlayer(e);
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
                MoveAfterTarget(e);
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
            //Debug.Log("Choosing the only neighbour!");
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
                //Debug.Log("LastRoom: " + e.lastRoom.type + " || CurrentRoom: " + e.currentRoom.type);
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

    private List<Room> ListNeighbours(Room r, Room previous)
    {
        List<Room> neighbours = new List<Room>();

        if (r.north.neighbour != null && r.north.neighbour != previous) { neighbours.Add(r.north.neighbour); }
        if (r.east.neighbour != null && r.east.neighbour != previous) { neighbours.Add(r.east.neighbour); }
        if (r.south.neighbour != null && r.south.neighbour != previous) { neighbours.Add(r.south.neighbour); }
        if (r.west.neighbour != null && r.west.neighbour != previous) { neighbours.Add(r.west.neighbour); }

        // Shuffle
        neighbours = RoomShuffler.ShuffleRooms(neighbours);
        return neighbours;
    }

    private void MoveToRoom(Enemy e, Room fromRoom, Room toRoom)
    {
        // First check that Player isn't on the way
        if (CheckRoomForPlayer(e, toRoom)) { return; }

        // If not, proceed to Move enemy
        toRoom.monster = e;
        fromRoom.monster = null;

        e.currentRoom = toRoom;
        e.lastRoom = fromRoom;
    }
    #endregion

    #region Hunting
    private void MoveAfterTarget(Enemy e)
    {
        RoomManager rm = RoomManager.Instance;

        if (IsPlayerWithinSight(e))
        { e.targetRoom = rm.currentRoom; }

        // Choose the Room that is closest to Player
        //List<(Orientation, int)> results = rm.ClosestOrientationToOtherRoom(e.currentRoom, rm.currentRoom, 3);
        List<(Orientation, int)> results = rm.ClosestOrientationToOtherRoom(e.currentRoom, e.targetRoom, 3);

        // Check that it is not null
        if (results.Count == 0)
        {
            CalmEnemy(e);
            return;
        }

        // Try to move there
        Room neighbour = e.currentRoom; // Default
        switch (results[0].Item1)
        {
            case Orientation.north: neighbour = e.currentRoom.north.neighbour; break;
            case Orientation.west: neighbour = e.currentRoom.west.neighbour; break;
            case Orientation.east: neighbour = e.currentRoom.east.neighbour; break;
            case Orientation.south: neighbour = e.currentRoom.south.neighbour; break;
        }
        Debug.Log("Trying to move to " + results[0].Item1 + " || Neighbour null: " + (neighbour == null));

        // Move
        MoveToRoom(e, e.currentRoom, neighbour);
    }
    #endregion

    #region Detection
    private bool CheckRoomForPlayer(Enemy e, Room toRoom)
    {
        if (RoomManager.Instance.currentRoom == toRoom)
        {

            // If not Aggroed already
            // --> Alert this enemy to the presence of Player
            if (e.mode == Enemy.Mode.patrol)
            {
                AggroEnemy(e);
            }
            else
            {
                // If ALREADY Aggroed --> Kill Player
                AttackPlayer(e);
            }

            return true;
        }

        // No Player, free to Move
        return false;
    }

    private void DetectPlayer(Enemy e)
    {
        // Check if Current Room (Player Pos) is within sight
        if (IsPlayerWithinSight(e))
        {
            // If yes, go after them!
            AggroEnemy(e);
        }
        else
        {
            // If not, check out the last position
            // --> Continue patrolling
            CalmEnemy(e);
        }


    }

    private bool IsPlayerWithinSight(Enemy e)
    {
        // Check if Player is within sight
        RoomManager rm = RoomManager.Instance;

        int dist = 3;
        if (rm.IsOtherRoomNear(e.currentRoom, rm.currentRoom, dist))
        {
            //Debug.Log("Player is within " + dist + " rooms");
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Aggression
    public void StumbleIntoEnemy(Enemy e)
    {
        // Check if that Enemy is Aggroed
        if (e.mode == Enemy.Mode.hunt)
        {
            // Attack Player!
            AttackPlayer(e);
        }
        else
        {
            // just Aggro the Enemy
            AggroEnemy(e);
        }
    }

    public void AggroEnemy(Enemy e)
    {
        Debug.Log("Going AFTER the Player");

        UpdateTargetPos(e);

        // If not, just start hunting them
        e.mode = Enemy.Mode.hunt;
    }

    private void UpdateTargetPos(Enemy e)
    {
        e.targetRoom = RoomManager.Instance.currentRoom;
    }

    private void CalmEnemy(Enemy e)
    {
        e.mode = Enemy.Mode.patrol;
    }

    private void AttackPlayer(Enemy e)
    {
        Debug.Log("ATTACKING PLAYER");
    }
    #endregion

    #region RELOCATION
    public void RelocateEnemy(Enemy e)
    {
        // First calm them
        CalmEnemy(e);

        // Remove reference from the room
        e.currentRoom.monster = null;

        // Then teleport them
        int teleportDistance = 2;
        e.currentRoom = GetRandomRoomAt(e.currentRoom, teleportDistance);
    }

    public Room GetRandomRoomAt(Room original, int howFar)
    {
        List<List<Room>> roomLists = new List<List<Room>>();
        List<Room> tempList;
        Room temp = original;

        // Sample (tries) amount of different paths
        int tries = 3;
        // How many TRIES
        for (int i = 0; i < tries; i++)
        {

            // Reset temp
            temp = original;
            roomLists.Add(new List<Room>());
            roomLists[i].Add(temp);

            // How FAR it tries to go
            for (int c = 0; c < howFar; c++)
            {
                // Get a neighbour
                tempList = ListNeighbours(temp, temp);

                // Check that they're not empty
                if (tempList.Count == 0) { break; }

                // Update temp
                temp = tempList[0];

                // Add it to the list
                roomLists[i].Add(temp);
            }
            // Flip the list, so that last one is first
            roomLists[i].Reverse();
        }

        // Order Lists by which goes the longest
        roomLists.OrderByDescending(x => x.Count);

        // Return that room
        return roomLists[0][0];
    }
    #endregion
}
