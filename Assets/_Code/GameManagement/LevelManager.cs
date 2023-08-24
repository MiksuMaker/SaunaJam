using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;


    public List<Level> levels = new List<Level>();
    public int currentLevel = 0;

    #region Setup
    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }


    #endregion
    public Level GetCurrentLevel()
    {
        // Check that level is found
        if (currentLevel >= levels.Count) { Debug.Log("No preferences for level number " + currentLevel); return null; }

        // Return that level
        return levels[currentLevel];
    }

    public void UpdateCurrentLevel(int indexOverrideModifier = 1)
    {
        currentLevel += indexOverrideModifier;

        if (levels.Count <= currentLevel)
        {
            currentLevel = levels.Count - 1;
        }
        else if (currentLevel < 0)
        {
            currentLevel = 0;
        }
    }

    public bool IsThisLastLevel()
    {
        Debug.Log("Current level: " + currentLevel);
        Debug.Log("Amount of levels: " + levels.Count);

        if (levels.Count == currentLevel + 1)
        {
            // End the Game
            return true;
        }
        else
            return false;
    }
}
