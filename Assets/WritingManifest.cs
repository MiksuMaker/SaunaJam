using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WritingManifest : ItemManifest
{
    #region Properties
    public TextMeshPro text;
    #endregion

    #region Setup
    public override void SetupManifest(Item item, Vector3 worldPos)
    {
        base.SetupManifest(item, worldPos);

        // Setup the text
        text.text = item.description;
    }
    #endregion

    #region Functions
    public override void UpdateManifest()
    {
        // Update the text component
        text.text = item.description;
    }
    #endregion
}
