using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaManifest : ItemManifest
{
    #region Properties
    #endregion

    #region Setup
    public override void SetupManifest(Item item, Vector3 worldPos, RoomAttribute attribute)
    {
        base.SetupManifest(item, worldPos, attribute);
    }
    #endregion

    #region Handling
    public override void InteractWithItem()
    {
        // Do what you want the item to do here
        SaunaManager.Instance.TrySacrificeItems();

        // Destroy after use
        //ItemManager.Instance.RemoveItem(this);
    }
    #endregion

    #region Functions

    #endregion
}
