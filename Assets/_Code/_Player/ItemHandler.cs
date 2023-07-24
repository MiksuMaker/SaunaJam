using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    #region Properties
    Writer writer;
    #endregion

    #region Setup

    #endregion

    #region Functions
    public void InteractWithItem(ItemManifest itemManifest)
    {
        //if (itemManifest.item.type == Item.Type.writing)
        //{
        //    // Instead write
        //    if (writer == null) { writer = GetComponent<Writer>(); }
        //    writer.StartWriting();
        //}

        // Default to just use
        itemManifest.InteractWithItem();
    }
    #endregion
}
