using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Properties
    RoomExplorer roomExplorer;
    Writer writer;

    KeyCode InteractionKey = KeyCode.Space;

    KeyCode WriteKey = KeyCode.R;
    KeyCode EnterKey = KeyCode.Return;

    bool controlsDisabled = false;
    #endregion

    #region Setup
    private void Awake()
    {
        writer = FindObjectOfType<Writer>();
        roomExplorer = FindObjectOfType<RoomExplorer>();
    }

    private void Update()
    {
        if (controlsDisabled) { return; }

        if (!writer.writing)
        {
            CheckForTurnInput();
            CheckForMoveInput();
            CheckForInteractionInput();
        }

        CheckForWriteInput();
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

    private void CheckForInteractionInput()
    {
        if (Input.GetKeyDown(InteractionKey))
        {
            roomExplorer.Interact();
        }
    }

    private void CheckForWriteInput()
    {
        if (!writer.writing)
        {
            // Check if Player wishes to begin writing
            //if (Input.GetKeyDown(WriteKey) || Input.GetKeyDown(EnterKey))
            if (Input.GetKeyDown(EnterKey))
            {
                // Check if viable to Write
                if (writer.CheckIfLegalToWrite())
                {
                    // Start Writing
                    writer.StartWriting();
                }
            }
        }
        else
        {
            // Check what the Player is writing
            string text = writer.GetCurrentText();

            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (text.Length != 0)
                    {
                        text = text.Substring(0, text.Length - 1);
                        writer.Write(text);
                    }
                }
                else if ((c == '\n') || (c == '\r')) // enter/return
                {
                    //Debug.Log("Ending Writing");
                    // End writing
                    writer.EndWriting();
                }
                else
                {
                    // Just add it to the text
                    writer.Write(text + c);
                }
            }
        }
    }
    #endregion

    #region Helpers
    public void DisablePlayerControls()
    {
        controlsDisabled = true;
    }
    #endregion
}
