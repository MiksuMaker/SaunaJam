using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManifest : ItemManifest
{
    public override void SetupManifest(Item item, Vector3 worldPos)
    {
        base.SetupManifest(item, worldPos);
    }

    public override void UseItem()
    {
        // Notify that water was collected
        Collector.Instance.CollectItem(item.type);

        // Destroy item
        base.UseItem();
    }
}
