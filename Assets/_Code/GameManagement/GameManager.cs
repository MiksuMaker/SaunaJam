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
    #endregion

    #region Functions
    public void EndGame(float endTime)
    {
        // Load the Next Scene?
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, endTime));
    }

    public void RestartGame(float time)
    {
        SceneManager.LoadScene(0);
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
