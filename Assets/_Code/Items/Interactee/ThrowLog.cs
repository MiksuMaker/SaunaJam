using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowLog : MonoBehaviour
{
    #region Properties
    [SerializeField]
    Animator animator;

    string basic = "ThrowLogAnimation_";
    #endregion

    #region Setup
    #endregion

    #region Functions
    public void DoThrowLogAnimation(int num)
    {
        Debug.Log("Starting animation!");
        StartCoroutine(Throw(num));
    }

    IEnumerator Throw(int num)
    {
        float waitTime;
        string name = basic;

        if (num == 1)
        {
            name += "1";
        }
        else
        {
            name += "2";
        }

        animator.Play(name);
        animator.Update(0);
        waitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        // Wait
        yield return new WaitForSeconds(waitTime);

        // Destroy
        Destroy(gameObject);
    }
    #endregion
}
