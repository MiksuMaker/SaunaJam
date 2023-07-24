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
    public virtual void SetupManifest(Item item, Vector3 worldPos)
    {
        this.item = item;

        // Setup position and rotation
        transform.position = worldPos;
        RotationToOrientation(item.wallOrientation);
    }

    public virtual void UpdateManifest()
    {
        //
    }
    #endregion

    #region Handling
    public virtual void InteractWithItem()
    {
        // Do what you want the item to do here

        // Destroy after use
        ItemManager.Instance.RemoveItemManifest(this);
    }
    #endregion

    #region Helpers
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
