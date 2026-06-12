using UnityEngine;

public class IntroStateEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Boss boss = animator.GetComponent<Boss>();
        if (boss != null)
        {
            boss.isIntroFinished = true;
        }
    }
}