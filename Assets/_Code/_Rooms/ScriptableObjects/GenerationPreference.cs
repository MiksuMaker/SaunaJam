using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RoomGenerationPreference")]
public class GenerationPreference : ScriptableObject
{
    [Header("Amount of Rooms")]
    public int amountOfRooms = 20;

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

    [Header("Rule Settings")]
    [SerializeField] public bool useRules = true;
    [SerializeField] public List<Rule> rules;


    [System.Serializable]
    public class Rule
    {
        public TypeRoom fromType;   // Rule is activated if this is the type of room connecting from

        public TypeRoom afterType;  // ChanceModifier alters chances for this type of room

        public float chanceModifier; // 0 = disabled, 10 (etc. high number) = almost guaranteed
    }
}
