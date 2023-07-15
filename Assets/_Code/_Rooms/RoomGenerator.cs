using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeRoom
{
    _1_deadEnd,
    _2_corner, _2_straight,
    _3_threeway,
    _4_fourway,
}

public enum Orientation
{
    north, west, east, south,
}

public class RoomGenerator : MonoBehaviour
{
    #region Properties
    [Header("Amount of Room tiles to create")]
    [SerializeField]
    int amountOfRooms = 4;
    int createdAmountOfRooms = 0;

    [SerializeField]
    Transform roomParent;

    List<Room> roomsWithUnfinishedConnections = new List<Room>();


    

    

    enum FromConnection
    {
        north,
        west, east,
        south,

        start,
    }

    #endregion

    #region Setup
    private void Start()
    {
        // Mockup start
        GenerateRooms();
    }

    public void GenerateRooms()
    {
        // Generate first room
        GenerateFirstRoom(FromConnection.north);

        // Manifest it
        RoomManager.Instance.InitiateFirstRoom();

        // Keep generating rooms until you have generated enough
        RoomGenerationLoop();
    }

    private void RoomGenerationLoop()
    {
        while (createdAmountOfRooms < amountOfRooms)
        {
            if (roomsWithUnfinishedConnections.Count == 0) { Debug.Log("Run out of Unfinished connections!"); break; }

            // Pick an unfinished connection room
            Room nextToConnect = roomsWithUnfinishedConnections[0];

            // Create rooms for each unconnected connection
            CheckDirectionsAndGenerateIfNecessary(nextToConnect);
        }
        if (createdAmountOfRooms >= amountOfRooms) { Debug.Log("Created full amount of rooms"); }

        // Fill up the rest of  unfinished connections with dead ends


        Debug.Log("Rooms amount: " + RoomManager.Instance.roomsCount);

        //RoomManager.Instance.DebugAllRooms();
    }

    private void CheckDirectionsAndGenerateIfNecessary(Room r)
    {
        // Check each connection and connect if needed
        if (CheckConnection(r.north))
        {
            // Create a room there
            GenerateNewRoom(FromConnection.south, r);
        }
        if (CheckConnection(r.west))
        {
            GenerateNewRoom(FromConnection.east, r);
        }
        if (CheckConnection(r.east))
        {
            GenerateNewRoom(FromConnection.west, r);
        }
        if (CheckConnection(r.south))
        {
            GenerateNewRoom(FromConnection.north, r);
        }

        // Remove from unifinished rooms
        roomsWithUnfinishedConnections.Remove(r);
    }
    #endregion

    #region Generation

    private void GenerateFirstRoom(FromConnection connectionDirectionFrom)
    {
        // Make new Room
        Room room = new Room();

        // Room type
        room.type = DecideRoomType(true);

        // Orientation
        room.orientation = DecideOrientation(room.type, connectionDirectionFrom);

        // Setup walls for the Room
        SetupRoomWalls(room, room.type, room.orientation);

        // Add to the RoomsList
        RoomManager.Instance.AddRoomToList(room);
        RoomManager.Instance.currentRoom = room;

        createdAmountOfRooms++;

        roomsWithUnfinishedConnections.Add(room);
    }

    private void GenerateNewRoom(FromConnection connectionFrom, Room fromRoom)
    {
        // Make new Room
        Room room = new Room();

        // Room type
        room.type = DecideRoomType();

        // Orientation
        room.orientation = DecideOrientation(room.type, connectionFrom);

        // Setup walls for the Room
        SetupRoomWalls(room, room.type, room.orientation);

        // Add to the RoomsList
        RoomManager.Instance.AddRoomToList(room);

        createdAmountOfRooms++;

        // Add to unfinished connections
        roomsWithUnfinishedConnections.Add(room);

        // Connect up to the fromRoom
        ConnectRooms(fromRoom, room, connectionFrom);

    }

    private TypeRoom DecideRoomType(bool isStart = false)
    {
        int randNum = Random.Range(0, 3);
        if (isStart) { randNum = Random.Range(1, 3); }


        TypeRoom t = TypeRoom._1_deadEnd;
        if (randNum == 0) { t = TypeRoom._1_deadEnd; }
        else if (randNum == 1) { t = TypeRoom._2_corner; }
        else if (randNum == 2) { t = TypeRoom._2_straight; }

        return t;
    }

