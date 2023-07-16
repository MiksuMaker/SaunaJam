using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManifest : MonoBehaviour
{
    #region Properties
    public Item item;
    public GameObject go { get { return gameObject; } }
    #endregion

    #region Setup
    public virtual void SetupManifest(Vector3 worldPos)
    {
        // Setup position and rotation
        transform.position = worldPos;
        RotationToOrientation(item.wallOrientation);
    }
    #endregion

    #region Functions
    protected void RotationToOrientation(Orientation wall)
    {
        float angle = 0f;
        switch (wall)
        {
            case Orientation.north: angle = 0f; break;
            case Orientation.west: angle = -90f; break;
            case Orientation.east: angle = 90f; break;
            case Orientation.south: angle = 180f; break;
        }

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    #endregion
}
