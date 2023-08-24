using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStats : MonoBehaviour
{
    static public WorldStats Instance;

    public GameObject Player { get; set; }
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

        if (Player == null)
        {
            Player = FindObjectOfType<RoomExplorer>().gameObject;
        }
    }

    public float X = 6f;
    public float Z = 6f;
    public float Scale = 6f;

    public int numberOfRooms = 0;
    public int getNumberOfRoomsAndCountup { get { int num = numberOfRooms; numberOfRooms++; return num; } }

    public float timeFactor = 1f;
}
