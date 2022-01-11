/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FFStudio
{
	public class Fire_UnityEvent : MonoBehaviour
	{
#region Fields
        public EventListenerDelegateResponse event_listener;
        public UnityEvent unityEvent;
#endregion

#region Properties
#endregion

#region Unity API
        private void OnEnable()
        {
			event_listener.OnEnable();
		}

        private void OnDisable()
        {
			event_listener.OnDisable();
		}

        private void Awake()
        {
			event_listener.response = OnEventResponse;
		}
#endregion

#region API
#endregion

#region Implementation
        private void OnEventResponse()
        {
			unityEvent.Invoke();
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}