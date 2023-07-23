using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    #region Properties
    static public UI_Controller Instance;

    HeatEffectController heatEffectController;
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

        heatEffectController = GetComponentInChildren<HeatEffectController>();
    }
    #endregion

    #region Functions
    public void AdjustHeatEffect(float percentage)
    {
        heatEffectController.AdjustHeatImage(percentage);
    }
    #endregion
}
