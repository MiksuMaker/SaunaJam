using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaManifest : ItemManifest
{
    #region Properties
    public Animator animator {  get { return graphicsGO.GetComponent<Animator>(); } }
    #endregion

    #region Setup
    public override void SetupManifest(Item item, Vector3 worldPos, RoomAttribute attribute)
    {
        base.SetupManifest(item, worldPos, attribute);

        // Hook up with Sauna Graphics
        SaunaManager.Instance.ConnectSaunaGraphics(this);
    }
    #endregion

    #region Handling
    public override void InteractWithItem()
    {
        // Check that no dialogue is in process
        if (UI_Controller.Instance.textInProcess || SaunaManager.Instance.sacrificeInProcess)
        { return; }

        // Do what you want the item to do here
        SaunaManager.Instance.TrySacrificeItems();

        // Destroy after use
        //ItemManager.Instance.RemoveItem(this);
    }
    #endregion

    #region Functions

    #endregion
}
