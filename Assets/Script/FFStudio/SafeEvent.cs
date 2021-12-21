/* Created by and for usage of FF Studios (2021). */

namespace FFStudio
{
    public class SafeEvent
    {
    #region Fields (Private)
        private event UnityMessage safeEvent;
    #endregion

    #region Properties
    #endregion

    #region Unity API
    #endregion

    #region API
        public void Invoke()
        {
            safeEvent?.Invoke();
        }
        
        public void ClearInvokeList()
        {
            safeEvent = null;
        }

        public void Subscribe( UnityMessage methodToSubscribe )
        {
    #if UNITY_EDITOR
            if( safeEvent != null )
            {
                var subscribedMethods = safeEvent.GetInvocationList();
                var nameOfMethodToSubscribe = methodToSubscribe.Method.Name;
                for( int i = 0; i < subscribedMethods.Length; i++ )
                {
                    var subscribedMethod = subscribedMethods[ i ];
                    if( subscribedMethod.Method.Name == nameOfMethodToSubscribe && subscribedMethod.Target == methodToSubscribe.Target )
                    {
                        FFLogger.LogWarning( "Method \"" + nameOfMethodToSubscribe + "\" is being assigned twice. Previous target was " + 
                                                subscribedMethod.Target );
                        safeEvent -= methodToSubscribe;
                    }
                }
            }
    #endif
            safeEvent += methodToSubscribe;
        }

        public void Unsubscribe( UnityMessage methodToUnsubscribe )
        {
    #if UNITY_EDITOR
            if( safeEvent != null )
            {
                bool foundMethod = false;
                var subscribedMethods = safeEvent.GetInvocationList();
                var nameOfMethodToUnsubscribe = methodToUnsubscribe.Method.Name;
                for( int i = 0; i < subscribedMethods.Length; i++ )
                {
                    var subscribedMethod = subscribedMethods[ i ];
                    if( subscribedMethod.Method.Name == nameOfMethodToUnsubscribe )
                        foundMethod = true;
                }

                if( foundMethod == false )
                {
                    FFLogger.LogWarning( "Method \"" + nameOfMethodToUnsubscribe + "\" is not assigned, but is being removed." );
                    return; // Info: Prevent unsubscribing from null event. User should take care of not subscribing before release.
                }
            }
    #endif
            safeEvent -= methodToUnsubscribe;
        }

    #if UNITY_EDITOR
        public bool IsMethodSubscribed( UnityMessage method )
        {
            if( safeEvent != null )
            {
                var subscribedMethods = safeEvent.GetInvocationList();
                var nameOfmethod = method.Method.Name;
                for( int i = 0; i < subscribedMethods.Length; i++ )
                {
                    var outputMsg = subscribedMethods[ i ];
                    if( outputMsg.Method.Name == nameOfmethod )
                        return true;
                }
            }

            return false;
        }
    #endif
    #endregion

    #region Implementation
    #endregion

    #region Editor Only
    #if UNITY_EDITOR
    #endif
    #endregion
    }
}