    private Orientation DecideOrientation(TypeRoom type, FromConnection connectFrom)
    {

        switch (type, connectFrom)
        {
            // DEAD ENDS : No need for randomization
            case (TypeRoom._1_deadEnd, FromConnection.north): return Orientation.north;
            case (TypeRoom._1_deadEnd, FromConnection.west): return Orientation.west;
            case (TypeRoom._1_deadEnd, FromConnection.east): return Orientation.east;
            case (TypeRoom._1_deadEnd, FromConnection.south): return Orientation.south;

            // STRAIGHTS : No need for randomization
            case (TypeRoom._2_straight, FromConnection.north): return Orientation.north;
            case (TypeRoom._2_straight, FromConnection.west): return Orientation.west;
            case (TypeRoom._2_straight, FromConnection.east): return Orientation.east;
            case (TypeRoom._2_straight, FromConnection.south): return Orientation.south;

            // CORNERS : Decide a direction
            case (TypeRoom._2_corner, FromConnection.north): return GetRandomValidOrientation(new Orientation[2] 
                                                                                                { Orientation.north,
                                                                                                  Orientation.west});
            case (TypeRoom._2_corner, FromConnection.west): return GetRandomValidOrientation(new Orientation[2]
                                                                                                { Orientation.south,
                                                                                                  Orientation.west});
            case (TypeRoom._2_corner, FromConnection.east): return GetRandomValidOrientation(new Orientation[2]
                                                                                                { Orientation.north,
                                                                                                  Orientation.east});
            case (TypeRoom._2_corner, FromConnection.south): return GetRandomValidOrientation(new Orientation[2]
                                                                                                { Orientation.south,
                                                                                                  Orientation.east});

            default: return Orientation.south;
        }
    }

    private Orientation GetRandomValidOrientation(Orientation[] orientations)
    {
        int count = orientations.Length;
        return orientations[Random.Range(0, count)];
    }

    private void SetupRoomWalls(Room room, TypeRoom type, Orientation orientation)
    {
        // Is a direction wall?
        bool n = false;
        bool w = false;
        bool e = false;
        bool s = false;

        #region Switch Case
        switch (type, orientation)
        {
            // DEAD ENDS
            case (TypeRoom._1_deadEnd, Orientation.north):
                w = true; e = true; s = true;
                break;
            case (TypeRoom._1_deadEnd, Orientation.west):
                n = true; e = true; s = true;
                break;
            case (TypeRoom._1_deadEnd, Orientation.east):
                w = true; n = true; s = true;
                break;
            case (TypeRoom._1_deadEnd, Orientation.south):
                w = true; e = true; n = true;
                break;

            // CORNERS
            case (TypeRoom._2_corner, Orientation.north):
                w = true; s = true;
                break;
            case (TypeRoom._2_corner, Orientation.west):
                e = true; s = true;
                break;
            case (TypeRoom._2_corner, Orientation.east):
                w = true; n = true;
                break;
            case (TypeRoom._2_corner, Orientation.south):
                e = true; n = true;
                break;

            // STRAIGHTS
            case (TypeRoom._2_straight, Orientation.north):
                w = true; e = true;
                break;
            case (TypeRoom._2_straight, Orientation.west):
                n = true; s = true;
                break;
        }
        #endregion

        if (room == null) { Debug.Log("Room is null"); }

        // Setup Room walls
        room.north.isWall = n;
        room.west.isWall = w;
        room.east.isWall = e;
        room.south.isWall = s;
    }
    #endregion

    #region Connections
    private void CheckForUnfinishedConnections(Room r)
    {
        // Go through all the connections

        if (CheckConnection(r.north) || CheckConnection(r.west) || CheckConnection(r.east) || CheckConnection(r.south))
        {
            // Add to unfinished connections
            if (!roomsWithUnfinishedConnections.Contains(r))
            {
                roomsWithUnfinishedConnections.Add(r);
            }
        }
        else
        {
            // If no unfinished connections found, remove from list
            if (roomsWithUnfinishedConnections.Contains(r))
            {
                roomsWithUnfinishedConnections.Remove(r);
            }
        }
    }

    private bool CheckConnection(Connection connection)
    {
        // Add those that are NOT a wall and don't have a neighbour yet to the list

        if (!connection.isWall && connection.neighbour == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ConnectRooms(Room previous, Room newOne, FromConnection dir)
    {
        // Previous room connects to the new one, new one connects to the previous one

        switch (dir)
        {
            case FromConnection.north: previous.south.neighbour = newOne; newOne.north.neighbour = previous; break;
            case FromConnection.west: previous.east.neighbour = newOne; newOne.west.neighbour = previous; break;
            case FromConnection.east: previous.west.neighbour = newOne; newOne.east.neighbour = previous; break;
            case FromConnection.south: previous.north.neighbour = newOne; newOne.south.neighbour = previous; break;
        }
    }
    #endregion
}


