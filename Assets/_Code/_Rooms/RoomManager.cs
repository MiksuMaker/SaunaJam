using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Properties
    static public RoomManager Instance;

    public Room currentRoom;

    List<Room> roomsList = new List<Room>();

    public int roomsCount { get { return roomsList.Count; } }

    // Manifestation
    RoomHusk roomHusk;

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

        roomHusk = FindObjectOfType<RoomHusk>();
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
        ManifestRoom(roomsList[0]);
    }

    public void ManifestRoom(Room room)
    {
        PickRoomGraphics(room, roomHusk);
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
        husk.SetupRoomHusk(graphics);

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
}
