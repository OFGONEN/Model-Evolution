/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public abstract class ColliderListener : MonoBehaviour
	{
#region Fields
		// Private
		[ SerializeField ] private Component attachedComponent;
		private Collider attachedCollider;

		// Public Properties
		public Component AttachedComponent => attachedComponent;
		public Collider AttachedCollider => attachedCollider;
#endregion

#region UnityAPI
		private void Awake()
		{
			attachedCollider = GetComponent< Collider >();
		}
#endregion

#region API
		public abstract void ClearEventList();

		public void SetColliderActive( bool active )
		{
			attachedCollider.enabled = active;
		}
#endregion

#region Implementation
        protected abstract void InvokeEvent();
#endregion
	}
}