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
    [SerializeField] public int current_hydrationLevel;
    [Header("DE-Hydration")]
    [Range(0f, 1f)]
    [SerializeField] float warningPercentage = 0.25f;
    [SerializeField] int ticksPerHydrationDrop = 4;
    int currentTicksLeft;
    [Header("RE-Hydration")]
    [SerializeField] int hydrationPerDrink = 4;
    #endregion

    #region Setup
    private void Start()
    {
        SetupDrinker();
    }

    private void SetupDrinker()
    {
        current_hydrationLevel = START_hydrationLevel;
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
        if (current_hydrationLevel > 0)
        { current_hydrationLevel--; }

        // Check for effects
        HandleDehydration();
    }

    private void HandleDehydration()
    {
        // Check if Hydration is below warning percentage
        if (warningPercentage >= ((float)current_hydrationLevel / (float)MAX_hydrationLevel))
        {
            Debug.Log("Warning%: " + warningPercentage + ", value: " + ((float)current_hydrationLevel / (float)MAX_hydrationLevel));

            // Getting dehydrated!
            if (debugOn) { Debug.Log("Getting thirsty.."); }


            // Do some UI depending on how dehydrated you are
            HandleDehydrationEffects();


            // Check if opposite of drowning has happened
            if (current_hydrationLevel <= 0)
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
        current_hydrationLevel += hydrationPerDrink;

        // Check that hydration doesn't go too high
        current_hydrationLevel = Mathf.Min(current_hydrationLevel, MAX_hydrationLevel);

        if (debugOn) { Debug.Log("Hydration is now at " + current_hydrationLevel); }
    }
    #endregion

    #region EFFECTS
    private void HandleDehydrationEffects()
    {
        // Do some heat up effect according to how severe the Thirst is
    }
    #endregion
}
