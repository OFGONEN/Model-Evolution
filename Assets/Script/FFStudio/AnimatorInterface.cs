/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class AnimatorInterface : MonoBehaviour
	{
#region Fields
		[ BoxGroup( "Setup" ), SerializeField ] private Animator animator;
		[ BoxGroup( "Setup" ), SerializeField ] private AnimationParameterData[] parameterDatas;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public void UpdateParameter( int index )
		{
			var data = parameterDatas[ index ];

			switch( data.parameterType )
			{
				case AnimationParameterType.Trigger:
					animator.SetTrigger( data.parameter_name );
					break;
				case AnimationParameterType.Bool:
					animator.SetBool( data.parameter_name, data.parameter_bool );
					break;
				case AnimationParameterType.Int:
					animator.SetInteger( data.parameter_name, data.parameter_int );
					break;
				case AnimationParameterType.Float:
					animator.SetFloat( data.parameter_name, data.parameter_float );
					break;
			}
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}