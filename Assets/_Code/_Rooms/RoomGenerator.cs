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


    

    

    enum DirectionOfConnection
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
        GenerateFirstRoom(DirectionOfConnection.north);

        // Manifest it
        RoomManager.Instance.InitiateFirstRoom();

        // Keep generating rooms until you have generated enough
        RoomGenerationLoop();
    }

    private void RoomGenerationLoop()
    {
        while (createdAmountOfRooms < amountOfRooms)
        {
            if (roomsWithUnfinishedConnections.Count == 0) { break; }

            // Pick an unfinished connection room
            Room nextToConnect = roomsWithUnfinishedConnections[0];

            // Create rooms for each unconnected connection
            CheckDirectionsAndGenerateIfNecessary(nextToConnect);
        }

        // Fill up the rest of  unfinished connections with dead ends


        Debug.Log("Rooms amount: " + RoomManager.Instance.roomsCount);

        RoomManager.Instance.DebugAllRooms();
    }

    private void CheckDirectionsAndGenerateIfNecessary(Room r)
    {
        // Check each connection and connect if needed
        if (CheckConnection(r.north))
        {
            // Create a room there
            GenerateNewRoom(DirectionOfConnection.south, r);
        }
        if (CheckConnection(r.west))
        {
            GenerateNewRoom(DirectionOfConnection.east, r);
        }
        if (CheckConnection(r.east))
        {
            GenerateNewRoom(DirectionOfConnection.west, r);
        }
        if (CheckConnection(r.south))
        {
            GenerateNewRoom(DirectionOfConnection.north, r);
        }

        // Remove from unifinished rooms
        roomsWithUnfinishedConnections.Remove(r);
    }
    #endregion

    #region Generation

    private void GenerateFirstRoom(DirectionOfConnection connectionDirectionFrom)
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

    private void GenerateNewRoom(DirectionOfConnection connectionFrom, Room fromRoom)
    {
        // Make new Room
        Room room = new Room();

        // Room type
        room.type = DecideRoomType(true);

        // Orientation
        room.orientation = DecideOrientation(room.type, connectionFrom);

        // Setup walls for the Room
        SetupRoomWalls(room, room.type, room.orientation);

        // Add to the RoomsList
        RoomManager.Instance.AddRoomToList(room);

        createdAmountOfRooms++;

        // Add unfinished connections
        //CheckForUnfinishedConnections(room);

        // Connect up to the fromRoom
        ConnectToPrevious(fromRoom, room, connectionFrom);

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

    private Orientation DecideOrientation(TypeRoom type, DirectionOfConnection connectFrom)
    {
        return Orientation.north;
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

    private void ConnectToPrevious(Room previous, Room thisOne, DirectionOfConnection dir)
    {
        switch (dir)
        {
            case DirectionOfConnection.north: previous.south.neighbour = thisOne; break;
            case DirectionOfConnection.west: previous.east.neighbour = thisOne; break;
            case DirectionOfConnection.east: previous.west.neighbour = thisOne; break;
            case DirectionOfConnection.south: previous.north.neighbour = thisOne; break;
        }
    }
    #endregion
}


