using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeRoom
{
    _1_deadEnd,
    _2_corner, _2_straight,
    _3_threeway,
    _4_fourway,

    skip,

    //wide_1_deadEnd,
}

public enum RoomAttribute
{
    thin, wide,
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

    List<Room> north_connections = new List<Room>();
    List<Room> south_connections = new List<Room>();
    List<Room> west_connections = new List<Room>();
    List<Room> east_connections = new List<Room>();

    [Header("Preset")]
    [SerializeField]
    PreSet startingLayout;

    [Header("Room Generation Settings")]
    #region Preferences
    [SerializeField]
    GenerationPreference preferences;
    [SerializeField] bool usePreferences = true;

    [Space(10)]

    [SerializeField] float deadEnd_Chance = 1f;
    [SerializeField] float corner_Chance = 1f;
    [SerializeField] float straight_Chance = 1f;
    [SerializeField] float threeway_Chance = 1f;
    [SerializeField] float fourway_Chance = 1f;
    [SerializeField] float skip_Chance = 1f;

    [SerializeField] bool forbidCutttingShort = true;
    #endregion

    enum DirectionOfConnection
    {
        north,
        west, east,
        south,

        start,
    }

    enum CutShortMode
    {
        allowed, forceDeadEnd, forbidden,
    }

    #endregion

    #region Setup
    private void Start()
    {
        // Set Preferences
        SetPreferences();

        // Mockup start
        GenerateRooms();
    }

    private void SetPreferences()
    {
        if (preferences == null || !usePreferences) { return; }

        // Set settings according to Preferences
        amountOfRooms = preferences.amountOfRooms;

        deadEnd_Chance = preferences.deadEndChance;
        corner_Chance = preferences.cornerChance;
        straight_Chance = preferences.straigthChance;
        threeway_Chance = preferences.threewayChance;
        fourway_Chance = preferences.fourwayChance;
        skip_Chance = preferences.skipChance;

        forbidCutttingShort = preferences.forbidCuttingShort;
    }
    #endregion

    #region ROOM GENERATION PROTOCOL
    public void GenerateRooms()
    {
        // Generate first room
        GenerateFirstRooms();

        // Keep generating rooms until you have generated enough
        RoomGenerationLoop();

        // Generate Items
        ItemManager.Instance.SpawnItems();

        // Manifest it
        RoomManager.Instance.LoadInitialRooms();
    }

    private void RoomGenerationLoop()
    {
        while (createdAmountOfRooms < amountOfRooms)
        {


            if (roomsWithUnfinishedConnections.Count == 0) { Debug.Log("Run out of Unfinished connections!"); break; }

            // Pick an unfinished connection room
            Room nextToConnect = roomsWithUnfinishedConnections[0];

            // Handle Settings
            CutShortMode dMode = CutShortMode.allowed;
            if (forbidCutttingShort && roomsWithUnfinishedConnections.Count <= 1)
            { /*Debug.Log("DeadEnds forbidden as last piece!");*/ dMode = CutShortMode.forbidden; }

            // Create rooms for each unconnected connection
            CheckDirectionsAndGenerateIfNecessary(nextToConnect, dMode);


        }
        if (createdAmountOfRooms >= amountOfRooms) { Debug.Log("Created full amount of rooms"); }

        Debug.Log("Rooms amount: " + RoomManager.Instance.roomsCount);


        // Fill up the rest of  unfinished connections with dead ends
        FillDeadEnds();
    }


    private void FillDeadEnds()
    {
        // Go through all the rooms, check which ones don't have finished connections
        int roomCount = RoomManager.Instance.roomsCount;

        for (int i = 0; i < roomCount; i++)
        {
            CheckForUnfinishedConnections(RoomManager.Instance.roomsList[i]);
        }


        foreach (Room r in roomsWithUnfinishedConnections)
        {
            ListUpPotentialConnections(r);
        }

        // Combine them at random
        ConnectUpPotentials(north_connections, south_connections, DirectionOfConnection.south);
        ConnectUpPotentials(west_connections, east_connections, DirectionOfConnection.east);

        roomsWithUnfinishedConnections.Clear();

        // Check again
        for (int i = 0; i < roomCount; i++)
        {
            CheckForUnfinishedConnections(RoomManager.Instance.roomsList[i]);
        }

        // This time just fill them with dead ends
        int stubCount = roomsWithUnfinishedConnections.Count;
        for (int i = 0; i < stubCount; i++)
        {
            CheckDirectionsAndGenerateIfNecessary(roomsWithUnfinishedConnections[0], CutShortMode.forceDeadEnd); // V1, just dead ends
        }
    }

