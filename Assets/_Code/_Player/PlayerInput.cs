using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Properties
    RoomExplorer roomExplorer;
    #endregion

    #region Setup
    private void Start()
    {
        roomExplorer = FindObjectOfType<RoomExplorer>();
    }

    private void Update()
    {
        CheckForMoveInput();
    }
    #endregion

    #region Functions
    private void CheckForMoveInput()
    {
        Vector3 moveVector = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W))
        {
            moveVector += Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            moveVector += Vector3.left;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            moveVector += Vector3.right;

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveVector += Vector3.back;

        }

        if (moveVector == Vector3.zero) { return; }

        if (roomExplorer == null) { Debug.Log("RoomExplorer not found"); }
        roomExplorer.Explore(moveVector);
    }
    #endregion
}
