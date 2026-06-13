using UnityEngine;

public class TransformingState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Boss boss = animator.GetComponent<Boss>();
        if (boss != null)
        {
            boss.isTransforming = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Boss boss = animator.GetComponent<Boss>();
        if (boss != null)
        {
            boss.isTransforming = false;
        }
    }
}