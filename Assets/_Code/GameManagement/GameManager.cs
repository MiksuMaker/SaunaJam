using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Properties
    static public GameManager Instance;

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

    private void Start()
    {
        // When the Scene is loaded, this Start() function boots up the game
        StartLevel();
    }
    #endregion

    #region Functions
    public void StartLevel()
    {
        // Fetch current level
        Level current = LevelManager.Instance.GetCurrentLevel();

        // Start Generation
        RoomGenerator.Instance.StartRoomGeneration(current.preset, current.preference);

        // Generate Items
        ItemManager.Instance.SpawnItems(current.itemSet);

        // Generate Enemies
        EnemyManager.Instance.SpawnEnemies(current.steamSpawnAmount, current.gnomeSpawnAmount);

        // Setup Sauna
        SaunaManager.Instance.SetupSaunaManager(current.requiredLogs);

        // Manifest it
        RoomManager.Instance.LoadInitialRooms();
    }

    public void LoadNextLevel(float endTime)
    {
        // Update LevelManager
        LevelManager.Instance.UpdateCurrentLevel();

        // Load the Next Scene?
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, endTime));
    }

    public void RestartLevel(float time)
    {
        SceneManager.LoadScene(0);
    }

    public void RestartLevel(float time, Reason reason)
    {
        // Comment on how the Player died
        switch (reason)
        {
            case Reason.dehydration:
                UIText[] texts = new UIText[] { new UIText("", 0.5f, 1f, 1f),
                                            new UIText("Another one succumbs to the heat", 0.5f, 1f, 1f),
                                            //new UIText("How disappointing", 0.2f, 1f, 1f),
                };

                UI_Controller.Instance.FlashTextOnScreen(texts);
                StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, 1f));
                break;

            case Reason.gnomeAttack:
                StartCoroutine(DoGnomeAttack(time));
                break;
        }

    }

    IEnumerator DoGnomeAttack(float timeBeforeFlash = 1f)
    {
        yield return new WaitForSeconds(timeBeforeFlash);

        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, 2f, 0.01f, true));
    }

    public enum Reason // For restarting
    {
        dehydration, steamAttack, gnomeAttack,
    }

    IEnumerator LoadScene(int sceneNum, float loadTime, float fadeTime = 2f, bool forceFade = false)
    {
        float passedTime = 0f;

        // DO THE BEFORE LOAD STUFF HERE

        // Wait for the text to finish
        if (!forceFade)
        {
            while (UI_Controller.Instance.textInProcess)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        // FADE
        UI_Controller.Instance.AdjustSteamEffect(1f, fadeTime);

        // WAIT
        while (passedTime < loadTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            passedTime += Time.deltaTime;
        }



        // Load
        SceneManager.LoadScene(sceneNum);
    }
    #endregion
}
