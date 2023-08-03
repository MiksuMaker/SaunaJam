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

    // String paths
    string deadEnd = "Room 1 DeadEnd";
    string corner = "Room 2 Corner";
    string straight = "Room 2 Straight";
    string threeway = "Room 3 Threeway";
    string fourway = "Room 4 Fourway";

    string wideModifier = "Wide ";
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

        if (r.attribute == RoomAttribute.wide) { path += wideModifier; }

        // Get correct graphics
        switch (r.type)
        {
            case TypeRoom._1_deadEnd:
                path += deadEnd;
                break;
            case TypeRoom._2_corner:
                path += corner;
                break;
            case TypeRoom._2_straight:
                path += straight;
                break;
            case TypeRoom._3_threeway:
                path += threeway;
                break;
            case TypeRoom._4_fourway:
                path += fourway;
                break;
        }

        //Debug.Log(path);

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

        //Debug.Log(log);
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
    }

    private void FillUpRooms(Room cr)
    {
        // CENTER
        CreateHuskAt(2, 2, cr);

        // NORTH
        if (cr.north.neighbour != null)
        {
            // Create a husk there
            CreateHuskAt(2, 3, cr.north.neighbour);
            if (cr.north.neighbour.north.neighbour != null)
            {
                CreateHuskAt(2, 4, cr.north.neighbour.north.neighbour);

                if (cr.north.neighbour.north.neighbour.north.neighbour != null)
                { CreateHuskAt(2, 5, cr.north.neighbour.north.neighbour.north.neighbour); }
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

    #region Neighbour Testing & Getting
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


    // ===============================

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

    // ===============================

    public bool IsOtherRoomNear(Room original, Room other, int howFar)
    {
        Room temp = original;

        bool succesfull = true;

        Orientation[] orientations = new Orientation[] { Orientation.north, Orientation.west, Orientation.east, Orientation.south };

        // First check if it is the same room
        //if (original == other)
        //{ return true; }

        // Keep going through the neighbours as far as needed
        for (int j = 0; j < orientations.Length; j++)
        {
            // Reset temp
            temp = original;
            // Reset succesfull
            succesfull = true;

            Debug.Log("Checking " + orientations[j]);

            for (int i = 1; i < howFar + 1; i++)
            {

                switch (orientations[j])
                {
                    case (Orientation.north):
                        if (temp.north.neighbour != null)
                        {
                            // Test if it is the room
                            if (other == temp.north.neighbour) { return Success(i); } // <-- Is in sight
                            temp = temp.north.neighbour;    // Otherwise, continue the operation
                        }
                        else
                        { succesfull = false; }

                        break;
                    case (Orientation.west):
                        if (temp.west.neighbour != null)
                        {
                            if (other == temp.west.neighbour) { return Success(i); }
                            temp = temp.west.neighbour;
                        }
                        else { succesfull = false; }

                        break;

                    case (Orientation.east):

                        if (temp.east.neighbour != null)
                        {
                            if (other == temp.east.neighbour) { return Success(i); } 
                            temp = temp.east.neighbour; 
                        }
                        else { succesfull = false; }

                        break;

                    case (Orientation.south):

                        if (temp.south.neighbour != null)
                        {
                            if (other == temp.south.neighbour) { return Success(i); }
                            temp = temp.south.neighbour;
                        }
                        else { succesfull = false; }
                        break;
                }
                if (succesfull == false) { break; }
            }

            // Change Orientation

        }

        // If the other room wasn't found, return false
        Debug.Log("Player not found in " + howFar + " distance");
        return false;
    }

    private bool Success(int distance)
    {
        Debug.Log("Success within " + distance);
        return true;
    }
    #endregion

    private void CreateHuskAt(int x, int y, Room room)
    {
        var newHusk = Instantiate(Resources.Load("RoomHusk")) as GameObject;
        RoomHusk husk = newHusk.GetComponent<RoomHusk>();



        float X = HuskToRoomPosition(x) + desiredLocation.x;
        float Y = HuskToRoomPosition(y) + desiredLocation.z;

        husk.Position(X, Y);
        newHusk.transform.parent = transform;
        husk.Name("Husk " + X + "," + Y);

        ManifestRoom(room, husk);

        // Manifest Items too
        ItemManager.Instance.ManifestRoomItems(room, new Vector3(X, 0f, Y));

        // Manifest Enemies too
        EnemyManager.Instance.ManifestEnemy(room, new Vector3(X, 0f, Y));
    }

    private float HuskToRoomPosition(int value)
    {
        float calcValue = (value - 2);


        return calcValue * WorldStats.Instance.Scale;

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
                MoveAndUpdate(currentRoom.north.neighbour);
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
                MoveAndUpdate(currentRoom.west.neighbour);

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
                MoveAndUpdate(currentRoom.east.neighbour);
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
                MoveAndUpdate(currentRoom.south.neighbour);
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

    private void MoveAndUpdate(Room newRoom)
    {
        // Update current room etc.
        currentRoom = newRoom;

        UpdateRooms();
    }

    public void UpdateRooms()
    {
        // Kill all children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        // Demanifest all items
        ItemManager.Instance.ClearManifestations();

        // Handle Enemies
        EnemyManager.Instance.ClearManifestations();
        EnemyManager.Instance.MoveEnemies();

        // Setup new husks
        FillUpRooms(currentRoom);


        DebugLogRoom(currentRoom);
    }


    #endregion
}
