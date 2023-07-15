using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHusk : MonoBehaviour
{
    // RoomHusk is used for physical representation of the rooms

    #region Properties
    // Generation
    [HideInInspector]
    public GameObject graphics;
    public bool hasGraphics { get { return (graphics != null); } }

    IEnumerator roomMover;
    #endregion

    #region Setup
    public void ChangeHuskGraphics(GameObject _graphics)
    {
        if (graphics != null) { Destroy(graphics); }

        //if (!graphics.activeSelf) { graphics.SetActive(true); }

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

    public void HideHuskGraphics()
    {
        //graphics.SetActive(false);
        if (graphics != null) { Destroy(graphics); }
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

    #region Moving
    public void Adopt(GameObject _graphics)
    {
        graphics = _graphics;
    }

    public void MoveRoom()
    {
        MoveRoomGraphics(graphics.transform.position);
    }

    private void MoveRoomGraphics(Vector3 fromPos)
    {
        if (roomMover != null)
        {
            StopCoroutine(roomMover);
        }
        roomMover = RoomMover(1f, fromPos);
    }

    IEnumerator RoomMover(float desiredTime, Vector3 fromPos)
    {
        float increment = 0.1f;

        WaitForSeconds wait = new WaitForSeconds(increment);

        float waitedTime = 0f;
        while (waitedTime < desiredTime)
        {
            yield return wait;

            waitedTime += increment;

            // Move
            graphics.transform.position = Vector3.Lerp(fromPos, transform.position, (waitedTime / desiredTime));
        }

        // Finish moving
        graphics.transform.position = transform.position;
    }
    #endregion
}
