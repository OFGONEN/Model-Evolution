/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class SharedReferenceSetter : MonoBehaviour
	{
#region Fields
		public SharedReferenceNotifier sharedReferenceProperty;
		public Component referenceComponent;
#endregion

#region UnityAPI
		private void OnEnable()
		{
			sharedReferenceProperty.SharedValue = referenceComponent;
		}

		private void OnDisable()
		{
			sharedReferenceProperty.SharedValue = null;
		}
#endregion
	}
}