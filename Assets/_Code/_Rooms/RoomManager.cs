using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Properties
    static public RoomManager Instance;

    public Room currentRoom;

    public List<Room> roomsList = new List<Room>();

    public int roomsCount { get { return roomsList.Count; } }

    // Manifestation
    RoomHusk huskCenter;
    RoomHusk huskNorth;
    RoomHusk huskWest;
    RoomHusk huskEast;
    RoomHusk huskSouth;


    // String paths
    string deadEnd = "Room 1 DeadEnd";
    string corner = "Room 2 Corner";
    string straight = "Room 2 Straight";
    string threeway = "Room 3 Threeway";
    string fourway = "Room 4 Fourway";

    string mockupModifier = " Mockup";
    #endregion

    #region Setup
    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetupRoomHusks();
    }

    private void SetupRoomHusks()
    {
        huskCenter = FindObjectOfType<RoomHusk>();

        string path = "RoomHusk";
        var North = Instantiate(Resources.Load(path), transform) as GameObject;
        huskNorth = North.GetComponent<RoomHusk>();
        var West = Instantiate(Resources.Load(path), transform) as GameObject;
        huskWest = West.GetComponent<RoomHusk>();
        var East = Instantiate(Resources.Load(path), transform) as GameObject;
        huskEast = East.GetComponent<RoomHusk>();
        var South = Instantiate(Resources.Load(path), transform) as GameObject;
        huskSouth = South.GetComponent<RoomHusk>();

        // Position them correctly
        huskNorth.Position(0f, WorldStats.Instance.Z);
        huskWest.Position(-WorldStats.Instance.X, 0f);
        huskEast.Position(WorldStats.Instance.X, 0f);
        huskSouth.Position(0f, -WorldStats.Instance.Z);

        // Rename
        huskCenter.Name("Center");
        huskNorth.Name("North");
        huskWest.Name("West");
        huskEast.Name("East");
        huskSouth.Name("South");
    }
    #endregion

    #region Rooms List Management
    public void AddRoomToList(Room room)
    {
        roomsList.Add(room);
    }
    #endregion

    #region Manifestation
    public void InitiateFirstRoom()
    {
        ManifestRoom(roomsList[0], huskCenter);
        ChangeAdjacentRooms(roomsList[0]);
    }

    public void ManifestRoom(Room room, RoomHusk husk)
    {
        PickRoomGraphics(room, husk);
    }

    private void PickRoomGraphics(Room r, RoomHusk husk)
    {
        float angle = AngleRoomToOrientation(r.orientation);

        string path = "";

        // Get correct graphics
        switch (r.type)
        {
            case TypeRoom._1_deadEnd:
                path = deadEnd;
                    break;
            case TypeRoom._2_corner:
                path = corner;
                    break;
            case TypeRoom._2_straight:
                path = straight;
                    break;
            case TypeRoom._3_threeway:
                path = threeway;
                    break;
            case TypeRoom._4_fourway:
                path = fourway;
                    break;
        }

        // Instantiate graphics
        GameObject graphics = Instantiate(Resources.Load(path + mockupModifier), husk.transform) as GameObject;
        if (husk == null) { Debug.LogError("No husk found"); }
        if (graphics == null) { Debug.LogError("No graphics found"); }
        husk.ChangeHuskGraphics(graphics);

        // Rotate them according to the angle
        Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
        husk.RotateGraphics(rotation);
    }

    private float AngleRoomToOrientation(Orientation orientation)
    {
        switch (orientation)
        {
            case Orientation.north:
                return 180f;
                break;
            case Orientation.west:
                return 90f;
                break;
            case Orientation.east:
                return -90f;
                break;
            case Orientation.south:
                return 0f;
                break;
            default:
                return 0f;
        }
    }
    #endregion

    #region Room Moving
    public void TryChangeRoom(Vector3 moveVector)
    {
        if (currentRoom == null) { Debug.Log("Current Room is null"); }

        
        if (moveVector == Vector3.forward)
        {
            // Try go north
            if (currentRoom.north.neighbour != null)
            {
                // Change room
                currentRoom = currentRoom.north.neighbour;
                ManifestRoom(currentRoom, huskCenter);
                ChangeAdjacentRooms(currentRoom);
            }
        }
        else if (moveVector == Vector3.left)
        {
            if (currentRoom.west.neighbour != null) { currentRoom = currentRoom.west.neighbour; ManifestRoom(currentRoom, huskCenter); ChangeAdjacentRooms(currentRoom); }
        }
        else if (moveVector == Vector3.right)
        {
            if (currentRoom.east.neighbour != null) { currentRoom = currentRoom.east.neighbour; ManifestRoom(currentRoom, huskCenter); ChangeAdjacentRooms(currentRoom); }
        }
        else if (moveVector == Vector3.back)
        {
            if (currentRoom.south.neighbour != null) { currentRoom = currentRoom.south.neighbour; ManifestRoom(currentRoom, huskCenter); ChangeAdjacentRooms(currentRoom); }
        }
        else
        {
            Debug.Log("Bonk! There is a wall in " + moveVector.ToString() + " direction");
        }

        DebugLogRoom(currentRoom);
    }

    private void ChangeAdjacentRooms(Room room)
    {
        // Check all adjacent rooms

        // NORTH
        if (room.north.neighbour != null)
        {
            // Change North Husk
            ManifestRoom(room.north.neighbour, huskNorth);
        }
        else
        {
            // Hide the graphics of that Husk
            huskNorth.HideHuskGraphics();
        }
        if (room.west.neighbour != null) { ManifestRoom(room.west.neighbour, huskWest); } else { huskWest.HideHuskGraphics(); }
        if (room.east.neighbour != null) { ManifestRoom(room.east.neighbour, huskEast); } else { huskEast.HideHuskGraphics(); }
        if (room.south.neighbour != null) { ManifestRoom(room.south.neighbour, huskSouth); } else { huskSouth.HideHuskGraphics(); }
    }

    public void DebugAllRooms()
    {
        foreach (Room r in roomsList)
        {
            // Go through all neighbours
            DebugLogRoom(r);
        }
    }

    private void DebugLogRoom(Room r)
    {
        string log = "Room: " + r.name + " || ";

        log += " N: " + (r.north.neighbour == null ? "null" : " " + r.north.neighbour.name);
        log += " W: " + (r.west.neighbour == null ? "null" : " " + r.west.neighbour.name);
        log += " E: " + (r.east.neighbour == null ? "null" : " " + r.east.neighbour.name);
        log += " S: " + (r.south.neighbour == null ? "null" : " " + r.south.neighbour.name);

        log += " || " + r.type + " || " + r.orientation;

        Debug.Log(log);
    }
    #endregion
}
