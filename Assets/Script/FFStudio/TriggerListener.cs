/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class TriggerListener : ColliderListener< TriggerMessage, Collider >
	{
#region Fields (Private)
		private event TriggerMessage triggerEvent;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public override void ClearEventList()
		{
			triggerEvent = null;
		}

		public override void Subscribe( TriggerMessage method )
		{
			// TODO: (fauder) SafeEvent.
			triggerEvent += method;
		}

		public override void Unsubscribe( TriggerMessage method )
		{
			triggerEvent -= method;
		}
#endregion

#region Implementation
        protected override void InvokeEvent( Collider other )
		{
			triggerEvent?.Invoke( other );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}