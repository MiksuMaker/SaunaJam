using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaManifest : ItemManifest
{
    #region Properties
    #endregion

    #region Setup
    public override void SetupManifest(Item item, Vector3 worldPos)
    {
        base.SetupManifest(item, worldPos);
    }
    #endregion

    #region Handling
    public override void UseItem()
    {
        // Do what you want the item to do here
        Debug.Log("Item " + item.type + " in use!");

        // Destroy after use
        //ItemManager.Instance.RemoveItem(this);
    }
    #endregion

    #region Functions

    #endregion
}
