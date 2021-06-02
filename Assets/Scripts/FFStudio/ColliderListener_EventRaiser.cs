using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FFStudio
{
	public class ColliderListener_EventRaiser : MonoBehaviour
	{
		#region Fields
		public event TriggerEnter triggerEnter;
		#endregion

		#region UnityAPI
		private void OnTriggerEnter( Collider other )
		{
			triggerEnter?.Invoke( other );
		}
		#endregion
	}
}