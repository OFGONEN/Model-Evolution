/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class CollisionListener : ColliderListener< CollisionMessage, Collision >
	{
#region Fields (Private)
		private event CollisionMessage collisionEvent;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public override void ClearEventList()
		{
			collisionEvent = null;
		}

		public override void Subscribe( CollisionMessage method )
		{
			// TODO: (Fauder) SafeEvent.
			collisionEvent += method;
		}

		public override void Unsubscribe( CollisionMessage method )
		{
			collisionEvent -= method;
		}
#endregion

#region Implementation
        protected override void InvokeEvent( Collision collision )
		{
			collisionEvent?.Invoke( collision );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}