/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;
using Sirenix.OdinInspector;

public class NotifyUpdater< SharedDataNotifierType, SharedDataType > : MonoBehaviour
        where SharedDataNotifierType : SharedDataNotifier< SharedDataType >
{
#region Fields
        [ BoxGroup( "Setup" ), SerializeField ] protected SharedDataNotifierType sharedDataNotifier;
        [ BoxGroup( "Setup" ), SerializeField ] protected UnityEvent notify_event;
#endregion

#region Unity API
        private void OnEnable()
        {
            sharedDataNotifier.Subscribe( OnSharedDataChange );
        }
        
        private void OnDisable()
        {
            sharedDataNotifier.Unsubscribe( OnSharedDataChange );
        }
#endregion

#region Base Class API
        protected virtual void OnSharedDataChange()
        {
            notify_event.Invoke();
        }
#endregion
}