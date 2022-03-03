/* Created by and for usage of FF Studios (2021). */

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