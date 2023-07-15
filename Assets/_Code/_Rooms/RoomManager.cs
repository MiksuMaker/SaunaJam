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
    //List<RoomHusk> husks = new List<RoomHusk>();
    RoomHusk[,] husks = new RoomHusk[5, 5];
    RoomHusk huskCenter;

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

        //SetupRoomHusks2();
    }

    private void SetupRoomHusks()
    {
        //huskCenter = FindObjectOfType<RoomHusk>();

        //string path = "RoomHusk";
        //var North = Instantiate(Resources.Load(path), transform) as GameObject;
        //huskNorth = North.GetComponent<RoomHusk>();
        //var West = Instantiate(Resources.Load(path), transform) as GameObject;
        //huskWest = West.GetComponent<RoomHusk>();
        //var East = Instantiate(Resources.Load(path), transform) as GameObject;
        //huskEast = East.GetComponent<RoomHusk>();
        //var South = Instantiate(Resources.Load(path), transform) as GameObject;
        //huskSouth = South.GetComponent<RoomHusk>();

        //// Position them correctly
        //huskNorth.Position(0f, WorldStats.Instance.Z);
        //huskWest.Position(-WorldStats.Instance.X, 0f);
        //huskEast.Position(WorldStats.Instance.X, 0f);
        //huskSouth.Position(0f, -WorldStats.Instance.Z);

        //// Rename
        //huskCenter.Name("Center");
        //huskNorth.Name("North");
        //huskWest.Name("West");
        //huskEast.Name("East");
        //huskSouth.Name("South");
    }

    private void SetupRoomHusks2()
    {

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
        var firstHusk = Instantiate(Resources.Load("RoomHusk"), transform) as GameObject;
        huskCenter = firstHusk.GetComponent<RoomHusk>();
        ManifestRoom(roomsList[0], huskCenter);
        //ChangeAdjacentRooms(roomsList[0]);
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
        //if (currentRoom == null) { Debug.Log("Current Room is null"); }

        //if (moveVector == Vector3.forward)
        //{
        //    // Try go north
        //    if (currentRoom.north.neighbour != null)
        //    {
        //        currentRoom = currentRoom.north.neighbour;
        //        ManifestRoom(currentRoom, huskCenter, Direction.north);
        //        ChangeAdjacentRooms(currentRoom);
        //    }
        //}
        //else if (moveVector == Vector3.left)
        //{
        //    if (currentRoom.west.neighbour != null)
        //    {
        //        currentRoom = currentRoom.west.neighbour;
        //        ManifestRoom(currentRoom, huskCenter, Direction.west);


        //        ChangeAdjacentRooms(currentRoom);
        //    }
        //}
        //else if (moveVector == Vector3.right)
        //{
        //    if (currentRoom.east.neighbour != null)
        //    {
        //        currentRoom = currentRoom.east.neighbour;
        //        ManifestRoom(currentRoom, huskCenter, Direction.east);
        //        ChangeAdjacentRooms(currentRoom);
        //    }
        //}
        //else if (moveVector == Vector3.back)
        //{
        //    if (currentRoom.south.neighbour != null)
        //    {
        //        currentRoom = currentRoom.south.neighbour;
        //        ManifestRoom(currentRoom, huskCenter, Direction.south);
        //        ChangeAdjacentRooms(currentRoom);
        //    }
        //}
        //else
        //{
        //    Debug.Log("Bonk! There is a wall in " + moveVector.ToString() + " direction");
        //}


        //    //DebugLogRoom(currentRoom);
    }

    //private void ChangeAdjacentRooms(Room room)
    //{
    //    // Check all adjacent rooms

    //    // NORTH
    //    if (room.north.neighbour != null)
    //    {
    //        // Change North Husk
    //        ManifestRoom(room.north.neighbour, huskNorth, Direction.north);
    //    }
    //    else
    //    {
    //        // Hide the graphics of that Husk
    //        huskNorth.HideHuskGraphics();
    //    }
    //    if (room.west.neighbour != null) { ManifestRoom(room.west.neighbour, huskWest, Direction.west); } else { huskWest.HideHuskGraphics(); }
    //    if (room.east.neighbour != null) { ManifestRoom(room.east.neighbour, huskEast, Direction.east); } else { huskEast.HideHuskGraphics(); }
    //    if (room.south.neighbour != null) { ManifestRoom(room.south.neighbour, huskSouth, Direction.south); } else { huskSouth.HideHuskGraphics(); }
    //}

    //private void MoveRooms(RoomHusk husk, Direction moveDirection)
    //{
    //    //Debug.Log("Moving rooms!"); 
    //    // Adopt graphics from the direction, if there are any
    //    switch (moveDirection)
    //    {
    //        case Direction.north:
    //            //TryAdoption(huskCenter, huskSouth);
    //            //TryAdoption(huskNorth, huskCenter);

    //            // Try to manifest new room in that direction

    //            // MoveRooms
    //            huskNorth.MoveRoom(Vector3.forward);
    //            huskCenter.MoveRoom(Vector3.zero);

    //            break;

    //        case Direction.west:
    //            //TryAdoption(huskCenter, huskEast);
    //            //TryAdoption(huskWest, huskCenter);

    //            // MoveRooms
    //            huskWest.MoveRoom(Vector3.right);
    //            huskCenter.MoveRoom(Vector3.zero);
    //            break;

    //        case Direction.east:
    //            //TryAdoption(huskCenter, huskWest);
    //            //TryAdoption(huskEast, huskCenter);

    //            // MoveRooms
    //            huskEast.MoveRoom(Vector3.left);
    //            huskCenter.MoveRoom(Vector3.zero);
    //            break;

    //        case Direction.south:
    //            //TryAdoption(huskCenter, huskNorth);
    //            //TryAdoption(huskSouth, huskCenter);

    //            // MoveRooms
    //            huskSouth.MoveRoom(Vector3.back);
    //            huskCenter.MoveRoom(Vector3.zero);
    //            break;
    //    }

    //}

    //private void TryAdoption(RoomHusk oldParent, RoomHusk newAdopter)
    //{
    //    // Check if old Parent is null
    //    if (oldParent.hasGraphics)
    //    {
    //        // Adoption is possible
    //        newAdopter.Adopt(oldParent.graphics);
    //    }
    //}

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

    #region New Husk Generation
    public void LoadInitialRooms()
    {
        var firstHusk = Instantiate(Resources.Load("RoomHusk"), transform) as GameObject;
        huskCenter = firstHusk.GetComponent<RoomHusk>();

        husks[2, 2] = huskCenter;
        currentRoom = roomsList[0];
        ManifestRoom(currentRoom, huskCenter);

        // Fill up the rest of the husks
        currentRoom = roomsList[0];
        FillUpRooms(currentRoom);
    }

    private void FillUpRooms(Room cr, bool exaggarate = false)
    {
        // Center is at 3,3
        // Load up everything in the cross pattern

        // NORTH
        if (cr.north.neighbour != null && husks[2, 3] == null)
        {
            // Create a husk there
            CreateHuskAt(2, 3, cr.north.neighbour);
            if (cr.north.neighbour.north.neighbour != null && husks[2,4] == null)
            {
                CreateHuskAt(2, 4, cr.north.neighbour.north.neighbour);
            }
        }

        // WEST
        if (cr.west.neighbour != null && husks[1, 2] == null)
        {
            CreateHuskAt(1, 2, cr.west.neighbour);

            if (cr.west.neighbour.west.neighbour != null && husks[0, 2] == null)
            {
                CreateHuskAt(0, 2, cr.west.neighbour.west.neighbour);
            }
        }

        // EAST
        if (cr.east.neighbour != null && husks[3, 2] == null)
        {
            CreateHuskAt(3, 2, cr.east.neighbour);
            if (cr.east.neighbour.east.neighbour != null && husks[4, 2] == null)
            {
                CreateHuskAt(4, 2, cr.east.neighbour.east.neighbour);
            }
        }

        // SOUTH
        if (cr.south.neighbour != null && husks[2, 1] == null)
        {
            CreateHuskAt(2, 1, cr.south.neighbour);
            if (cr.south.neighbour.south.neighbour != null && husks[2, 0] == null)
            {
                CreateHuskAt(2, 0, cr.south.neighbour.south.neighbour);
            }
        }
    }

    private void CreateHuskAt(int x, int y, Room room, bool exaggarate = false)
    {
        var newHusk = Instantiate(Resources.Load("RoomHusk")) as GameObject;
        RoomHusk husk = newHusk.GetComponent<RoomHusk>();

        //Debug.Log(x + "," + y);
        husks[x, y] = husk;

        float X = HuskToRoomPosition(x, exaggarate);
        float Y = HuskToRoomPosition(y, exaggarate);

        husk.Position(X, Y);
        newHusk.transform.parent = transform;
        husk.Name("Husk " + X + "," + Y);

        ManifestRoom(room, husk);
    }

    private float HuskToRoomPosition(int value, bool exaggarate = false)
    {
        float calcValue = (value - 2);

        if (exaggarate)
        {
            float sign = Mathf.Sign(calcValue);
            calcValue += sign;
        }

        return calcValue * WorldStats.Instance.Scale;

    }
    #endregion

    #region New Husk Movement
    public void TryChangeRoom2(Vector3 moveVector)
    {
        if (currentRoom == null) { Debug.Log("Current Room is null"); }

        if (moveVector == Vector3.forward)
        {
            // Try go north
            if (currentRoom.north.neighbour != null)
            {
                MoveAllRooms(Direction.north, currentRoom.north.neighbour);
            }
        }
        else if (moveVector == Vector3.left)
        {
            if (currentRoom.west.neighbour != null)
            {
                MoveAllRooms(Direction.west, currentRoom.west.neighbour);

            }
        }
        else if (moveVector == Vector3.right)
        {
            if (currentRoom.east.neighbour != null)
            {
                MoveAllRooms(Direction.east, currentRoom.east.neighbour);

            }
        }
        else if (moveVector == Vector3.back)
        {
            if (currentRoom.south.neighbour != null)
            {
                MoveAllRooms(Direction.south, currentRoom.south.neighbour);

            }
        }
        else
        {
            Debug.Log("Bonk! There is a wall in " + moveVector.ToString() + " direction");
        }


        //    //DebugLogRoom(currentRoom);
    }

    private void MoveAllRooms(Direction toDirection, Room newRoom)
    {
        // Update current room etc.
        currentRoom = newRoom;

        // Move all husks on grid to the Direction
        //AdjustHuskGrid(toDirection);

        // Setup new husks
        //FillUpRooms(currentRoom, true);

        // Move all husks opposite to the "movement" direction


        // Delete unnecessary husks
    }

    private void AdjustHuskGrid(Direction toDirection)
    {

        foreach (RoomHusk h in husks)
        {
            if (h == null) { continue; }

            // Move one to the side, delete the rest
        }

    }

    private void UpdateHusks(Direction toDirection)
    {
        switch (toDirection)
        {
            case (Direction.north):
                // New center husk
                huskCenter = husks[2, 3];

                // Get new Husks

                break;
        }
    }
    #endregion

    public void MoveHuskToDirection(Vector3 toDirection, RoomHusk husk)
    {

    }

    IEnumerator HuskMover(Vector3 toDirection)
    {
        float increment = 0.1f;
        WaitForSeconds wait = new WaitForSeconds(increment);
        float desiredTime = 3f;
        float waitedTime = 0f;

        Vector3 beginPos = transform.position;
        Vector3 wantedPos = beginPos + (toDirection * WorldStats.Instance.Scale);
        Vector3 updatePos;

        while (waitedTime < desiredTime)
        {
            yield return wait;

            waitedTime += increment;

            // Move
            //graphics.transform.position = Vector3.Lerp(fromPos, transform.position, (waitedTime / desiredTime));
            updatePos = Vector3.Lerp(beginPos, wantedPos, (waitedTime / desiredTime));

            foreach (RoomHusk h in husks)
            {

            }
        }

        // Finish moving
        updatePos = wantedPos;
    }

    private Vector3 DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.north: return Vector3.forward;
            case Direction.west: return Vector3.left;
            case Direction.east: return Vector3.right;
            case Direction.south: return Vector3.back;
            default: return Vector3.zero;
        }
    }
}
