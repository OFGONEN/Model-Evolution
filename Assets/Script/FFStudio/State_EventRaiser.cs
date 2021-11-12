using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class State_EventRaiser : StateMachineBehaviour
	{
		public GameEvent onEnter_Event;
		public GameEvent onExit_Event;

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
		{
			onEnter_Event?.Raise();
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		// override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
		// {
		// }

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onExit_Event?.Raise();
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
}