using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Properties
    static public RoomManager Instance;

    public Room currentRoom;

    [HideInInspector]
    public Vector3 desiredLocation = Vector3.zero;

    public List<Room> roomsList = new List<Room>();

    public int roomsCount { get { return roomsList.Count; } }

    // Manifestation
    RoomHusk huskCenter;

    RoomHusk[,] huskGrid = new RoomHusk[7, 7];

    // String paths
    string deadEnd = "Room 1 DeadEnd";
    string corner = "Room 2 Corner";
    string straight = "Room 2 Straight";
    string threeway = "Room 3 Threeway";
    string fourway = "Room 4 Fourway";

    string mockupModifier = " Mockup";

    [SerializeField]
    bool debugOn = false;
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
        huskGrid[3, 3] = huskCenter;
        ManifestRoom(roomsList[0], huskGrid[3, 3]);
        //ManifestRoom(roomsList[0], huskCenter);
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
            case Orientation.west:
                return 90f;
            case Orientation.east:
                return -90f;
            case Orientation.south:
                return 0f;
            default:
                return 0f;
        }
    }
    #endregion

    #region Room Moving

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
        if (!debugOn) { return; }

        string log = "Room: " + r.name + " || ";

        //log += " N: " + (r.north.neighbour == null ? "null" : " " + r.north.neighbour.name);
        //log += " W: " + (r.west.neighbour == null ? "null" : " " + r.west.neighbour.name);
        //log += " E: " + (r.east.neighbour == null ? "null" : " " + r.east.neighbour.name);
        //log += " S: " + (r.south.neighbour == null ? "null" : " " + r.south.neighbour.name);

        //log += " || " + r.type + " || " + r.orientation;

        log += " || Depth: " + r.depth;

        //log += " || Items: " + r.hasItems;

        Debug.Log(log);
    }
    #endregion

    #region New Husk Generation
    public void LoadInitialRooms()
    {
        var firstHusk = Instantiate(Resources.Load("RoomHusk"), transform) as GameObject;
        huskCenter = firstHusk.GetComponent<RoomHusk>();

        currentRoom = roomsList[0];

        // Fill up the rest of the husks
        currentRoom = roomsList[0];
        FillUpRooms(currentRoom);

        PrintHuskGrid();
    }

    private void FillUpRooms(Room cr)
    {
        // CENTER
        CreateHuskAt(2, 2, cr); // Center will ALWAYS have a husk.


        // NORTH
        //if (cr.north.neighbour != null)
        //{
        //    // Create a husk there
        //    CreateHuskAt(2, 3, cr.north.neighbour);
        //    if (cr.north.neighbour.north.neighbour != null)
        //    {
        //        CreateHuskAt(2, 4, cr.north.neighbour.north.neighbour);

        //        if (cr.north.neighbour.north.neighbour.north.neighbour != null)
        //        { CreateHuskAt(2, 5, cr.north.neighbour.north.neighbour.north.neighbour); }
        //    }
        //}
        Orientation ori = Orientation.north;
        if (TestForNeighbourAt(cr, ori, 1))
        {
            CreateHuskAt(2, 3, GetNeighbourAt(cr, ori, 1));

            if (TestForNeighbourAt(cr, ori, 2))
            {
                CreateHuskAt(2, 4, GetNeighbourAt(cr, ori, 2));

                if (TestForNeighbourAt(cr, ori, 3)) { CreateHuskAt(2, 5, GetNeighbourAt(cr, ori, 3)); }
            }
        }

        // WEST
        //if (cr.west.neighbour != null && husks[1, 2] == null)
        if (cr.west.neighbour != null)
        {
            CreateHuskAt(1, 2, cr.west.neighbour);

            //if (cr.west.neighbour.west.neighbour != null && husks[0, 2] == null)
            if (cr.west.neighbour.west.neighbour != null)
            {
                CreateHuskAt(0, 2, cr.west.neighbour.west.neighbour);
                if (cr.west.neighbour.west.neighbour.west.neighbour != null)
                { CreateHuskAt(-1, 2, cr.west.neighbour.west.neighbour.west.neighbour); }
            }
        }

        // EAST
        //if (cr.east.neighbour != null && husks[3, 2] == null)
        if (cr.east.neighbour != null)
        {
            CreateHuskAt(3, 2, cr.east.neighbour);
            //if (cr.east.neighbour.east.neighbour != null && husks[4, 2] == null)
            if (cr.east.neighbour.east.neighbour != null)
            {
                CreateHuskAt(4, 2, cr.east.neighbour.east.neighbour);
                if (cr.east.neighbour.east.neighbour.east.neighbour != null)
                { CreateHuskAt(5, 2, cr.east.neighbour.east.neighbour.east.neighbour); }
            }
        }

        // SOUTH
        //if (cr.south.neighbour != null && husks[2, 1] == null)
        if (cr.south.neighbour != null)
        {
            CreateHuskAt(2, 1, cr.south.neighbour);
            //if (cr.south.neighbour.south.neighbour != null && husks[2, 0] == null)
            if (cr.south.neighbour.south.neighbour != null)
            {
                CreateHuskAt(2, 0, cr.south.neighbour.south.neighbour);
                if (cr.south.neighbour.south.neighbour.south.neighbour != null)
                { CreateHuskAt(2, -1, cr.south.neighbour.south.neighbour.south.neighbour); }
            }
        }
    }

    private void CreateHuskAt(int x, int y, Room room)
    {
        // First check if there is a need for new husk there
        int gridX = x + 1; int gridY = y + 1;
        //Debug.Log("Testing huskGrid at: " + gridX + ", " + gridY);

        float X = HuskToRoomPosition(x) + desiredLocation.x;
        float Y = HuskToRoomPosition(y) + desiredLocation.z;

        if (huskGrid[gridX, gridY] != null)
        {
            //Debug.Log("No Husk at: " + X + ", " + Y);
            // There is already a husk, no need for another one!
            return;
        }


        var newHusk = Instantiate(Resources.Load("RoomHusk")) as GameObject;
        RoomHusk husk = newHusk.GetComponent<RoomHusk>();



        husk.Position(X, Y);
        newHusk.transform.parent = transform;
        husk.Name("Husk " + X + "," + Y);

        // Add to Husks list too
        huskGrid[gridX, gridY] = husk;

        ManifestRoom(room, husk);

        // Manifest Items too
        ItemManager.Instance.ManifestRoomItems(room, new Vector3(X, 0f, Y));
    }


    private float HuskToRoomPosition(int value)
    {
        float calcValue = (value - 2);

        return calcValue * WorldStats.Instance.Scale;

    }

    private void ShiftHuskGrid(int xChange, int yChange)
    {
        //bool xPositive = (xChange >= 0) ? true : false;
        //bool yPositive = (yChange >= 0) ? true : false;
        bool positiveChange;
        positiveChange = (xChange > 0 || yChange > 0) ? true : false;

        int length = 6;
        switch (positiveChange)
        {
            case (false):
                //Shift the entire Grid towards new desired coordinates
                for (int X = 0; X < length; X++)
                {
                    for (int Y = 0; Y < length; Y++)
                    {
                        ShiftFromTo(X, Y, X + xChange, Y + yChange);
                    }
                }

                break;
            case (true):
                for (int X = length; X > 0; X--)
                {
                    for (int Y = length; Y > 0; Y--)
                    {
                        ShiftFromTo(X, Y, X + xChange, Y + yChange);
                    }
                }
                break;
        }
    }

    private void ShiftFromTo(int xFrom, int yFrom, int xTo, int yTo)
    {
        if (huskGrid[xFrom, yFrom] == null) { return; }

        // Destroy if it will not be placed at the Central Cross
        if (!(xTo != 3 || yTo != 3))
            //if ((xTo != 3 && yTo == 3))
        {
            // If either one is not three, destroy the husk
            Destroy(huskGrid[xFrom, yFrom].gameObject);
        }

        RoomHusk shifted = huskGrid[xFrom, yFrom];

        if (xTo < 0 || xTo > 6 || yTo < 0 || yTo > 6)
        {
            // Destroy the husk
            Destroy(huskGrid[xFrom, yFrom].gameObject);
            return;
        }

        // Move there
        huskGrid[xTo, yTo] = shifted;
    }

    private void PrintHuskGrid()
    {
        string log = "";

        int length = 7;
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                log += XO(x, y);
            }
            log += "\n";
        }

        Debug.Log(log);
    }

    private string XO(int x, int y)
    {
        string s = "[" + x + "," + y + "]";
        if (huskGrid[x, y] == null) { return "O "; }
        else { return "X "; }
    }
    #endregion

    #region New Room Movement
    public bool TryChangeRoom2(Vector3 moveVector)
    {
        if (currentRoom == null) { Debug.Log("Current Room is null"); }

        if (moveVector == Vector3.forward)
        {
            // Try go north
            if (currentRoom.north.neighbour != null)
            {
                //MoveAllRooms(Direction.north, currentRoom.north.neighbour);
                UpdateCurrentDesiredPosition(Vector3.forward);
                MoveToRoom(currentRoom.north.neighbour, Vector2.up);
                return true;
            }
            else { return false; }
        }
        else if (moveVector == Vector3.left)
        {
            if (currentRoom.west.neighbour != null)
            {
                //MoveAllRooms(Direction.west, currentRoom.west.neighbour);
                UpdateCurrentDesiredPosition(Vector3.left);
                MoveToRoom(currentRoom.west.neighbour, Vector2.left);

                return true;
            }
            else { return false; }
        }
        else if (moveVector == Vector3.right)
        {
            if (currentRoom.east.neighbour != null)
            {
                //MoveAllRooms(Direction.east, currentRoom.east.neighbour);
                UpdateCurrentDesiredPosition(Vector3.right);
                MoveToRoom(currentRoom.east.neighbour, Vector2.right);
                return true;
            }
            else { return false; }
        }
        else if (moveVector == Vector3.back)
        {
            if (currentRoom.south.neighbour != null)
            {
                //MoveAllRooms(Direction.south, currentRoom.south.neighbour);
                UpdateCurrentDesiredPosition(Vector3.back);
                MoveToRoom(currentRoom.south.neighbour, Vector2.down);
                return true;
            }
            else { return false; }
        }
        else
        {
            Debug.Log("Bonk! There is a wall in " + moveVector.ToString() + " direction");
            return false;
        }


        //    //DebugLogRoom(currentRoom);
    }

    private void UpdateCurrentDesiredPosition(Vector3 moveDir)
    {
        desiredLocation += moveDir * WorldStats.Instance.Scale;
    }

    private void MoveToRoom(Room newRoom, Vector2 dir)
    {
        // Update current room etc.
        currentRoom = newRoom;

        ShiftHuskGrid((int)dir.x, -(int)dir.y);

        // Kill all children THAT ARE NO LONGER NEEDED
        //foreach (Transform child in transform)
        //{
        //    Destroy(child.gameObject);
        //}
        DestroyManifestationsNotNeeded();

        // Demanifest all items
        ItemManager.Instance.ClearManifestations();

        // Setup new husks
        FillUpRooms(currentRoom);

        PrintHuskGrid();


        DebugLogRoom(currentRoom);
    }

    private void DestroyManifestationsNotNeeded()
    {
        // Destroy all husks that are on the middle cross section
        int length = 7;
        int middle = 3;

        int spared = 0;
        int destroyed = 0;
        for (int X = 0; X < length; X++)
        {
            for (int Y = 0; Y < length; Y++)
            {
                // Don't destroy the ones whose X OR Y values is 3

                // Otherwise, destroy it
                if (huskGrid[X, Y] != null)
                {
                    if (X == middle || Y == middle)
                    {
                        spared++;
                        continue;
                    }

                    Destroy(huskGrid[X, Y].gameObject);
                    destroyed++;
                }
            }
        }
        Debug.Log("Destroyed " + destroyed + ", Spared " + spared);
    }

    private Room GetNeighbourAt(Room original, Orientation orientation, int howFar)
    {
        Room temp = original;

        bool succesfull = true;

        // Keep going through the neighbours as far as needed
        for (int i = 0; i < howFar; i++)
        {

            switch (orientation)
            {
                case (Orientation.north):
                    if (temp.north.neighbour != null)
                    { temp = temp.north.neighbour; }
                    else { succesfull = false; }
                    break;
                case (Orientation.west):
                    if (temp.west.neighbour != null)
                    { temp = temp.west.neighbour; }
                    else { succesfull = false; }
                    break;
                case (Orientation.east):
                    if (temp.east.neighbour != null)
                    { temp = temp.east.neighbour; }
                    else { succesfull = false; }
                    break;
                case (Orientation.south):
                    if (temp.south.neighbour != null)
                    { temp = temp.south.neighbour; }
                    else { succesfull = false; }
                    break;
            }
            if (succesfull == false) { break; }

        }

        // Return the desired neighbour or null, if that neighbour wasn't available
        if (succesfull)
        {
            return temp;
        }
        else
        {
            return null;
        }
    }

    private bool TestForNeighbourAt(Room original, Orientation orientation, int howFar)
    {
        Room temp = original;

        bool succesfull = true;

        // Keep going through the neighbours as far as needed
        for (int i = 0; i < howFar; i++)
        {

            switch (orientation)
            {
                case (Orientation.north):
                    if (temp.north.neighbour != null)
                    { temp = temp.north.neighbour; }
                    else { succesfull = false; }
                    break;
                case (Orientation.west):
                    if (temp.west.neighbour != null)
                    { temp = temp.west.neighbour; }
                    else { succesfull = false; }
                    break;
                case (Orientation.east):
                    if (temp.east.neighbour != null)
                    { temp = temp.east.neighbour; }
                    else { succesfull = false; }
                    break;
                case (Orientation.south):
                    if (temp.south.neighbour != null)
                    { temp = temp.south.neighbour; }
                    else { succesfull = false; }
                    break;
            }
            if (succesfull == false) { break; }

        }

        // Return the desired neighbour or null, if that neighbour wasn't available
        if (succesfull)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
