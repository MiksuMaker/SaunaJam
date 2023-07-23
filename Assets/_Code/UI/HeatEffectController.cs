using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatEffectController : MonoBehaviour
{
    #region Properties
    [SerializeField] float heatSlideTime = 2f;
    [SerializeField] Image heatImage;

    IEnumerator heatAdjuster;

    [SerializeField] Color baseColor;
    [SerializeField] float MAX_Alpha = 120f;
    #endregion

    #region Setup
    
    #endregion

    #region Functions
    public void AdjustHeatImage(float percentage)
    {
        if (heatAdjuster != null)
        {
            StopCoroutine(heatAdjuster);
        }
        heatAdjuster = Adjuster(MAX_Alpha * percentage);
        StartCoroutine(heatAdjuster);
    }

    IEnumerator Adjuster(float newAlpha)
    {
        if (newAlpha == heatImage.color.a) { yield break; }

        float passedTime = 0f;

        float difference = newAlpha - heatImage.color.a;

        Color newColor = heatImage.color;

        //float startAlpha = heatImage.color.a / 255;
        float startAlpha = heatImage.color.a;
        //Debug.Log("Alpha Original: " + heatImage.color.a + ", Alpha Value: " + startAlpha);
        float goalAlpha = (heatImage.color.a + difference) / 255;
        float nextAlpha;

        //Debug.Log("Start: " + startAlpha + ", Goal: " + goalAlpha + ", Difference: " + difference);

        while (passedTime < heatSlideTime)
        {
            passedTime += Time.deltaTime;

            // Slide
            nextAlpha = Mathf.Lerp(startAlpha, goalAlpha, (passedTime / heatSlideTime));


            //Debug.Log("Next Alpha: " + nextAlpha);
            newColor = new Color(heatImage.color.r, heatImage.color.g, heatImage.color.b, nextAlpha);
            //newColor = new Color32((byte)heatImage.color.r, (byte)heatImage.color.g, (byte)heatImage.color.b, (byte)(nextAlpha / 100));
            heatImage.color = newColor;

            // Wait
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Finish
        newColor = new Color(heatImage.color.r, heatImage.color.g, heatImage.color.b, goalAlpha);
        heatImage.color = newColor;
    }
    #endregion
}
