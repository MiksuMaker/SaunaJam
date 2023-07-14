using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExplorer : MonoBehaviour
{
    #region Properties

    #endregion

    #region Setup
    
    #endregion

    #region Functions
    public void Explore(Vector3 moveVector)
    {
        RoomManager.Instance.TryChangeRoom(moveVector);
    }
    #endregion
}