    private void ConnectUpPotentials(List<Room> a, List<Room> b, DirectionOfConnection fromAtoB)
    {

        int a_count = a.Count;
        int b_count = b.Count;

        int max = Mathf.Min(a_count, b_count);
        //Debug.Log("Max connections: " + max);

        // Connect random members of each team to each other
        for (int i = 0; i < max; i++)
        {
            // Take random members
            Room A = a[Random.Range(0, a.Count)];
            Room B = b[Random.Range(0, b.Count)];

            // Check that they're not the same
            if (A == B) { continue; }

            // Connect them!
            ConnectRooms(A, B, fromAtoB);

            // Remove them from the lists
            a.Remove(A);
            b.Remove(B);
        }
    }
    #endregion


    #region First Room Creation
    private void GenerateFirstRooms()
    {
        // Check if Starting Layout has been given
        if (startingLayout != null)
        {
            // Use the Starting Layout
            UnravelStartingLayout();
        }
        else
        {
            DoBasicLayout();
        }
    }

    private void UnravelStartingLayout()
    {
        List<(Room, PreSet.Layout)> startingRooms = new List<(Room, PreSet.Layout)>();

        // Go through all the Rooms, create them
        foreach (PreSet.Layout i in startingLayout.instructions)
        {
            // Create a room
            Room r = new Room();
            r.name = i.name;
            r.type = i.type;
            r.orientation = i.orientation;

            // Check if Sauna
            if (r.name == "Sauna") { r.attribute = RoomAttribute.wide; }

            SetupRoomWalls(r, r.type, r.orientation);
            RoomManager.Instance.AddRoomToList(r);
            //SetupSauna(r);
            SetupItems(r, i);

            // Pair it up
            startingRooms.Add((r, i));
        }

        // Connect up all the rooms accordingly
        int n = startingRooms.Count;
        Debug.Log("Starting Rooms Count: " + startingRooms.Count);
        foreach (var s in startingRooms)
        {
            #region Neighbour check zone
            // Check that it isn't connected yet AND desires a neighbour
            if (s.Item2.north_Neighbour_name != "")
            {
                // Go through all the rooms and check what is named as the neighbour
                for (int i = 0; i < n; i++)
                {
                    if (startingRooms[i] == s) { continue; }

                    if (startingRooms[i].Item2.name == s.Item2.north_Neighbour_name)
                    {
                        // They are neighbours! ---> CONNECT THEM
                        ConnectRooms(s.Item1, startingRooms[i].Item1, DirectionOfConnection.south);
                        //DebugRoomCreation(s.Item1, startingRooms[i].Item1);
                    }
                }

            }
            #region Rest of the checks
            // WEST
            if (s.Item2.west_Neighbour_name != "")
            {
                // Go through all the rooms and check what is named as the neighbour
                for (int i = 0; i < n; i++)
                {
                    if (startingRooms[i] == s) { continue; }
                    if (startingRooms[i].Item2.name == s.Item2.west_Neighbour_name)
                    {
                        ConnectRooms(s.Item1, startingRooms[i].Item1, DirectionOfConnection.east);
                        //DebugRoomCreation(s.Item1, startingRooms[i].Item1);
                    }
                }
            }
            // EAST
            if (s.Item2.east_Neighbour_name != "")
            {
                // Go through all the rooms and check what is named as the neighbour
                for (int i = 0; i < n; i++)
                {
                    if (startingRooms[i] == s) { continue; }
                    if (startingRooms[i].Item2.name == s.Item2.east_Neighbour_name)
                    {
                        ConnectRooms(s.Item1, startingRooms[i].Item1, DirectionOfConnection.west);
                        //DebugRoomCreation(s.Item1, startingRooms[i].Item1);
                    }
                }
            }
            // SOUTH
            if (s.Item2.south_Neighbour_name != "")
            {
                // Go through all the rooms and check what is named as the neighbour
                for (int i = 0; i < n; i++)
                {
                    if (startingRooms[i] == s) { continue; }
                    if (startingRooms[i].Item2.name == s.Item2.south_Neighbour_name)
                    {
                        ConnectRooms(s.Item1, startingRooms[i].Item1, DirectionOfConnection.north);
                        //DebugRoomCreation(s.Item1, startingRooms[i].Item1);
                    }
                }
            }
            #endregion
            #endregion
        }

        // See if there are any unfinished rooms
        foreach (var u in startingRooms)
        {
            // Check each for unfinished rooms
            CheckForUnfinishedConnections(u.Item1);
        }
    }

    private void DebugRoomCreation(Room r1, Room r2)
    {
        Debug.Log("Room " + r1.name + " has been connected with Room " + r2.name);
    }

