using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteamEffectController : MonoBehaviour
{
    #region Properties
    [SerializeField] float steamSlideTime = 2f;
    [SerializeField] Image steamImage;

    IEnumerator heatAdjuster;

    [SerializeField] Color baseColor;
    [SerializeField] float MAX_Alpha = 120f;
    #endregion

    #region Setup
    private void Start()
    {
        AdjustSteamImage(0f);
    }

    [ContextMenu("Off Steam")]
    private void OffSteam()
    {
        AdjustSteamImage(0f);
    }

    [ContextMenu("On Steam")]
    private void OnSteam()
    {
        AdjustSteamImage(1f);
    }
    #endregion

    #region Functions
    public void AdjustSteamImage(float percentage, float fadeTime = 2f) // Percentage is a value between 0 and 1
    {
        if (heatAdjuster != null)
        {
            StopCoroutine(heatAdjuster);
        }
        heatAdjuster = Adjuster(MAX_Alpha * percentage, fadeTime);
        StartCoroutine(heatAdjuster);
    }

    IEnumerator Adjuster(float newAlpha, float steamTime = 2f)
    {
        if (newAlpha == steamImage.color.a) { yield break; }

        float passedTime = 0f;

        float difference = newAlpha - steamImage.color.a;

        Color newColor = steamImage.color;

        //float startAlpha = heatImage.color.a / 255;
        float startAlpha = steamImage.color.a;
        //Debug.Log("Alpha Original: " + heatImage.color.a + ", Alpha Value: " + startAlpha);
        float goalAlpha = (steamImage.color.a + difference) / 255;
        float nextAlpha;


        // You need to wait for one frame when starting
        // with a COntextMenu action, otherwise the deltaTime will be 0.3333... seconds for some reason
        yield return new WaitForSeconds(Time.deltaTime); 

        while (passedTime < steamTime)
        {

            // Slide
            //nextAlpha = Mathf.Lerp(startAlpha, goalAlpha, (passedTime / steamSlideTime));
            nextAlpha = Mathf.Lerp(startAlpha, goalAlpha, Easing.EaseOutQuart(passedTime / steamSlideTime));
            //nextAlpha = Mathf.Lerp(startAlpha, goalAlpha, Easing.EaseOutQuart(passedTime / heatSlideTime));


            //newColor = new Color(steamImage.color.r, steamImage.color.g, steamImage.color.b, nextAlpha);
            newColor = new Color(steamImage.color.r, steamImage.color.g, steamImage.color.b, nextAlpha);
            steamImage.color = newColor;

            // Wait
            yield return new WaitForSeconds(Time.deltaTime);
            passedTime += Time.deltaTime;
        }

        // Finish
        newColor = new Color(steamImage.color.r, steamImage.color.g, steamImage.color.b, goalAlpha);
        steamImage.color = newColor;
    }
    #endregion
}
