using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExplorer : MonoBehaviour
{
    #region Properties
    public Direction currentFacingDirection = Direction.north;

    Vector3 currentDesiredLocation = Vector3.zero;
    Vector3 currentDesiredRotation = Vector3.forward;

    [SerializeField]
    GameObject Player;

    IEnumerator turnCoroutine;
    IEnumerator moveCoroutine;
    bool moving = false;
    #endregion

    #region Setup

    #endregion

    #region Functions
    public void Explore(Vector3 moveVector)
    {
        float value = moveVector.z;
        Vector3 actualMoveVector = Vector3.zero;
        switch (currentFacingDirection)
        {
            case (Direction.north): actualMoveVector += Vector3.forward * value; break;
            case (Direction.west): actualMoveVector += Vector3.left * value; break;
            case (Direction.east): actualMoveVector += Vector3.right * value; break;
            case (Direction.south): actualMoveVector += Vector3.back * value; break;
        }

        RoomManager.Instance.TryChangeRoom2(actualMoveVector);
    }

    public void Explore2(Vector3 moveVector)
    {
        float value = moveVector.z;
        Vector3 actualMoveVector = Vector3.zero;
        switch (currentFacingDirection)
        {
            case (Direction.north): actualMoveVector += Vector3.forward * value; break;
            case (Direction.west): actualMoveVector += Vector3.left * value; break;
            case (Direction.east): actualMoveVector += Vector3.right * value; break;
            case (Direction.south): actualMoveVector += Vector3.back * value; break;
        }

        if (RoomManager.Instance.TryChangeRoom2(actualMoveVector))
        {
            MovePlayer(actualMoveVector);
        }
    }

    private void MovePlayer(Vector3 moveVector)
    {
        if (moving) { return; }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MovingCoroutine(4f, moveVector);
        StartCoroutine(moveCoroutine);
    }

    IEnumerator MovingCoroutine(float timeToMove, Vector3 wantedDir)
    {
        //moving = true;

        float increment = 1 / (timeToMove * 10f);
        WaitForSeconds wait = new WaitForSeconds(increment);
        float progress = 0f;

        Vector3 beginPos = Player.transform.position;
        Vector3 wantedPos = GetRoundedPos2(beginPos + (wantedDir * WorldStats.Instance.Scale));

        while (progress < 1f)
        {
            progress += increment;

            Player.transform.position = Vector3.Lerp(beginPos, wantedPos, progress);

            yield return wait;
        }

        //Debug.Log("Moving has ended");
        //moving = false;
    }

    public void TurnFacingDirection(Vector3 turnVector)
    {
        bool rightwise = true;
        if (turnVector == Vector3.left) { rightwise = false; }

        switch (rightwise, currentFacingDirection)
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
        currentFacingDirection = nextFaceDirection;

        // Turn camera towards new direction

        switch (currentFacingDirection)
        {
            case (Direction.north): currentDesiredRotation = Vector3.forward; Turn(Vector3.forward); break;
            case (Direction.west): currentDesiredRotation = Vector3.left; Turn(Vector3.left); break;
            case (Direction.east): currentDesiredRotation = Vector3.right; Turn(Vector3.right); break;
            case (Direction.south): currentDesiredRotation = Vector3.back; Turn(Vector3.back); break;
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
        //Vector3 lookDir = DirectionToVector(currentFacingDirection);

        while (progress < 1f)
        {
            progress += increment;

            lookDir = Vector3.Lerp(lookDir, wantedDir, progress);

            Player.transform.LookAt(lookDir.normalized + Player.transform.position);

            yield return wait;
        }
        //Debug.Log("Turning has ended");
    }

    private Vector3 GetRoundedPos(Vector3 currentPos)
    {
        float x = currentPos.x;
        float y = currentPos.z;

        float scale = WorldStats.Instance.Scale;

        Debug.Log("X before:" + x);
        Debug.Log("X rounded:" + Mathf.Round(x / scale));

        // Round them to fit the World Scale
        x = Mathf.Round(x / scale) * scale;
        y = Mathf.Round(y / scale) * scale;
        Debug.Log("X after:" + x);

        return (Vector3.left * x) + (Vector3.forward * y);
    }

    private Vector3 GetRoundedPos2(Vector3 currentPos)
    {
        float x = currentPos.x;
        float y = currentPos.z;

        float scale = WorldStats.Instance.Scale;

        x = Mathf.Round(x / scale) * scale;
        y = Mathf.Round(y / scale) * scale;

        Vector3 roundedPos = new Vector3(x, 0f, y);

        Debug.Log(roundedPos.ToString());

        return roundedPos;
    }

    private Vector3 DirectionToVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.north: return Vector3.forward;
            case Direction.west: return Vector3.left;
            case Direction.east: return Vector3.right;
            case Direction.south: return Vector3.back;
            default: return Vector3.zero;
        }
    }
    #endregion
}
