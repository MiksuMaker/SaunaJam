using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    #region Debugs

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


    #region Neighbour Testing & Getting
    private void TryRenderAtDirectionFor(Room original, Orientation orientation, int howFar)
    {
        // Test for a room

        // Get the room

        Room temp = original;
        int X = 2; int Y = 2;

        bool succesfull = true;

        for (int i = 1; i < howFar + 1; i++)
        {

            switch (orientation)
            {
                case (Orientation.north):
                    Y += 1;
                    if (temp.north.neighbour != null)
                    {
                        // Move the temp forward
                        temp = temp.north.neighbour;

                        // Create
                        CreateHuskAt(X, Y, temp);
                    }
                    else
                    { succesfull = false; }

                    break;
                case (Orientation.south):
                    Y -= 1;
                    if (temp.south.neighbour != null)
                    {
                        // Move the temp forward
                        temp = temp.south.neighbour;

                        // Create
                        CreateHuskAt(X, Y, temp);
                    }
                    else
                    { succesfull = false; }

                    break;
                case (Orientation.west):
                    X -= 1;
                    if (temp.west.neighbour != null)
                    {
                        // Move the temp forward
                        temp = temp.west.neighbour;

                        // Create
                        CreateHuskAt(X, Y, temp);
                    }
                    else
                    { succesfull = false; }

                    break;
                case (Orientation.east):
                    X += 1;
                    if (temp.east.neighbour != null)
                    {
                        // Move the temp forward
                        temp = temp.east.neighbour;

                        // Create
                        CreateHuskAt(X, Y, temp);
                    }
                    else
                    { succesfull = false; }

                    break;
            }
            // If not succesfull, stop
            if (succesfull == false) { break; }
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
        if (original == other)
        { return true; }

        // Keep going through the neighbours as far as needed
        for (int j = 0; j < orientations.Length; j++)
        {
            // Reset temp
            temp = original;
            // Reset succesfull
            succesfull = true;

            //Debug.Log("Checking " + orientations[j]);

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
        //Debug.Log("Player not found in " + howFar + " distance");
        return false;
    }

    public bool IsOtherRoomInDirection(Room original, Room other, Orientation orientation, int howFar)
    {
        // Setup
        Room temp = original;
        bool succesfull = true;

        for (int i = 1; i < howFar + 1; i++)
        {

            switch (orientation)
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

        // Return false if not found
        return false;
    }


    public List<(Orientation, int)> ClosestOrientationToOtherRoom(Room original, Room other, int howFar)
    {
        Room temp = original;

        bool succesfull = true;

        Orientation[] orientations = new Orientation[] { Orientation.north, Orientation.west, Orientation.east, Orientation.south };

        // Create the list
        List<(Orientation, int)> list = new List<(Orientation, int)>();


        // Keep going through the neighbours as far as needed
        for (int j = 0; j < orientations.Length; j++)
        {
            // Reset temp
            temp = original;
            // Reset succesfull
            succesfull = true;

            // Reset foundRoom --> Add all orientations to a list
            bool foundRoom = false;

            for (int i = 1; i < howFar + 1; i++)
            {

                switch (orientations[j])
                {
                    case (Orientation.north):
                        if (temp.north.neighbour != null)
                        {
                            // Test if it is the room
                            if (other == temp.north.neighbour)
                            { foundRoom = true; } // <-- Is in sight --> CONTINUE
                            temp = temp.north.neighbour;    // Otherwise, continue the operation
                        }
                        else
                        { succesfull = false; }

                        break;
                    case (Orientation.west):
                        if (temp.west.neighbour != null)
                        {
                            if (other == temp.west.neighbour) { foundRoom = true; }
                            temp = temp.west.neighbour;
                        }
                        else { succesfull = false; }

                        break;

                    case (Orientation.east):

                        if (temp.east.neighbour != null)
                        {
                            if (other == temp.east.neighbour) { foundRoom = true; }
                            temp = temp.east.neighbour;
                        }
                        else { succesfull = false; }

                        break;

                    case (Orientation.south):

                        if (temp.south.neighbour != null)
                        {
                            if (other == temp.south.neighbour) { foundRoom = true; }
                            temp = temp.south.neighbour;
                        }
                        else { succesfull = false; }
                        break;
                }
                if (foundRoom == true)
                {
                    // Add it to the list
                    list.Add((orientations[j], i));
                    // Exit the loop for this orientation
                    break;
                }
                else if (succesfull == false) { break; }
            }
        }

        // Next after succesful orientations have been found, get the one with the shortest distance

        // (Unless there are no or only one member on the list
        if (list.Count <= 1) { return list; }

        // But after that, get the one with shortest distance (ties don't need to be randomized)
        list = list.OrderBy(x => x.Item2).ToList();

        return list;
    }


    private bool Success(int distance)
    {
        //Debug.Log("Success within " + distance);
        return true;
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

        // All the other directions
        int renderDistance = 10;
        TryRenderAtDirectionFor(cr, Orientation.north, renderDistance);
        TryRenderAtDirectionFor(cr, Orientation.east, renderDistance);
        TryRenderAtDirectionFor(cr, Orientation.south, renderDistance);
        TryRenderAtDirectionFor(cr, Orientation.west, renderDistance);


    }


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
    public bool TryChangeRoom(Vector3 moveVector)
    {
        if (currentRoom == null) { Debug.Log("Current Room is null"); }

        // Prep EnemyManager
        EnemyManager em = EnemyManager.Instance;

        if (moveVector == Vector3.forward)
        {
            // Try go north
            if (currentRoom.north.neighbour != null)
            {
                em.CheckIfRoomHasEnemies(currentRoom.north.neighbour);
                //if (em.CheckIfRoomHasEnemies(currentRoom.north.neighbour)) { return false; }
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
                em.CheckIfRoomHasEnemies(currentRoom.west.neighbour);
                //if (em.CheckIfRoomHasEnemies(currentRoom.west.neighbour)) { return false; }
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
                em.CheckIfRoomHasEnemies(currentRoom.east.neighbour);
                //if (em.CheckIfRoomHasEnemies(currentRoom.east.neighbour)) { return false; }
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
                em.CheckIfRoomHasEnemies(currentRoom.south.neighbour);
                //if (em.CheckIfRoomHasEnemies(currentRoom.south.neighbour)) { return false; }
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

    public void UpdateRooms(bool andMoveEnemies = true)
    {
        // Kill all children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        // Handle Particles
        ParticleManager.Instance.HideAllParticles();

        // Demanifest all items
        ItemManager.Instance.ClearManifestations();

        // Handle Enemies
        EnemyManager.Instance.ClearManifestations();
        if (andMoveEnemies) { EnemyManager.Instance.MoveEnemies(); }

        // Setup new husks
        FillUpRooms(currentRoom);


        DebugLogRoom(currentRoom);
    }


    #endregion
}
