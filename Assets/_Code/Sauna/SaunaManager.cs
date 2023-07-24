using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaManager : MonoBehaviour
{
    #region Properties
    static public SaunaManager Instance;

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
}
