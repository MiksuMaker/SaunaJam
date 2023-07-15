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

        // Keep generating rooms until you have generated enough
        RoomGenerationLoop();

        // Manifest it
        RoomManager.Instance.InitiateFirstRoom();
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

        Debug.Log("Rooms amount: " + RoomManager.Instance.roomsCount);


        // Fill up the rest of  unfinished connections with dead ends
        FillDeadEnds();
    }

    private void CheckDirectionsAndGenerateIfNecessary(Room r, bool forceDeadEnd = false)
    {
        // Check each connection and connect if needed
        if (CheckConnection(r.north))
        {
            // Create a room there
            GenerateNewRoom(FromConnection.south, r, forceDeadEnd);
        }
        if (CheckConnection(r.west))
        {
            GenerateNewRoom(FromConnection.east, r, forceDeadEnd);
        }
        if (CheckConnection(r.east))
        {
            GenerateNewRoom(FromConnection.west, r, forceDeadEnd);
        }
        if (CheckConnection(r.south))
        {
            GenerateNewRoom(FromConnection.north, r, forceDeadEnd);
        }

        // Remove from unifinished rooms
        roomsWithUnfinishedConnections.Remove(r);
    }

    private void FillDeadEnds()
    {
        // Go through all the rooms, check which ones don't have finished connections
        int roomCount = RoomManager.Instance.roomsCount;

        for (int i = 0; i < roomCount; i++)
        {
            CheckForUnfinishedConnections(RoomManager.Instance.roomsList[i]);
        }

        // Now that unfinished rooms have been added, go through them, adding dead ends
        int stubCount = roomsWithUnfinishedConnections.Count;
        for (int i = 0; i < stubCount; i++)
        {
            CheckDirectionsAndGenerateIfNecessary(roomsWithUnfinishedConnections[0], true);
        }
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

    private void GenerateNewRoom(FromConnection connectionFrom, Room fromRoom, bool forceDeadEnd = false)
    {
        // Make new Room
        Room room = new Room();

        // Room type
        if (!forceDeadEnd) { room.type = DecideRoomType(); } else { room.type = TypeRoom._1_deadEnd; }

        // Orientation
        room.orientation = DecideOrientation(room.type, connectionFrom);

        // Setup walls for the Room
        SetupRoomWalls(room, room.type, room.orientation);

        // Add to the RoomsList
        RoomManager.Instance.AddRoomToList(room);

        createdAmountOfRooms++;

        // Add to unfinished connections
        if (room.type != TypeRoom._1_deadEnd)
        { roomsWithUnfinishedConnections.Add(room); }

        // Connect up to the fromRoom
        ConnectRooms(fromRoom, room, connectionFrom);

    }

    private TypeRoom DecideRoomType(bool isStart = false)
    {
        int maxNum = 5;
        int randNum = Random.Range(0, maxNum);
        if (isStart) { randNum = Random.Range(1, maxNum); }


        TypeRoom t = TypeRoom._1_deadEnd;
        if (randNum == 0) { t = TypeRoom._1_deadEnd; }
        else if (randNum == 1) { t = TypeRoom._2_corner; }
        else if (randNum == 2) { t = TypeRoom._2_straight; }

        else if (randNum == 3) { t = TypeRoom._3_threeway; }
        else if (randNum == 4) { t = TypeRoom._4_fourway; }

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
            case (TypeRoom._2_straight, FromConnection.east): return Orientation.west;
            case (TypeRoom._2_straight, FromConnection.south): return Orientation.north;

            // CORNERS : Decide a direction
            case (TypeRoom._2_corner, FromConnection.north):
                return GetRandomValidOrientation(new Orientation[2]
                                                   { Orientation.north,
                                                     Orientation.west});
            case (TypeRoom._2_corner, FromConnection.west):
                return GetRandomValidOrientation(new Orientation[2]
                                                    { Orientation.south,
                                                      Orientation.west});
            case (TypeRoom._2_corner, FromConnection.east):
                return GetRandomValidOrientation(new Orientation[2]
                                                    { Orientation.north,
                                                      Orientation.east});
            case (TypeRoom._2_corner, FromConnection.south):
                return GetRandomValidOrientation(new Orientation[2]
                                                   { Orientation.south,
                                                     Orientation.east});

            // THREEWAY : Decide a direction
            case (TypeRoom._3_threeway, FromConnection.north):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.north,
                                                     Orientation.west,
                                                     Orientation.south});
            case (TypeRoom._3_threeway, FromConnection.west):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.east,
                                                     Orientation.west,
                                                     Orientation.south});
            case (TypeRoom._3_threeway, FromConnection.east):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.north,
                                                     Orientation.west,
                                                     Orientation.east});
            case (TypeRoom._3_threeway, FromConnection.south):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.north,
                                                     Orientation.east,
                                                     Orientation.south});

            // FOURWAY : Doesn't matter, but randomize still
            case (TypeRoom._4_fourway, _):
                return GetRandomValidOrientation(new Orientation[4]
                                                    { Orientation.north,
                                                      Orientation.west,
                                                      Orientation.east,
                                                      Orientation.south});

            // DEFAULT
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

            // THREEWAYS
            case (TypeRoom._3_threeway, Orientation.north):
                w = true;
                break;
            case (TypeRoom._3_threeway, Orientation.west):
                s = true;
                break;
            case (TypeRoom._3_threeway, Orientation.east):
                n = true;
                break;
            case (TypeRoom._3_threeway, Orientation.south):
                e = true;
                break;

            // FOURWAYS
            case (TypeRoom._4_fourway, _):
                // No walls!
                break;

            // DEFAULT
            default: Debug.LogWarning("Missed a roomtype!"); break;
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


