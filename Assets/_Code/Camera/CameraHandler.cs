using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Properties
    public static CameraHandler Instance;

    Transform playerTrans;
    GameObject cameraGO;

    [Header("Rotations")]
    [SerializeField] float rotationTime = 1f;
    [SerializeField] float longerRotationTime = 5f;
    [Header("Normal")]
    //[SerializeField]
    //Vector3 normalPos;
    [SerializeField]
    Vector3 normalRot;

    [Header("Down Tilt")]
    [SerializeField]
    Vector3 downTiltedRot;
    [Header("Lesser Tilt")]
    [SerializeField]
    Vector3 lesserTiltedRot;

    IEnumerator cameraTilter;

    IEnumerator cameraMover;
    IEnumerator cameraTurner;
    bool dying = false;
    bool cameraTurnDenied = false;
    #endregion

    #region Setup
    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else { Destroy(this); }
    }
    private void Start()
    {
        SetupCameraHandler();
    }
    public void SetupCameraHandler()
    {
        playerTrans = transform.parent;
        cameraGO = transform.GetChild(0).gameObject;
    }
    #endregion

    #region Functions
    public void CheckCameraTilt(Item.Type type, bool turning = true)
    {
        if (type == Item.Type.woodLog || type == Item.Type.saunaStone)
        {
            // Tilt the camera
            TiltCamera(downTiltedRot, turning);
        }
        else if (type == Item.Type.water)
        {
            TiltCamera(lesserTiltedRot, turning);
        }
        else
        {
            // Tilt it back to normal pos
            TiltCamera(normalRot, turning);
        }
    }

    public void TiltCamera(Vector3 toRot, bool turning = true)
    {
        if (dying) { return; }

        // Stop the Coroutine
        if (cameraTilter != null) { StopCoroutine(cameraTilter); }

        // Check correct time
        float timeToTilt = rotationTime;
        if (!turning)
        {
            // Faster tilt than when walking straight ahead
            timeToTilt = longerRotationTime;
        }
        cameraTilter = CameraTilter(cameraGO.transform.rotation, toRot, timeToTilt);

        // Start the Coroutine again
        StartCoroutine(cameraTilter);
    }

    IEnumerator CameraTilter(Quaternion startRot, Vector3 toRot, float timeToTilt)
    {
        // Check first if the current rotation is the ToRotation
        if (startRot == Quaternion.Euler(toRot)) { yield break; }

        //float increment = 1 / (timeToTilt * 100f);
        //WaitForSeconds wait = new WaitForSeconds(increment);
        float increment = 0.01f;

        //Quaternion currentRot = Quaternion.Euler(startRot);
        Vector3 r = transform.localRotation.eulerAngles;

        Quaternion finalRot = Quaternion.Euler(toRot.x, toRot.y, toRot.z);

        float progress = 0f;
        //while (progress < 1f)
        while (progress < timeToTilt)
        {
            // Desired Rotation
            //cameraGO.transform.rotation.Set(0f, 0f, 0f, 1f);

            // Turn the camera a bit towards the goal rotation
            cameraGO.transform.localRotation = Quaternion.Lerp(cameraGO.transform.localRotation, finalRot, (progress / timeToTilt));
            //cameraGO.transform.localRotation = Quaternion.Lerp(cameraGO.transform.localRotation, finalRot, progress);

            //Debug.Log(cameraGO.transform.localRotation);

            // Wait
            //progress += Time.deltaTime;
            //yield return new WaitForSeconds(Time.deltaTime);

            progress += increment;
            yield return new WaitForSeconds(increment);
        }

        // Finish
        cameraGO.transform.localRotation = Quaternion.Euler(toRot.x, toRot.y, toRot.z);
    }

    public class CamTurn
    {
        public Vector3 endRot;
        public float timeToTurn;

        public CamTurn(Vector3 end, float time)
        {
            endRot = end; timeToTurn = time;
        }
    }

    private void TurnCamera(CamTurn[] turns, bool overridden = false)
    {
        if (cameraTurnDenied) { if (!overridden) { return; } }

        if (cameraTurner != null) { StopCoroutine(cameraTurner); }
        cameraTurner = CameraTurner(turns);
        StartCoroutine(cameraTurner);
    }

    IEnumerator CameraTurner(CamTurn[] turns)
    {
        float passedTime;
        float increment = 0.01f;
        WaitForSeconds wait = new WaitForSeconds(increment);
        //Vector3 nextRot;
        Quaternion nextRot;

        // Go through every move
        foreach (var m in turns)
        {
            Vector3 startVector = cameraGO.transform.localRotation.eulerAngles;
            //startVector = m.startRot == Vector3.zero ? transform.localPosition : m.startRot;

            passedTime = 0f;

            while (passedTime < m.timeToTurn)
            {
                passedTime += increment;
                //nextRot = Vector3.Lerp(startVector, m.endRot, Easing.EaseInExpo(passedTime / m.timeToTurn));
                nextRot = Quaternion.Lerp(Quaternion.Euler(startVector), Quaternion.Euler(m.endRot), Easing.EaseInExpo(passedTime / m.timeToTurn));
                //gameObject.transform.localPosition = nextPos;
                gameObject.transform.localRotation = nextRot;
                yield return wait;
            }

            // Finalize
            gameObject.transform.localRotation = Quaternion.Euler(m.endRot);
        }
    }
    #endregion

    #region Moving Camera

    private void MoveCamera(CamMove[] moves)
    {
        if (cameraMover != null) { StopCoroutine(cameraMover); }
        cameraMover = CameraMover(moves);
        StartCoroutine(cameraMover);
    }

    IEnumerator CameraMover(CamMove[] moves)
    {
        float passedTime;
        float increment = 0.01f;
        WaitForSeconds wait = new WaitForSeconds(increment);
        Vector3 nextPos;

        // Go through every move
        foreach (var m in moves)
        {
            Vector3 startVector;
            //startVector = m.startPos == Vector3.zero ? transform.localPosition : m.startPos;
            startVector = transform.position;

            passedTime = 0f;

            while (passedTime < m.timeToMove)
            {
                passedTime += increment;
                nextPos = Vector3.Lerp(startVector, m.endPos, Easing.EaseInExpo(passedTime / m.timeToMove));
                gameObject.transform.localPosition = nextPos;
                yield return wait;
            }

            // Finalize
            gameObject.transform.localPosition = m.endPos;
        }
    }

    public class CamMove
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public float timeToMove;

        public CamMove(Vector3 start, Vector3 end, float time)
        {
            startPos = start; endPos = end; timeToMove = time;
        }
    }
    #endregion

    #region Dying
    public void DoDehydrateAnimation()
    {
        // Moves
        CamMove[] moves = new CamMove[] {
                                            new CamMove(Vector3.zero, new Vector3(0f, 1f, -.7f), 1f),
                                            //new CamMove(Vector3.zero, new Vector3(0f, 1f, 0f), 2f),
                                            new CamMove(Vector3.zero, new Vector3(0f, 0.2f, 0.5f), 1f)
        };
        MoveCamera(moves);

        // Turns
        CamTurn[] turns = new CamTurn[]
        {
            //new CamTurn(new Vector3(-7f, 0f, 0f), .8f),
            //new CamTurn(new Vector3(3f, 0f, -5f), 0.5f),
            //new CamTurn(new Vector3(-35f, 60f, 60f), 0.7f),
            new CamTurn(new Vector3(-35f, 60f, 60f), 2f),
        };
        TurnCamera(turns);
        //transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -70f));
        dying = true;
    }

    public void DoGnomeDeathAnimation(Orientation gnomeOrientation)
    {
        #region Attempt 1
        //cameraTurnDenied = true;

        //Vector3 lookPos = Vector3.zero;
        //Vector3 bePos = Vector3.zero;

        //switch (gnomeOrientation)
        //{
        //    case Orientation.north:
        //        lookPos += -Vector3.forward;
        //        break;
        //    case Orientation.west:
        //        lookPos += -Vector3.left;
        //        break;
        //    case Orientation.east:
        //        lookPos += -Vector3.right;
        //        break;
        //    case Orientation.south:
        //        lookPos += -Vector3.back;
        //        break;
        //}

        ////bePos += lookPos;
        //lookPos *= 1f;
        //lookPos += cameraGO.transform.position;
        //bePos += cameraGO.transform.localPosition + Vector3.up;

        //Debug.Log("LookPos: " + lookPos.ToString());
        //Debug.Log("OG Pos: " + transform.position.ToString());
        //Debug.DrawLine(lookPos, lookPos + Vector3.up, Color.red, 50f);

        //// Turn towards that position
        //Quaternion ogRotation = cameraGO.transform.rotation;
        //cameraGO.transform.LookAt(lookPos);
        //Debug.DrawRay(cameraGO.transform.position, cameraGO.transform.forward, Color.magenta, 50f);
        //Quaternion wantedRotation = cameraGO.transform.rotation;
        ////// Reset
        //cameraGO.transform.rotation = ogRotation;

        ////// Return that rotation to CameraTurner
        //float animationTime = 2f;

        //CamTurn[] turns = new CamTurn[] { new CamTurn(wantedRotation.eulerAngles, animationTime), };
        //TurnCamera(turns, true);

        //CamMove[] moves = new CamMove[] { new CamMove(cameraGO.transform.position, bePos, animationTime), };
        //MoveCamera(moves);

        //TiltCamera(normalRot);
        #endregion


        cameraTurnDenied = true;

        Vector3 lookRot = Vector3.zero;
        Vector3 counterRot = Vector3.zero;

        // Counter according to Player's current Rotation
        switch (RoomExplorer.Instance.currentOrientation)
        {
            case Orientation.north:
                counterRot = new Vector3(0f, 0f, 0f);
                break;
            case Orientation.west:
                counterRot = new Vector3(0f, 90f, 0f);
                break;
            case Orientation.east:
                counterRot = new Vector3(0f, -90f, 0f);
                break;
            case Orientation.south:
                counterRot = new Vector3(0f, 180f, 0f);
                break;
        }

        //Debug.Log("Gnome Orientation: " + gnomeOrientation.ToString());
        switch (gnomeOrientation)
        {
            case Orientation.north:
                lookRot = new Vector3(0f, 180f, 0f);
                break;
            case Orientation.west:
                lookRot = new Vector3(0f, 90f, 0f);
                break;
            case Orientation.east:
                lookRot = new Vector3(0f, -90f, 0f);
                break;
            case Orientation.south:
                lookRot = new Vector3(0f, 180f, 0f);
                break;
        }

        lookRot += counterRot;

        float animationTime = 2f;
        CamTurn[] turns = new CamTurn[] { new CamTurn(lookRot, animationTime), };
        TurnCamera(turns, true);

        Vector3 exPos = transform.position;
        Vector3 bePos = new Vector3(0f, 1f, 0.5f) + exPos;
        CamMove[] moves = new CamMove[] { new CamMove(exPos, bePos, animationTime) };
        //MoveCamera(moves);

        TiltCamera(normalRot);
    }

    #endregion
}
