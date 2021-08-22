using UnityEngine;

/// <summary>
/// Makes the sprite lerp between white and black whilst it is in this state.
/// </summary>
public class DodgeBehaviour : StateMachineBehaviour
{
    private SpriteRenderer sprite;
    private float time = 0.0f;
    private static readonly float ANIMATION_SPEED = 0.25f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sprite = animator.GetComponent<SpriteRenderer>();
        time = 0.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;
        sprite.color = Color.Lerp(Color.white, Color.black, (Mathf.Sin(Mathf.Rad2Deg * time * ANIMATION_SPEED) * 0.5f) + 0.5f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sprite.color = Color.white;    
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
