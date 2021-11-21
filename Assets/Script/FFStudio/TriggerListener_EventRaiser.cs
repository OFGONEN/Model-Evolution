/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class TriggerListener_EventRaiser : MonoBehaviour
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