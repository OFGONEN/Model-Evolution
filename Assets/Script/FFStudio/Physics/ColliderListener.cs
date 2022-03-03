/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public abstract class ColliderListener< DelegateType, CallbackArgumentType > : MonoBehaviour
	{
#region Fields (Inspector Interface)
		[ BoxGroup( "Setup" ), SerializeField ] private Component attachedComponent;

		public Component AttachedComponent => attachedComponent;
		public Collider AttachedCollider => attachedCollider;
		
		public UnityEvent< CallbackArgumentType > unityEvent;
#endregion

#region  Fields (Private)
		private Collider attachedCollider;
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

		public abstract void Subscribe( DelegateType method );
		public abstract void Unsubscribe( DelegateType method );
#endregion

#region Implementation
        protected abstract void InvokeEvent( CallbackArgumentType physicsCallbackArgument );
#endregion
	}
}