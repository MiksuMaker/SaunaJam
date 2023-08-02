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

    [Header("Down Tilt")]
    [SerializeField]
    Vector3 downTiltedRot;
    [Header("Lesser Tilt")]
    [SerializeField]
    Vector3 lesserTiltedRot;

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

        Quaternion finalRot = Quaternion.Euler(toRot.x, r.y, r.z);

        float progress = 0f;
        //while (progress < 1f)
        while (progress < timeToTilt)
        {
            // Desired Rotation
            //cameraGO.transform.rotation.Set(0f, 0f, 0f, 1f);

            // Turn the camera a bit towards the goal rotation
            cameraGO.transform.localRotation = Quaternion.Lerp(cameraGO.transform.localRotation, finalRot, (progress / timeToTilt));
            //cameraGO.transform.localRotation = Quaternion.Lerp(cameraGO.transform.localRotation, finalRot, progress);

            // Wait
            //progress += Time.deltaTime;
            //yield return new WaitForSeconds(Time.deltaTime);

            progress += increment;
            yield return new WaitForSeconds(increment);
        }

        // Finish
        cameraGO.transform.localRotation = Quaternion.Euler(toRot.x, r.y, r.z);
    }
    #endregion
}
