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
    public int woodAmountCollected = 0;
    public int stoneAmountCollected = 0;
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

        // Inform Player if enough wood has been collected
        if (woodAmountCollected + SaunaManager.Instance.givenWoodLogs == SaunaManager.Instance.neededWoodLogs + 1)
        {
            UIText[] texts = new UIText[]
            {
                new UIText("Yes", 0.3f, 1f, 0.4f),
                new UIText("Bring the timber to me", 0.3f, 1f, 1f),
            };

            UI_Controller.Instance.FlashTextOnScreen(texts);
        }
    }

    private void CollectStone()
    {
        stoneAmountCollected++;
        Debug.Log("Stone amount: " + stoneAmountCollected);

        //stoneCollected?.Invoke();

        // Call to Enemy Mover to teleport any present gnomes away
        EnemyManager.Instance.GetRidOfGnome();
    }
    #endregion
}
