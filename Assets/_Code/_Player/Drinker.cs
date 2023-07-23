using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drinker : MonoBehaviour
{
    #region Properties
    [Header("Hydration Level")]
    [SerializeField] public int START_hydrationLevel = 10;
    [SerializeField] public int MAX_hydrationLevel = 20;
    [Space(10)]
    [SerializeField] public int hydrationLevel;
    [SerializeField] int ticksPerHydrationDrop = 4;
    int currentTicksLeft;
    [Header("Hydration")]
    [SerializeField] int hydrationPerDrink = 4;
    #endregion

    #region Setup
    private void Start()
    {
        SetupDrinker();
    }

    private void SetupDrinker()
    {
        hydrationLevel = START_hydrationLevel;
        currentTicksLeft = ticksPerHydrationDrop;
    }
    #endregion

    #region DEHYDRATE
    public void TickHydration()
    {
        // Reduce ticks
        currentTicksLeft--;

        if (currentTicksLeft <= 0)
        {
            // Reset
            currentTicksLeft = currentTicksLeft + ticksPerHydrationDrop;

            // Reduce Hydration
            ReduceHydration();
        }
    }

    private void ReduceHydration()
    {
        // Reduce Hydration if over zero
        if (hydrationLevel < 0)
        { hydrationLevel--; }

        // Check for effects
        HandleDehydrationEffects();
    }

    private void HandleDehydrationEffects()
    {
        // Check if opposite of drowning has happened
        if (hydrationLevel <= 0)
        {
            // You have died of thirst x_x
            Debug.Log("You have died of thirst!");
        }
    }
    #endregion

    #region HYDRATE
    public void IncreaseHydration()
    {
        hydrationLevel += hydrationPerDrink;

        // Check that hydration doesn't go too high
        hydrationLevel = Mathf.Min(hydrationLevel, MAX_hydrationLevel);
    }
    #endregion
}
