using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    #region Properties

    #endregion

    #region Setup

    #endregion

    #region Functions
    public void InteractWithItem(ItemManifest itemManifest)
    {
        // Default to just use
        itemManifest.UseItem();
    }
    #endregion
}
