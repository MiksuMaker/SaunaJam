using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaStoneManifest : ItemManifest
{
    public override void SetupManifest(Item item, Vector3 worldPos)
    {
        base.SetupManifest(item, worldPos);
    }

    public override void UseItem()
    {
        Debug.Log("SaunaStone was collected!");

        // Notify that WOOD was collected

        // Destroy item
        base.UseItem();
    }
}
