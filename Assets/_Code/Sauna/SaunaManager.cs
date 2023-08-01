using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaManager : MonoBehaviour
{
    #region Properties
    static public SaunaManager Instance;
    public SaunaGraphicsController saunaGraphics;

    [Header("Sacrificed Resources")]
    [SerializeField] int givenWoodLogs = 0;
    [SerializeField] int givenSaunaStones = 0;

    [Header("Needed Resources")]
    [SerializeField] int neededWoodLogs = 1;
    //[SerializeField] int neededSaunaStones = 1;


    #endregion

    #region Setup
    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        saunaGraphics = GetComponent<SaunaGraphicsController>();
    }
    #endregion

    #region SACRIFICES
    public void TrySacrificeItems()
    {
        Collector c = Collector.Instance;

        bool sacrificeSuccess = false;

        // Sacrifice WOOD
        if (c.woodAmountCollected != 0)
        {
            SacrificeWood(c.woodAmountCollected);

            // ANIMATE
            StartCoroutine(SacrificeAnimator(c.woodAmountCollected));

            // Use all
            c.woodAmountCollected = 0;
            sacrificeSuccess = true;
        }
        // Sacrifice STONE
        if (c.stoneAmountCollected != 0)
        {
            SacrificeStones(c.stoneAmountCollected);
            c.stoneAmountCollected = 0;
            sacrificeSuccess = true;
        }

        // Check if anything got sacrificed
        if (!sacrificeSuccess)
        {
            // Prompt Player that they need to collect stuff
            Debug.Log("No items to sacrifice!");
        }
    }

    private void SacrificeWood(int amount)
    {
        givenWoodLogs += amount;

        Debug.Log("Sacrificed " + amount + " pieces of wood!");
    }
    private void SacrificeStones(int amount)
    {
        givenSaunaStones += amount;

        Debug.Log("Sacrificed " + amount + " sauna stones!");
    }
    #endregion

    #region ANIMATIONS
    public void UpdateSaunaManifestPosition(Vector3 roomPos, Vector3 saunaPos)
    {
        saunaGraphics.saunaRoomPos = roomPos;
        saunaGraphics.saunaManifestPos = saunaPos;
    }

    public void ConnectSaunaGraphics(SaunaManifest saunaManifest)
    {
        saunaGraphics.ConnectSaunaGraphics(saunaManifest);

        UpdateSaunaManifestPosition(saunaManifest.transform.position, saunaManifest.graphicsGO.transform.position);
    }

    private IEnumerator SacrificeAnimator(int amount)
    {
        // Initiate the animationWaitTime
        float wait = 1f;

        // First, open up the Sauna
        saunaGraphics.DoAnimation(SaunaGraphicsController.SaunaAnimationState.open, out wait);

        // Wait for long as that animation takes
        yield return new WaitForSeconds(wait);

        // Next, play animation for every sacrificed log
        for (int i = 0; i < amount; i++)
        {
            // Vary the animation
            if (i % 2 == 0)
            {
                saunaGraphics.ThrowLog(2);
                yield return new WaitForSeconds(0.2f);
                saunaGraphics.DoAnimation(SaunaGraphicsController.SaunaAnimationState.feed_1, out wait);   
            }
            else
            {
                saunaGraphics.ThrowLog(1);
                yield return new WaitForSeconds(0.3f);
                saunaGraphics.DoAnimation(SaunaGraphicsController.SaunaAnimationState.feed_2, out wait);   
            }

            // Wait
            yield return new WaitForSeconds(wait);
        }

        // Close the Sauna
        saunaGraphics.DoAnimation(SaunaGraphicsController.SaunaAnimationState.close, out wait);

        yield return new WaitForSeconds(wait);
    }
    #endregion
}
