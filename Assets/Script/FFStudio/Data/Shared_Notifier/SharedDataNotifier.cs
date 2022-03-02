/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public abstract class SharedDataNotifier< SharedDataType > : SharedData< SharedDataType >
    {
#region Fields (Public)
#endregion

#region Fields (Private)
        private ChangeEvent changeEvent;
#endregion

#region Properties
        public SharedDataType SharedValue
        {
            get => sharedValue;
            set
            {
                if( !EqualityComparer< SharedDataType >.Default.Equals( sharedValue, value ) )
                {
                    sharedValue = value;

                    changeEvent?.Invoke();
                }
            }
        }
#endregion

#region Unity API
		private void OnValidate()
		{
			changeEvent?.Invoke();
		}
#endregion

#region API
        public void Subscribe( ChangeEvent methodToSubscribe )
        {
#if UNITY_EDITOR
			if( changeEvent != null )
            {
				var subscribedMethods = changeEvent.GetInvocationList();
				var nameOfMethodToSubscribe = methodToSubscribe.Method.Name;
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == nameOfMethodToSubscribe && subscribedMethod.Target == methodToSubscribe.Target )
					{
						FFLogger.LogWarning( name + ": Method \"" + nameOfMethodToSubscribe + "\" is being assigned twice. Previous target was " + 
											 subscribedMethod.Target, this );
						changeEvent -= methodToSubscribe;
					}
				}
            }
#endif
			changeEvent += methodToSubscribe;
		}

		public void Unsubscribe( ChangeEvent methodToUnsubscribe )
		{
#if UNITY_EDITOR
			if( changeEvent != null )
            {
				bool foundMethod = false;
				var subscribedMethods = changeEvent.GetInvocationList();
				var nameOfMethodToUnsubscribe = methodToUnsubscribe.Method.Name;
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == nameOfMethodToUnsubscribe && subscribedMethod.Target == methodToUnsubscribe.Target )
						foundMethod = true;
				}

				if( foundMethod == false )
				{
					FFLogger.LogWarning( name + ": Method \"" + nameOfMethodToUnsubscribe + "\" is not assigned, but is being removed.", this );
					return; // Info: Prevent unsubscribing from null event. User should take care of not subscribing before release.
				}
			}
#endif
			changeEvent -= methodToUnsubscribe;
		}

		public void SetValue_DontNotify( SharedDataType value )
        {
            sharedValue = value;
        }

		public void SetValue_NotifyAlways( SharedDataType value )
		{
			sharedValue = value;
			changeEvent?.Invoke();
		}

#if UNITY_EDITOR
		public bool IsMethodSubscribed( ChangeEvent method )
		{
			if( changeEvent != null )
			{
				var subscribedMethods = changeEvent.GetInvocationList();
				var nameOfmethod = method.Method.Name;
				for( int i = 0; i < subscribedMethods.Length; i++ )
				{
					var subscribedMethod = subscribedMethods[ i ];
					if( subscribedMethod.Method.Name == nameOfmethod && subscribedMethod.Target == method.Target )
						return true;
				}
			}

			return false;
		}
#endif
		#endregion
	}
}
