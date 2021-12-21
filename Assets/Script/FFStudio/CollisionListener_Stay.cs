/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class CollisionListener_Stay : CollisionListener
	{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
        private void OnCollisionStay( Collision collision )
        {
			InvokeEvent( collision );
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