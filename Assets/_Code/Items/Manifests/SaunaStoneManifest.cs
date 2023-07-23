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
        // Notify that WOOD was collected
        Collector.Instance.CollectItem(item.type);

        // Destroy item
        base.UseItem();
    }
}
