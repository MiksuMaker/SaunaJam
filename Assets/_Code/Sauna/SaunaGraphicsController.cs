using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaunaGraphicsController : MonoBehaviour
{
    #region Properties
    Animator saunaAnimator;
    Vector3 particlePos;

    public enum SaunaAnimationState
    {
        idle, open, stayOpen, feed_1, feed_2, close,
    }

    string idle = "Idle";
    string open = "Open";
    string stayOpen = "StayOpen";
    string feed_1 = "Feed_1";
    string feed_2 = "Feed_2";
    string close = "Close";
    #endregion

    #region Setup
    public void ConnectSaunaGraphics(SaunaManifest saunaManifest)
    {
        saunaAnimator = saunaManifest.animator;
        particlePos = saunaManifest.graphicsGO.transform.position; // + offset
    }
    #endregion

    #region Functions
    public void DoAnimation(SaunaAnimationState state, out float animationTime)
    {
        if (saunaAnimator == null) { Debug.Log("SaunaAnimator is null"); animationTime = 0f; return; }

        string animation = idle;

        switch (state)
        {
            case SaunaAnimationState.idle:
                animation = idle;
                break;
            case SaunaAnimationState.open:
                animation = open;
                break;
            case SaunaAnimationState.stayOpen:
                animation = stayOpen;
                break;
            case SaunaAnimationState.feed_1:
                animation = feed_1;
                break;
            case SaunaAnimationState.feed_2:
                animation = feed_2;
                break;
            case SaunaAnimationState.close:
                animation = close;
                break;
        }


        // Play Animation and Return animation time
        saunaAnimator.Play(animation);
        saunaAnimator.Update(0);    // The Animator needs to be udpated so that we can get the correct length
                                    // (as the animator wouldn't update otherwise on the same
                                    //  frame, giving .length of the previous animation)

        //Debug.Log(saunaAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation));
        //Debug.Log(animation + " Time: " + saunaAnimator.GetCurrentAnimatorStateInfo(0).length);
        animationTime = saunaAnimator.GetCurrentAnimatorStateInfo(0).length;
    }
    #endregion
}
