/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public abstract class CollisionListener : ColliderListener< CollisionMessage, Collision >
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
#if UNITY_EDITOR
			if( collisionEvent != null )
			{
				var subscribedMethods = collisionEvent.GetInvocationList();
				var nameOfMethodToSubscribe = method.Method.Name;
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == nameOfMethodToSubscribe && subscribedMethod.Target == method.Target )
					{
						FFLogger.LogWarning( "Method \"" + nameOfMethodToSubscribe + "\" is being assigned twice. Previous target was " +
											 subscribedMethod.Target, AttachedComponent );
						collisionEvent -= method;
					}
				}
			}
#endif
			collisionEvent += method;
		}

		public override void Unsubscribe( CollisionMessage method )
		{
#if UNITY_EDITOR
			if( collisionEvent != null )
			{
				bool foundMethod = false;
				var subscribedMethods = collisionEvent.GetInvocationList();
				var nameOfMethodToUnsubscribe = method.Method.Name;
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == nameOfMethodToUnsubscribe && subscribedMethod.Target == method.Target )
						foundMethod = true;
				}

				if( foundMethod == false )
				{
					FFLogger.LogWarning( "Method \"" + nameOfMethodToUnsubscribe + "\" is not assigned, but is being removed.", AttachedComponent );
					return; // Info: Prevent unsubscribing from null event. User should take care of not subscribing before release.
				}
			}
#endif
			collisionEvent -= method;
		}
#endregion

#region Implementation
        protected override void InvokeEvent( Collision collision )
		{
			collisionEvent?.Invoke( collision );

			unityEvent.Invoke( collision );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
		public bool IsMethodSubscribed( UnityMessage method )
		{
			if( collisionEvent != null )
			{
				var subscribedMethods = collisionEvent.GetInvocationList();
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == method.Method.Name && subscribedMethod.Target == method.Target )
						return true;
				}
			}

			return false;
		}
#endif
#endregion
	}
}