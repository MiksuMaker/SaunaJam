using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExplorer : MonoBehaviour
{
    #region Properties
    public delegate void MovementTick();
    public MovementTick moveTick;

    public Orientation currentOrientation = Orientation.north;

    [SerializeField]
    float timeToMoveBetweenRooms = 2f;
    [SerializeField]
    float turningTime = 2f;

    [SerializeField]
    GameObject Player;
    ItemHandler itemHandler;
    CameraHandler cameraHandler;

    IEnumerator turnCoroutine;
    IEnumerator moveCoroutine;

    #endregion

    #region Setup
    private void Awake()
    {
        itemHandler = FindObjectOfType<ItemHandler>();
        cameraHandler = Player.GetComponentInChildren<CameraHandler>();
    }
    #endregion

    #region Functions
    public void Explore(Vector3 moveVector)
    {
        float value = moveVector.z;
        Vector3 actualMoveVector = Vector3.zero;
        switch (currentOrientation)
        {
            case (Orientation.north): actualMoveVector += Vector3.forward * value; break;
            case (Orientation.west): actualMoveVector += Vector3.left * value; break;
            case (Orientation.east): actualMoveVector += Vector3.right * value; break;
            case (Orientation.south): actualMoveVector += Vector3.back * value; break;
        }

        RoomManager.Instance.TryChangeRoom2(actualMoveVector);
    }

    public void Explore2(Vector3 moveVector)
    {
        float value = moveVector.z;
        Vector3 actualMoveVector = Vector3.zero;
        switch (currentOrientation)
        {
            case (Orientation.north): actualMoveVector += Vector3.forward * value; break;
            case (Orientation.west): actualMoveVector += Vector3.left * value; break;
            case (Orientation.east): actualMoveVector += Vector3.right * value; break;
            case (Orientation.south): actualMoveVector += Vector3.back * value; break;
        }

        if (RoomManager.Instance.TryChangeRoom2(actualMoveVector))
        {
            MovePlayer(actualMoveVector);
            OrientCamera(false);
        }
        else
        {
            // Replaced by Interact()
        }
    }

    public void Interact()
    {
        if (ItemManager.Instance.TestForItem(RoomManager.Instance.currentRoom, currentOrientation))
        {
            itemHandler.InteractWithItem(ItemManager.Instance.GetItemManifest(RoomManager.Instance.currentRoom, currentOrientation));
        }
    }

    private void MovePlayer(Vector3 moveVector)
    {
        // Call delegate
        moveTick();

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MovingCoroutine(timeToMoveBetweenRooms, moveVector);
        StartCoroutine(moveCoroutine);
    }

    IEnumerator MovingCoroutine(float timeToMove, Vector3 wantedDir)
    {
        #region New Version

        Vector3 beginPos = Player.transform.position;
        Vector3 wantedPos = GetRoundedPos(beginPos + (wantedDir * WorldStats.Instance.Scale));

        float progress = 0f;
        float increment = 0.01f;
        while (progress < timeToMove)
        {
            progress += increment;

            //Player.transform.position = Vector3.Lerp(beginPos, wantedPos, (timeElapsed / timeToMove));
            //Player.transform.position = Vector3.Lerp(beginPos, wantedPos, progress);
            //Player.transform.position = Vector3.Lerp(Player.transform.position, wantedPos, progress);
            Player.transform.position = Vector3.Lerp(Player.transform.position, wantedPos, (progress / timeToMove));

            yield return new WaitForSeconds(increment);
        }
        Player.transform.position = wantedPos;
        #endregion
    }

    public void TurnFacingDirection(Vector3 turnVector)
    {
        bool rightwise = true;
        if (turnVector == Vector3.left) { rightwise = false; }

        switch (rightwise, currentOrientation)
        {
            case (true, Orientation.north): TurnToFace(Orientation.east); break;
            case (true, Orientation.west): TurnToFace(Orientation.north); break;
            case (true, Orientation.east): TurnToFace(Orientation.south); break;
            case (true, Orientation.south): TurnToFace(Orientation.west); break;

            case (false, Orientation.north): TurnToFace(Orientation.west); break;
            case (false, Orientation.west): TurnToFace(Orientation.south); break;
            case (false, Orientation.east): TurnToFace(Orientation.north); break;
            case (false, Orientation.south): TurnToFace(Orientation.east); break;
        }
    }

    private void TurnToFace(Orientation nextFaceDirection)
    {
        currentOrientation = nextFaceDirection;

        // Turn camera towards new direction

        switch (currentOrientation)
        {
            case (Orientation.north): Turn(Vector3.forward); break;
            case (Orientation.west): Turn(Vector3.left); break;
            case (Orientation.east): Turn(Vector3.right); break;
            case (Orientation.south): Turn(Vector3.back); break;
        }
    }

    private void Turn(Vector3 facingVector)
    {
        OrientCamera(true);

        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }
        turnCoroutine = TurningCoroutine(turningTime, facingVector);
        StartCoroutine(turnCoroutine);
    }

    IEnumerator TurningCoroutine(float timeToTurn, Vector3 wantedDir)
    {
        float progress = 0f;
        float increment = 0.01f;

        Vector3 lookDir = Player.transform.forward;

        while (progress < timeToTurn)
        {
            //progress += Time.deltaTime;
            progress += increment;

            lookDir = Vector3.Lerp(lookDir, wantedDir, (progress / timeToTurn));

            Player.transform.LookAt(lookDir.normalized + Player.transform.position);

            yield return new WaitForSeconds(increment);
        }

    }

    void OrientCamera(bool turning)
    {
        // Check if there is an item
        if (ItemManager.Instance.CheckIfOccupiedByItem(RoomManager.Instance.currentRoom, currentOrientation))
        {
            // Check type
            Item.Type type = ItemManager.Instance.CheckItemType(RoomManager.Instance.currentRoom, currentOrientation);

            // Orient camera if needed
            cameraHandler.CheckCameraTilt(type, turning);
        }
        else
        {
            // No item! Orient normally
            cameraHandler.TiltCamera(Vector3.zero, turning);
        }
    }

    private Vector3 GetRoundedPos(Vector3 currentPos)
    {
        float x = currentPos.x;
        float y = currentPos.z;

        float scale = WorldStats.Instance.Scale;

        x = Mathf.Round(x / scale) * scale;
        y = Mathf.Round(y / scale) * scale;

        Vector3 roundedPos = new Vector3(x, 0f, y);

        //Debug.Log(roundedPos.ToString());

        return roundedPos;
    }

    public Vector3 OrientationToVector(Orientation dir)
    {
        switch (dir)
        {
            case Orientation.north: return Vector3.forward;
            case Orientation.west: return Vector3.left;
            case Orientation.east: return Vector3.right;
            case Orientation.south: return Vector3.back;
            default: return Vector3.zero;
        }
    }
    #endregion
}
