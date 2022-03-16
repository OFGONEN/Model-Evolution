/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class State_TriggerSetter : StateMachineBehaviour
{
	override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
	{
		animator.SetTrigger( CurrentLevelData.Instance.levelData.cloth_pose );
	}
}