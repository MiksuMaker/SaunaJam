using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Properties
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

    [Header("DownTilt")]
    [SerializeField]
    Vector3 downTiltedRot;

    IEnumerator cameraTilter;
    #endregion

    #region Setup
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
        if (type == Item.Type.water || type == Item.Type.woodLog || type == Item.Type.saunaStone)
        {
            // Tilt the camera
            TiltCamera(false, turning);
        }
        else
        {
            // Tilt it back to normal pos
            TiltCamera(true, turning);
        }
    }

    public void TiltCamera(bool normalTiltOn, bool turning = true)
    {
        // Stop the Coroutine
        if (cameraTilter != null) { StopCoroutine(cameraTilter); }

        // Check correct time
        float timeToTilt = rotationTime;
        if (!turning)
        {
            // Faster tilt than when walking straight ahead
            timeToTilt = longerRotationTime;
        }

        if (normalTiltOn)
        {
            cameraTilter = CameraTilter(cameraGO.transform.rotation, normalRot, timeToTilt);
        }
        else
        {
            cameraTilter = CameraTilter(cameraGO.transform.rotation, downTiltedRot, timeToTilt);
        }

        // Start the Coroutine again
        StartCoroutine(cameraTilter);
    }

    IEnumerator CameraTilter(Quaternion startRot, Vector3 toRot, float timeToTilt)
    {
        // Check first if the current rotation is the ToRotation
        if (startRot == Quaternion.Euler(toRot)) { yield break; }

        // Get the time
        float increment = 1 / (timeToTilt * 100f);
        WaitForSeconds wait = new WaitForSeconds(increment);

        //Quaternion currentRot = Quaternion.Euler(startRot);
        Vector3 r = transform.localRotation.eulerAngles;

        Quaternion finalRot = Quaternion.Euler(toRot.x, r.y, r.z);


        float timeElapsedInSeconds = Time.time;
        float timeWaited = 0f;
        while (timeWaited < timeToTilt)
        {
            // Desired Rotation
            //cameraGO.transform.rotation.Set(0f, 0f, 0f, 1f);

            // Turn the camera a bit towards the goal rotation
            cameraGO.transform.localRotation = Quaternion.Lerp(cameraGO.transform.localRotation, finalRot, (timeWaited / rotationTime));
            //cameraGO.transform.rotation = Quaternion.Lerp(cameraGO.transform.rotation, finalRot, (timeWaited / rotationTime));

            // Wait
            timeWaited += increment;
            yield return wait;
        }

        // Finish
        cameraGO.transform.localRotation = Quaternion.Euler(toRot.x, r.y, r.z);

        timeElapsedInSeconds = Time.time - timeElapsedInSeconds;
        Debug.Log("Actual time elapsed: " + timeElapsedInSeconds);
    }
    #endregion
}
