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
    private void Start()
    {
        itemM = ItemManager.Instance;
        roomM = RoomManager.Instance;

        explorer = FindObjectOfType<RoomExplorer>();
    }

    public bool CheckIfLegalToWrite()
    {
        Room currentRoom = roomM.currentRoom;
        Orientation orientation = explorer.currentOrientation;

        // Check if there is a wall
        if (itemM.CheckWallValidity(currentRoom, orientation))
        {
            // Next Check if it is an empty wall OR if there is already some text
            if (!itemM.CheckIfOccupied(currentRoom, orientation) || itemM.CheckForWriting(currentRoom, orientation))
            {
                return true;
            }
        }
        // Else
        return false;
    }

    public void StartWriting()
    {
        Room curRoom = roomM.currentRoom;
        Orientation Ori = explorer.currentOrientation;
        // Check if there already is text
        if (itemM.CheckForWriting(curRoom, Ori))
        {
            // There is? Continue writing that
            currentText = itemM.GetItem(curRoom, Ori);
            currentTextManifest = itemM.GetItemManifest(curRoom, Ori);
        }
        else
        {
            // If not, create new writing item + manifest
            itemM.CreateAndAddItem(Item.Type.writing, curRoom, Ori);
            currentText = itemM.GetItem(curRoom, Ori);
            currentTextManifest = itemM.GetItemManifest(curRoom, Ori);
        }
    }

    public void StopWriting()
    {
        currentText = null;
        currentTextManifest = null;
    }
    #endregion

    #region Functions
    public void Write(string input)
    {
        Debug.Log("TextItem: " + currentText.description);
        Debug.Log("TextManifest: " + currentTextManifest.item.description);

        // Add it to current text
        currentText.description = input;

        // Update the manifest
        currentTextManifest.UpdateManifest();
    }

    public string GetCurrentText()
    {
        return currentText.description;
    }
    #endregion
}
