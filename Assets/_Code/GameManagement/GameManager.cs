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
    }

    public void EndGame(float endTime)
    {
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
                                            new UIText("How disappointing", 0.2f, 1f, 1f),};

                UI_Controller.Instance.FlashTextOnScreen(texts);
                break;

            case Reason.gnomeAttack:

                break;
        }

        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, 1f));
    }

    public enum Reason // For restarting
    {
        dehydration, steamAttack, gnomeAttack,
    }

    IEnumerator LoadScene(int sceneNum, float loadTime)
    {
        float passedTime = 0f;

        // DO THE BEFORE LOAD STUFF HERE

        // Wait for the text to finish
        while (UI_Controller.Instance.textInProcess)
        {
            yield return new WaitForEndOfFrame();
        }

        // FADE
        UI_Controller.Instance.AdjustSteamEffect(1f);

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
