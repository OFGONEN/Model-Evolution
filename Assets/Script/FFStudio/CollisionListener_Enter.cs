/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class CollisionListener_Enter : CollisionListener
	{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
        private void OnCollisionEnter( Collision collision )
        {
			collider_collision = collision;
			InvokeEvent();
		}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}