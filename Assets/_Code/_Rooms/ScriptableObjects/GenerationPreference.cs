using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RoomGenerationPreference")]
public class GenerationPreference : ScriptableObject
{
    [Header("Chances for certain room type")]

    // What is spesific type of room's chance of spawning in

    public float deadEndChance = 1f;
    public float cornerChance = 1f;
    public float straigthChance = 1f;
    public float threewayChance = 1f;
    public float fourwayChance = 1f;
    public float skipChance = 1f;

    [Space(10)]
    // Can a RoomGeneration be cut short from an "unlucky" DeadEnd?
    public bool forbidCuttingShort = true;   
}
