using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Writer : MonoBehaviour
{
    #region Properties
    RoomExplorer explorer;
    ItemManager itemM;
    RoomManager roomM;

    Item currentText;
    ItemManifest currentTextManifest;
    #endregion

    #region Setup
    private void Awake()
    {
        itemM = ItemManager.Instance;
        roomM = RoomManager.Instance;

        explorer = FindObjectOfType<RoomExplorer>();
    }

    public void OpenWriting()
    {

    }
    #endregion

    #region Functions
    public void Write()
    {
        if (itemM.GetItem(roomM.currentRoom, explorer.currentOrientation))
        {

        }
    }
    #endregion
}
