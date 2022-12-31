using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationStatusCheck : StateMachineBehaviour
{
    private string stateChanged = "IsStateChanged";

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(stateChanged, true);
    }
}
