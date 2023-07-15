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
        CheckForTurnInput();
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveVector += Vector3.back;

        }

        if (moveVector == Vector3.zero) { return; }

        if (roomExplorer == null) { Debug.Log("RoomExplorer not found"); }
        roomExplorer.Explore2(moveVector);
    }

    private void CheckForTurnInput()
    {
        Vector3 turnVector = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.A))
        {
            turnVector += Vector3.left;

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            turnVector += Vector3.right;
        }

        if (turnVector == Vector3.zero) { return; }

        roomExplorer.TurnFacingDirection(turnVector);
    }
    #endregion
}
