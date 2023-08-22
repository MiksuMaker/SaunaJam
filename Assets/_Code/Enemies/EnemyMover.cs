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
                // If you're gnome, check that Player isn't there
                if (e.type == Enemy.Type.gnome && RoomManager.Instance.currentRoom == neighbours[i]) { return; }

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
        // Check that no other Enemy is occupying the room
        if (toRoom.monster != null) { return; }

        // First check that Player isn't on the way
        if (e.mode == Enemy.Mode.patrol)
        {
            // Check if patrolling
            if (CheckRoomForPlayer(e, toRoom))
            { return; }
        }

        // If you're a gnome, check that Player doesn't see the room you're going to
        if (e.type == Enemy.Type.gnome && DoesPlayerSeeEnemy(toRoom, true))
        {
            //Debug.Log("Gnome MOVING TO PLAYER ROOM PREVENTED");
            return;
        }

        // If not, proceed to Move enemy
        toRoom.monster = e;
        fromRoom.monster = null;

        e.orientation = CalculateOrientation(fromRoom, toRoom);

        e.currentRoom = toRoom;
        e.lastRoom = fromRoom;

        //PrintEnemyStatus(e, fromRoom, toRoom);
    }

    private Orientation CalculateOrientation(Room previous, Room next)
    {
        if (previous.north.neighbour != null && previous.north.neighbour == next) { return Orientation.north; }
        else if (previous.west.neighbour != null && previous.west.neighbour == next) { return Orientation.west; }
        else if (previous.east.neighbour != null && previous.east.neighbour == next) { return Orientation.east; }
        else if (previous.south.neighbour != null && previous.south.neighbour == next) { return Orientation.south; }
        // Default
        return Orientation.north;
    }

    private void PrintEnemyStatus(Enemy e, Room fromRoom, Room toRoom)
    {
        string data = "";

        data += "From: " + fromRoom;
        data += "|| To: " + toRoom;
        data += "|| Mode: " + e.mode;

        Debug.Log(data);
    }
    #endregion

    #region Hunting
    private void MoveAfterTarget(Enemy e)
    {
        RoomManager rm = RoomManager.Instance;

        // Check if Player is within same Room
        if (rm.currentRoom == e.currentRoom) { AttackPlayer(e); return; }

        if (IsPlayerWithinSight(e))
        { e.targetRoom = rm.currentRoom; }

        #region Enemy Type Specific behaviours
        // If you're gnome, check that Player doesn't have sight on you
        if (e.type == Enemy.Type.gnome && DoesPlayerSeeEnemy(e.currentRoom))
        {
            return;
        }

        // If you're steam, add chance that you will not move
        if (e.type == Enemy.Type.steam)
        {
            float chance = 0.4f;
            if (Random.Range(0f, 1f) <= chance) { return; }
        }
        #endregion

        // Choose the Room that is closest to Player
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


        // Either TURN or MOVE towards that room

        // If facing already to correct orientation, move

        // If not, turn to face that orientation

        if (CanEnemyMove(e, results[0].Item1))
        {
            // Move
            MoveToRoom(e, e.currentRoom, neighbour);

            if (rm.currentRoom == e.currentRoom) { AttackPlayer(e); return; }
        }
        else
        {
            // Turn
            e.orientation = results[0].Item1;
        }

    }

    private bool CanEnemyMove(Enemy e, Orientation ori)
    {
        // If they're Steam, they get a free pass
        if (e.type == Enemy.Type.steam) { return true; }

        // Otherwise, their orientation must match the way they're supposed to be going
        if (e.orientation == ori)
        {
            return true;
        }
        else { return false; }
    }

    private bool DoesPlayerSeeEnemy(Room enemyRoom, bool overrideTurning = false)
    {
        RoomExplorer re = RoomExplorer.Instance;

        // Get Player orientation
        Orientation currentOrientation = re.currentOrientation;
        Orientation previousOrientation = re.previousOrientation;
        bool wasLastMoveTurn = re.lastMoveWasTurn;
        Debug.Log("LastMoveWasTurn: " + wasLastMoveTurn);

        bool seenByPlayer = false;

        // Get X amount of rooms in that direction
        int playerSeeDistance = 4;
        if (RoomManager.Instance.IsOtherRoomInDirection(RoomManager.Instance.currentRoom, enemyRoom, currentOrientation, playerSeeDistance))
        {
            // If Gnome is in any of those, Player sees them

            if (!wasLastMoveTurn || overrideTurning)
            {
                seenByPlayer = true;
            }
        }
        else
        {
            // If not, check if the Gnome is in the previous orientation IN THE NEXT ROOM
            if (RoomManager.Instance.IsOtherRoomInDirection(RoomManager.Instance.currentRoom, enemyRoom, previousOrientation, 1))
            {
                seenByPlayer = true;
            }
        }

        return seenByPlayer;
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
        //Debug.Log("Going AFTER the Player");

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
        if (e.mode == Enemy.Mode.hunt)
        {
            //Debug.Log(e.type + " is calmed...");
            e.mode = Enemy.Mode.patrol;
        }
    }

    private void AttackPlayer(Enemy e)
    {
        Debug.Log(e.type + " attacks Player!");

        if (e.type == Enemy.Type.steam)
        {
            // Dehydrate
            Drinker.Instance.ReduceHydration(true);
        }
        else
        {
            // Disable Player Controls
            FindObjectOfType<PlayerInput>().DisablePlayerControls();

            // Trigger the "gnome is behind you" animation
            CameraHandler.Instance.DoGnomeDeathAnimation(e.orientation);

            // Kill the Player
        }
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
        int teleportDistance = 6;
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
