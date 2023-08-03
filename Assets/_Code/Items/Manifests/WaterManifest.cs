using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManifest : ItemManifest
{
    public override void SetupManifest(Item item, Vector3 worldPos, RoomAttribute attribute)
    {
        base.SetupManifest(item, worldPos, attribute);
    }

    public override void InteractWithItem()
    {
        // Destroy item
        base.InteractWithItem();

        // Notify that water was collected
        Collector.Instance.CollectItem(item.type);

    }
}
