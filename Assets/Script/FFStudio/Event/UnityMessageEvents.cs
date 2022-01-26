/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class UnityMessageEvents : MonoBehaviour
	{
#region Fields
        [ BoxGroup( "Setup" ) ] public UnityEvent onEnableEvent;
        [ BoxGroup( "Setup" ) ] public UnityEvent onDisableEvent;
        [ BoxGroup( "Setup" ) ] public UnityEvent onAwakeEvent;
        [ BoxGroup( "Setup" ) ] public UnityEvent onStartEvent;
#endregion

#region Properties
#endregion

#region Unity API
        private void OnEnable()
        {
			onEnableEvent.Invoke();
		}

        private void OnDisable()
        {
			onDisableEvent.Invoke();
		}

        private void Awake()
        {
			onAwakeEvent.Invoke();
		}

        private void Start()
        {
			onStartEvent.Invoke();
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