    private void DoBasicLayout()
    {
        // Make new Room
        Room room = new Room(1);

        // Room type
        room.type = DecideRoomType();

        // Orientation
        room.orientation = DecideOrientation(room.type, DirectionOfConnection.north);

        // Setup walls for the Room
        SetupRoomWalls(room, room.type, room.orientation);

        // Add to the RoomsList
        RoomManager.Instance.AddRoomToList(room);
        RoomManager.Instance.currentRoom = room;

        createdAmountOfRooms++;
        roomsWithUnfinishedConnections.Add(room);   // Add to Unfinished rooms

        // Make the Sauna room a dead end in front of Player
        Room saunaRoom = new Room(0);
        saunaRoom.type = TypeRoom._1_deadEnd;
        saunaRoom.orientation = Orientation.south;
        SetupRoomWalls(saunaRoom, saunaRoom.type, saunaRoom.orientation);
        RoomManager.Instance.AddRoomToList(saunaRoom);
        createdAmountOfRooms++;

        // Connect them
        ConnectRooms(saunaRoom, room, DirectionOfConnection.north);
    }
    #endregion

    private void SetupItems(Room r, PreSet.Layout l)
    {
        if (!l.hasItems)
        {
            //Debug.Log("Room " + l.name + " has no Items!"); 
            return;
        }

        ItemManager.Instance.DecorateRoom(r, l.items);
    }

