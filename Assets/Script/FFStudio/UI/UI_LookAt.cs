/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace FFStudio
{
	public class UI_LookAt : MonoBehaviour
	{
#region Fields
        [ BoxGroup( "Setup" ) ] public SharedReferenceNotifier lookAt_Reference;
        [ BoxGroup( "Setup" ) ] public Vector3 lookAt_Axis;

		// Private
		private Transform lookAt_Transform;
		private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
        private void Awake()
        {
			updateMethod = ExtensionMethods.EmptyMethod;
		}

        private void Start()
        {
			lookAt_Transform = lookAt_Reference.SharedValue as Transform;
			updateMethod     = LookAtTarget;
		}

        private void Update()
        {
			updateMethod();
		}
#endregion

#region API
#endregion

#region Implementation
        private void LookAtTarget()
        {
			transform.LookAtAxis( lookAt_Transform.position, lookAt_Axis, -1f );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}