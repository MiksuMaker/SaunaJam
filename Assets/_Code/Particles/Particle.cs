using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public enum Type
    {
        // SAUNA


        // MONSTERS
        steamMonster,
    }
    public Type type;

    public bool inUse = false;

    public void Hide()
    {
        // Place at hidden location
        gameObject.transform.position = Vector3.down * 100f;

        inUse = false;
    }

    public void UseAt(Vector3 worldPos)
    {
        inUse = true;

        gameObject.transform.position = worldPos;
    }
}

