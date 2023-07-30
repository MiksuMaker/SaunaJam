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
    public override void SetupManifest(Item item, Vector3 worldPos, RoomAttribute attribute)
    {
        base.SetupManifest(item, worldPos, attribute);

        // Setup the text
        text.text = item.description;
    }
    #endregion

    #region Functions
    public override void InteractWithItem()
    {
        // Do nothing

        // Maybe prompt the user to use ENTER instead
    }

    public override void UpdateManifest()
    {
        // Update the text component
        text.text = item.description;
    }
    #endregion
}
