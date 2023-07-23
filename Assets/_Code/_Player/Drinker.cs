using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drinker : MonoBehaviour
{
    #region Properties
    [SerializeField] bool debugOn = false;

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

        // Subscribe to delegates
        Collector.Instance.waterCollected += IncreaseHydration;
        GetComponent<RoomExplorer>().moveTick += TickHydration;
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
        if (hydrationLevel > 0)
        { hydrationLevel--; }

        // Check for effects
        HandleDehydration();
    }

    private void HandleDehydration()
    {
        // Check if Hydration is below half
        if (hydrationLevel < (MAX_hydrationLevel / 4))
        {
            // Getting dehydrated!
            if (debugOn) { Debug.Log("Getting thirsty.."); }


            // Do some UI depending on how dehydrated you are
            HandleDehydrationEffects();


            // Check if opposite of drowning has happened
            if (hydrationLevel <= 0)
            {
                // You have died of thirst x_x
                if (debugOn) { Debug.Log("You have died of thirst! x_x"); }

                // Die();
            }
        }
    }
    #endregion

    #region HYDRATE
    public void IncreaseHydration()
    {
        hydrationLevel += hydrationPerDrink;

        // Check that hydration doesn't go too high
        hydrationLevel = Mathf.Min(hydrationLevel, MAX_hydrationLevel);

        if (debugOn) { Debug.Log("Hydration is now at " + hydrationLevel); }
    }
    #endregion

    #region EFFECTS
    private void HandleDehydrationEffects()
    {
        // Do some heat up effect according to how severe the Thirst is
    }
    #endregion
}
