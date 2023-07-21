using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManifest : ItemManifest
{
    public override void SetupManifest(Item item, Vector3 worldPos)
    {
        Debug.Log("Wood is manifested!");
       base.SetupManifest(item, worldPos);
    }

    public override void UseItem()
    {
        Debug.Log("WOOD was collected!");

        // Notify that WOOD was collected

        // Destroy item
        base.UseItem();
    }
}
