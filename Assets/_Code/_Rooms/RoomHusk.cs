using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHusk : MonoBehaviour
{
    // RoomHusk is used for physical representation of the rooms

    #region Properties
    // Generation
    GameObject graphics;

    #endregion

    #region Setup
    public void SetupRoomHusk(GameObject _graphics)
    {
        if (graphics != null) { Destroy(graphics); }

        graphics = _graphics;
    }
    #endregion

    #region Functions
    public void RotateGraphics(Quaternion rotation)
    {
        graphics.transform.rotation = rotation;
    }
    #endregion
}
