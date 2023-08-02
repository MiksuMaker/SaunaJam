using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    #region Properties
    static public UI_Controller Instance;

    HeatEffectController heatEffectController;
    SteamEffectController steamEffectController;

    TextMeshProUGUI textMesh;
    IEnumerator textFader;
    [HideInInspector]
    public bool textInProcess = false;
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

        // Get references
        heatEffectController = GetComponentInChildren<HeatEffectController>();
        steamEffectController = GetComponentInChildren<SteamEffectController>();

        SetupText();
    }
    #endregion

    #region Heat & Steam
    public void AdjustHeatEffect(float percentage)
    {
        heatEffectController.AdjustHeatImage(percentage);
    }

    public void AdjustSteamEffect(float percentage)
    {
        steamEffectController.AdjustSteamImage(percentage);
    }
    #endregion

    #region UI Text
    private void SetupText()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        // Empty it
        textMesh.text = "";

        //UIText[] texts = new UIText[] { new UIText("Hello", 2f, 1f, 1f), 
        //                                new UIText("Remember to <b><color=\"blue\">quench</b><color=\"white\"> your thirst") };
        //FlashTextOnScreen(texts);
    }

    public void FlashTextOnScreen(UIText[] texts)
    {
        if (textFader != null) { StopCoroutine(textFader); textInProcess = false; }
        textFader = TextFader(texts);
        StartCoroutine(textFader);
    }

    

    IEnumerator TextFader(UIText[] texts)
    {
        float passedTime;
        float nextAlpha;

        textInProcess = true;

        Color blank = new Color(1f, 1f, 1f, 0f);

        // Go through every text element
        foreach (var t in texts)
        {
            passedTime = 0f;

            // Change text
            textMesh.text = t.text;

            // Fade in
            textMesh.color = blank;

            while (passedTime < t.fadeInTime)
            {
                nextAlpha = Mathf.Lerp(0f, 1f, (passedTime / t.fadeInTime));
                textMesh.color = new Color(1f, 1f, 1f, nextAlpha);
                passedTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            // Stay On
            passedTime = 0f;
            while (passedTime < t.stayOnTime)
            {
                passedTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            // Fade Out
            passedTime = 0f;
            while (passedTime < t.fadeOutTime)
            {
                nextAlpha = Mathf.Lerp(1f, 0f, (passedTime / t.fadeInTime));
                textMesh.color = new Color(1f, 1f, 1f, nextAlpha);
                passedTime += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            // Finalize
            textMesh.color = new Color(1f, 1f, 1f, 0f);
        }

        textInProcess = false;
    }
    #endregion
}

public class UIText
{
    public string text;
    public float fadeInTime = 1f;
    public float stayOnTime = 2f;
    public float fadeOutTime = 3f;

    public UIText(string text, float fadeIn = 1f, float stayOn = 2f, float fadeOut = 3f)
    {
        this.text = text; fadeInTime = fadeIn; stayOnTime = stayOn; fadeOutTime = fadeOut;
    }
}