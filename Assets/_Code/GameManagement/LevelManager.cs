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

    public void UpdateCurrentLevel()
    {
        currentLevel++;

        if (levels.Count <= currentLevel)
        {
            currentLevel = levels.Count - 1;
        }
    }
}
