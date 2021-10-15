/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class SharedReferenceSetter : MonoBehaviour
	{
#region Fields
		public SharedReferenceProperty sharedReferenceProperty;
		public Component referenceComponent;
#endregion

#region UnityAPI
		private void OnEnable()
		{
			sharedReferenceProperty.SetValue( referenceComponent );
		}

		private void OnDisable()
		{
			sharedReferenceProperty.SetValue( null );
		}
#endregion
	}
}