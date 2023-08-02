using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    #region Properties
    GameObject graphics;
    #endregion

    #region Setup
    public void SetupItemPickUp(ItemManifest manifest)
    {
        transform.parent = WorldStats.Instance.Player.transform;

        // Move ItemPickup to manifest location
        transform.position = manifest.go.transform.position;

        // Get the possession of the item graphics
        graphics = manifest.graphicsGO;
        graphics.transform.parent = transform;

        // Move them towards Player
        StartCoroutine(MoveGraphics());

        // Destroy this
        // -> Handled in the coroutine
    }
    #endregion

    #region Functions
    IEnumerator MoveGraphics()
    {
        // Move the graphics towards the Player point
        Vector3 origin = graphics.transform.position;

        Vector3 offset = Vector3.up * 0.2f;
        //Vector3 behindPos = (origin - transform.position).normalized;
        Vector3 behindPos = (transform.position - origin).normalized * 0.5f;
        //Vector3 behindPos = Vector3.zero;
        Vector3 destination = transform.position + offset + behindPos;
        Vector3 path = (destination - origin);

        // Random Rotation
        Quaternion ogRot = graphics.transform.rotation;
        Quaternion desiredRot = Quaternion.Euler(ogRot.x + Random.Range(-90f, 90f),
                                                ogRot.y + Random.Range(-90f, 90f),
                                                ogRot.z + Random.Range(-90f, 90f));

        // Scale
        Vector3 ogScale = graphics.transform.localScale;

        float timeSpent = 0f;
        float pickupTime = 0.4f;

        while (timeSpent < pickupTime)
        {
            float progress = (timeSpent / pickupTime);

            // Move the item towards destination
            graphics.transform.position = origin + (path * Easing.EaseInOutBack(progress));

            // Rotate
            //graphics.transform.rotation = Quaternion.Lerp(ogRot, desiredRot, Easing.EaseInOutExpo(progress));
            graphics.transform.rotation = Quaternion.Lerp(ogRot, desiredRot, Easing.EaseInOutBackExpoHybrid(progress));

            // Scale
            graphics.transform.localScale = Vector3.Lerp(ogScale, Vector3.zero, Easing.EaseInExpo(progress));

            // Increase time
            // Wait

            //timeSpent += Time.deltaTime;
            //yield return new WaitForSeconds(Time.deltaTime);

            //timeSpent += Time.fixedDeltaTime;
            //yield return new WaitForSeconds(Time.fixedDeltaTime);

            timeSpent += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        // Destroy item
        Destroy(gameObject);
    }
    #endregion
}
