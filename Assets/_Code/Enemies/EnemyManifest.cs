using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManifest : MonoBehaviour
{
    [SerializeField] public GameObject gnomeGraphics;

    public void AlterGnomeGraphics(bool onOff)
    {
        gnomeGraphics.SetActive(onOff);
    }

    public void TurnManifest(Orientation turnOrientation)
    {
        Vector3 eulers = Vector3.zero;
        switch (turnOrientation)
        {
            case Orientation.north: break;
            case Orientation.west: eulers = new Vector3(0f, -90f, 0f); break;
            case Orientation.east: eulers = new Vector3(0f, 90f, 0f); break;
            case Orientation.south: eulers = new Vector3(0f, 180f, 0f); break;
        }

        // Rotate
        gnomeGraphics.transform.rotation = Quaternion.Euler(eulers);
    }
}
