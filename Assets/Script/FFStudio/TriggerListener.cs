/* Created by and for usage of FF Studios (2021). */

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
#if UNITY_EDITOR
			if( triggerEvent != null )
			{
				var subscribedMethods = triggerEvent.GetInvocationList();
				var nameOfMethodToSubscribe = method.Method.Name;
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == nameOfMethodToSubscribe && subscribedMethod.Target == method.Target )
					{
						FFLogger.LogWarning( "Method \"" + nameOfMethodToSubscribe + "\" is being assigned twice. Previous target was " +
											 subscribedMethod.Target, AttachedComponent );
						triggerEvent -= method;
					}
				}
			}
#endif
			triggerEvent += method;
		}

		public override void Unsubscribe( TriggerMessage method )
		{
#if UNITY_EDITOR
			if( triggerEvent != null )
			{
				bool foundMethod = false;
				var subscribedMethods = triggerEvent.GetInvocationList();
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
	public bool IsMethodSubscribed( UnityMessage method )
	{
		if( triggerEvent != null )
		{
			var subscribedMethods = triggerEvent.GetInvocationList();
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