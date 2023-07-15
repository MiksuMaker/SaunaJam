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
    public void ChangeRoomHuskGraphics(GameObject _graphics)
    {
        if (graphics != null) { Destroy(graphics); }

        if (!graphics.activeSelf) { graphics.SetActive(true); }

        graphics = _graphics;
    }
    public void Name(string newName)
    {
        gameObject.name = newName;
    }
    #endregion

    #region Graphics
    public void RotateGraphics(Quaternion rotation)
    {
        graphics.transform.rotation = rotation;
    }

    public void HideGraphics()
    {
        graphics.SetActive(false);
    }
    #endregion

    #region Positioning
    public void Position(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Position(float X, float Z)
    {
        transform.position = new Vector3(X, 0f, Z);
    }
    #endregion
}
