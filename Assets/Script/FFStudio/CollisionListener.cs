/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class CollisionListener : ColliderListener
	{
#region Fields
		public CollisionMessage delegateToPass;

		// Private \\
		private event CollisionMessage collisionEvet;
		protected Collision collider_collision;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public override void ClearEventList()
		{
			collisionEvet = null;
		}

		public override void Subscribe()
		{
			//TODO(Fauder) Ifdef foo suan triggerEvent iicnde var mi ?
			collisionEvet += delegateToPass;
		}

		public override void UnSubscribe()
		{
			collisionEvet -= delegateToPass;
		}
#endregion

#region Implementation
        protected override void InvokeEvent()
		{
			collisionEvet?.Invoke( collider_collision );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}