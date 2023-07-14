using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    // Room is used to store the data about the rooms
    public string name;

    public TypeRoom type;
    public Orientation orientation;

    public Connection north;
    public Connection west;
    public Connection east;
    public Connection south;

    public Room()
    {
        name = Random.Range(0, 100).ToString();

        north = new Connection();
        west = new Connection();
        east = new Connection();
        south = new Connection();
    }
}

public class Connection
{
    public bool isWall = false;
    public Room neighbour;

    public Connection()
    {
        neighbour = null;
    }
}

public class Changeling : Connection
{
    // Same as Room but prone to change
}