    #region New Room Generation
    private void GenerateNewRoom(DirectionOfConnection connectionFrom, Room fromRoom, CutShortMode deadMode = CutShortMode.allowed)
    {
        // First, DECIDE Room type
        TypeRoom type;

        // THIS needs to take DeadEndMode into calculations, and handle it with grace
        if (deadMode == CutShortMode.forceDeadEnd)
        {
            type = TypeRoom._1_deadEnd;
        }
        else
        {
            float deadEndChance = deadEnd_Chance * CheckRuleBook(fromRoom, TypeRoom._1_deadEnd);
            float skipChance = skip_Chance * CheckRuleBook(fromRoom, TypeRoom.skip);

            if (deadMode == CutShortMode.forbidden) { deadEndChance = 0f; skipChance = 0f; }

            type = DecideRoomType(deadEndChance,
                                  corner_Chance * CheckRuleBook(fromRoom, TypeRoom._2_corner),
                                  straight_Chance * CheckRuleBook(fromRoom, TypeRoom._2_straight),
                                  threeway_Chance * CheckRuleBook(fromRoom, TypeRoom._3_threeway),
                                  fourway_Chance * CheckRuleBook(fromRoom, TypeRoom._4_fourway),
                                  skipChance);
        }

        // Check if the room should be SKIPPED
        if (type == TypeRoom.skip) { /*Debug.LogWarning("Skipped"); */ return; /* Skip it */}


        // THEN, Make new Room
        Room room = new Room(fromRoom.depth + 1);

        // Then remember to assign type!
        room.type = type;

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

    private TypeRoom DecideRoomType(float deadC = 1f, float cornerC = 1f, float straightC = 1f, float threeC = 1f, float fourC = 1f, float skipC = 0f)
    {
        float deadChance = deadC;
        float cornerChance = deadChance + cornerC;
        float straightChance = cornerChance + straightC;
        float threeChance = straightChance + threeC;
        float fourChance = threeChance + fourC;
        float skipChance = fourChance + skipC;
        float total = skipChance;   // Cap the chances

        float rand = Random.Range(0f, total);

        TypeRoom t = TypeRoom._1_deadEnd; // Default

        if (rand <= deadChance) { t = TypeRoom._1_deadEnd; }
        else if (rand <= cornerChance) { t = TypeRoom._2_corner; }
        else if (rand <= straightChance) { t = TypeRoom._2_straight; }
        else if (rand <= threeChance) { t = TypeRoom._3_threeway; }
        else if (rand <= fourChance) { t = TypeRoom._4_fourway; }
        else if (rand <= skipChance) { t = TypeRoom.skip; }

        return t;
    }

    private Orientation DecideOrientation(TypeRoom type, DirectionOfConnection connectFrom)
    {

        switch (type, connectFrom)
        {
            // DEAD ENDS : No need for randomization
            case (TypeRoom._1_deadEnd, DirectionOfConnection.north): return Orientation.north;
            case (TypeRoom._1_deadEnd, DirectionOfConnection.west): return Orientation.west;
            case (TypeRoom._1_deadEnd, DirectionOfConnection.east): return Orientation.east;
            case (TypeRoom._1_deadEnd, DirectionOfConnection.south): return Orientation.south;

            // STRAIGHTS : No need for randomization
            case (TypeRoom._2_straight, DirectionOfConnection.north): return Orientation.north;
            case (TypeRoom._2_straight, DirectionOfConnection.west): return Orientation.west;
            case (TypeRoom._2_straight, DirectionOfConnection.east): return Orientation.west;
            case (TypeRoom._2_straight, DirectionOfConnection.south): return Orientation.north;

            // CORNERS : Decide a direction
            case (TypeRoom._2_corner, DirectionOfConnection.north):
                return GetRandomValidOrientation(new Orientation[2]
                                                   { Orientation.north,
                                                     Orientation.west});
            case (TypeRoom._2_corner, DirectionOfConnection.west):
                return GetRandomValidOrientation(new Orientation[2]
                                                    { Orientation.south,
                                                      Orientation.west});
            case (TypeRoom._2_corner, DirectionOfConnection.east):
                return GetRandomValidOrientation(new Orientation[2]
                                                    { Orientation.north,
                                                      Orientation.east});
            case (TypeRoom._2_corner, DirectionOfConnection.south):
                return GetRandomValidOrientation(new Orientation[2]
                                                   { Orientation.south,
                                                     Orientation.east});

            // THREEWAY : Decide a direction
            case (TypeRoom._3_threeway, DirectionOfConnection.north):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.north,
                                                     Orientation.west,
                                                     Orientation.south});
            case (TypeRoom._3_threeway, DirectionOfConnection.west):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.east,
                                                     Orientation.west,
                                                     Orientation.south});
            case (TypeRoom._3_threeway, DirectionOfConnection.east):
                return GetRandomValidOrientation(new Orientation[3]
                                                   { Orientation.north,
                                                     Orientation.west,
                                                     Orientation.east});
            case (TypeRoom._3_threeway, DirectionOfConnection.south):
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

    private float CheckRuleBook(Room r, TypeRoom typeInQuestion)
    {
        // If no Rules, default to 1
        if (preferences == null || !preferences.useRules || preferences.rules.Count == 0) { return 1f; }

        // Check rules for TypeInQuestion
        for (int i = 0; i < preferences.rules.Count; i++)
        {
            // Check if the FROM-ROOM type has rules
            if (preferences.rules[i].fromType == r.type)
            {
                // Check that this is the correct type in question
                if (preferences.rules[i].afterType == typeInQuestion)
                {
                    // The Rule states that this FromType has consequences for the TypeInQuestion
                    return preferences.rules[i].chanceModifier;
                }
            }
        }

        // If no Rules regarding TypeInQuestion, default to 1
        return 1;
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

    private void ConnectRooms(Room previous, Room newOne, DirectionOfConnection dir)
    {
        // Previous room connects to the new one, new one connects to the previous one

        switch (dir)
        {
            case DirectionOfConnection.north: previous.south.neighbour = newOne; newOne.north.neighbour = previous; break;
            case DirectionOfConnection.west: previous.east.neighbour = newOne; newOne.west.neighbour = previous; break;
            case DirectionOfConnection.east: previous.west.neighbour = newOne; newOne.east.neighbour = previous; break;
            case DirectionOfConnection.south: previous.north.neighbour = newOne; newOne.south.neighbour = previous; break;
        }
    }

    private void CheckDirectionsAndGenerateIfNecessary(Room r, CutShortMode dMode = CutShortMode.allowed)
    {
        // Check each connection and connect if needed
        if (CheckConnection(r.north))
        {
            // Create a room there
            GenerateNewRoom(DirectionOfConnection.south, r, dMode);
        }
        if (CheckConnection(r.west))
        {
            GenerateNewRoom(DirectionOfConnection.east, r, dMode);
        }
        if (CheckConnection(r.east))
        {
            GenerateNewRoom(DirectionOfConnection.west, r, dMode);
        }
        if (CheckConnection(r.south))
        {
            GenerateNewRoom(DirectionOfConnection.north, r, dMode);
        }

        // Remove from unifinished rooms
        roomsWithUnfinishedConnections.Remove(r);
    }

    private void ListUpPotentialConnections(Room r)
    {
        // Check each connection and connect if needed
        if (CheckConnection(r.north))
        {
            // Add to correct list
            north_connections.Add(r);
        }
        if (CheckConnection(r.west))
        {
            west_connections.Add(r);
        }
        if (CheckConnection(r.east))
        {
            east_connections.Add(r);
        }
        if (CheckConnection(r.south))
        {
            south_connections.Add(r);
        }
    }

    #endregion
}


