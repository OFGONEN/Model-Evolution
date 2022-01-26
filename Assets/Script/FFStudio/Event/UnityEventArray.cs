/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FFStudio
{
	public class UnityEventArray : MonoBehaviour
	{
#region Fields
		[ SerializeField ] private UnityEvent[] unityEvents;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public void InvokeUnityEvent( int index )
		{
			if( unityEvents.Length > 0 && index >= 0 && index < unityEvents.Length )
				unityEvents[ index ].Invoke();
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}