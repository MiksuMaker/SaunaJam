using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    #region Properties
    static public Collector Instance;

    public delegate void WaterCollectedDelegate();
    public WaterCollectedDelegate waterCollected;
    public delegate void WoodCollectedDelegate();
    public WoodCollectedDelegate woodCollected;
    public delegate void StoneCollectedDelegate();
    public StoneCollectedDelegate stoneCollected;

    int waterAmountCollected = 0;
    int woodAmountCollected = 0;
    int stoneAmountCollected = 0;
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

    #region COLLECTION
    public void CollectItem(Item.Type type)
    {
        switch (type)
        {
            case Item.Type.water: CollectWater(); break;
            case Item.Type.woodLog: CollectWood(); break;
            case Item.Type.saunaStone: CollectStone(); break;
            default: Debug.Log("Not the kind of Item you can collect!"); break;
        }
    }
    #endregion

    #region Announcement
    private void CollectWater()
    {
        waterAmountCollected++;

        //Debug.Log("Water amount: " + waterAmountCollected);

        waterCollected?.Invoke();
    }

    private void CollectWood()
    {
        woodAmountCollected++;
        //Debug.Log("Wood amount: " + woodAmountCollected);

        woodCollected?.Invoke();
    }

    private void CollectStone()
    {
        stoneAmountCollected++;
        Debug.Log("Stone amount: " + stoneAmountCollected);

        stoneCollected?.Invoke();
    }
    #endregion
}
