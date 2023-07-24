using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodManifest : ItemManifest
{
    public override void SetupManifest(Item item, Vector3 worldPos)
    {
        //Debug.Log("Wood is manifested!");
       base.SetupManifest(item, worldPos);
    }

    public override void InteractWithItem()
    {
        // Notify that WOOD was collected
        Collector.Instance.CollectItem(item.type);

        // Destroy item
        base.InteractWithItem();
    }
}
