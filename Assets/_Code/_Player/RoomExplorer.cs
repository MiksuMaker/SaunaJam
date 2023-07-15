using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExplorer : MonoBehaviour
{
    #region Properties
    public Direction facingDirection = Direction.north;

    [SerializeField]
    GameObject Player;

    IEnumerator turnCoroutine;
    #endregion

    #region Setup

    #endregion

    #region Functions
    public void Explore(Vector3 moveVector)
    {
        float value = moveVector.z;
        Vector3 actualMoveVector = Vector3.zero;
        switch (facingDirection)
        {
            case (Direction.north): actualMoveVector += Vector3.forward * value; break;
            case (Direction.west): actualMoveVector += Vector3.left * value; break;
            case (Direction.east): actualMoveVector += Vector3.right * value; break;
            case (Direction.south): actualMoveVector += Vector3.back * value; break;
        }

        RoomManager.Instance.TryChangeRoom(actualMoveVector);
    }

    public void TurnFacingDirection(Vector3 turnVector)
    {
        bool rightwise = true;
        if (turnVector == Vector3.left) { rightwise = false; }

        switch (rightwise, facingDirection)
        {
            case (true, Direction.north): TurnToFace(Direction.east); break;
            case (true, Direction.east): TurnToFace(Direction.south); break;
            case (true, Direction.south): TurnToFace(Direction.west); break;
            case (true, Direction.west): TurnToFace(Direction.north); break;

            case (false, Direction.north): TurnToFace(Direction.west); break;
            case (false, Direction.west): TurnToFace(Direction.south); break;
            case (false, Direction.south): TurnToFace(Direction.east); break;
            case (false, Direction.east): TurnToFace(Direction.north); break;
        }
    }

    private void TurnToFace(Direction nextFaceDirection)
    {
        Direction oldDirection = facingDirection;
        facingDirection = nextFaceDirection;

        // Turn camera towards new direction

        switch (facingDirection)
        {
            case (Direction.north): Turn(Vector3.forward); break;
            case (Direction.west): Turn(Vector3.left); break;
            case (Direction.east): Turn(Vector3.right); break;
            case (Direction.south): Turn(Vector3.back); break;
        }
    }

    private void Turn(Vector3 facingVector)
    {
        //Player.transform.LookAt(facingVector);
        //float currentAngle
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }
        turnCoroutine = TurningCoroutine(3f, facingVector);
        StartCoroutine(turnCoroutine);
    }

    IEnumerator TurningCoroutine(float timeToTurn, Vector3 wantedDir)
    {
        float increment = 1 / (timeToTurn * 10f);
        WaitForSeconds wait = new WaitForSeconds(increment);
        float progress = 0f;

        Vector3 lookDir = Player.transform.forward;

        while (progress < 1f)
        {
            progress += increment;

            lookDir = Vector3.Lerp(lookDir, wantedDir, progress);
            
            Player.transform.LookAt(lookDir.normalized);

            yield return wait;
        }
    }
    #endregion
}
