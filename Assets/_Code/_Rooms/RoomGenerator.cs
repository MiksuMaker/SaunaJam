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

    [SerializeField]
    Transform roomParent;

    List<Connection> unfinishedConnections = new List<Connection>();


    // String paths
    string deadEnd = "Room 1 DeadEnd";
    string corner = "Room 2 Corner";
    string straight = "Room 2 Straight";
    string threeway = "Room 3 Threeway";
    string fourway = "Room 4 Fourway";

    string mockupModifier = " Mockup";

    

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
        GenerateRoom(DirectionOfConnection.north);

        // Manifest it
        RoomManager.Instance.InitiateFirstRoom();

        // Keep generating rooms until you have generated enough

    }
    #endregion

    #region Generation
    private void GenerateOneRoom(DirectionOfConnection connectFrom)
    {
        // Load up one Room Prefab
        GameObject roomGO = Instantiate(Resources.Load("Room"), roomParent) as GameObject;
        RoomHusk room = roomGO.AddComponent<RoomHusk>();

        // Decide what Type of Room it will be
        TypeRoom type = DecideRoomType();

        // Decide orientation
        Orientation orientation = DecideOrientation(type, connectFrom); 

        // Give correct graphics to it
        GiveCorrectGraphics(roomGO, type);

        // Give reference to RoomManager

    }

    private void GenerateRoom(DirectionOfConnection connectionFrom)
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

    private void GiveCorrectGraphics(GameObject room, TypeRoom type)
    {

    }
    #endregion
}


