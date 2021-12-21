/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class TriggerListener : ColliderListener
	{
#region Fields
		public TriggerMessage delegateToPass;

		// Private \\
		private event TriggerMessage triggerEvent;
		protected Collider collider_trigger;
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

		public override void Subscribe()
		{
			//TODO(Fauder) Ifdef foo suan triggerEvent iicnde var mi ?
			triggerEvent += delegateToPass;
		}

		public override void UnSubscribe()
		{
			triggerEvent -= delegateToPass;
		}
#endregion

#region Implementation
        protected override void InvokeEvent()
		{
			triggerEvent?.Invoke( collider_trigger );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}