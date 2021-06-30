/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class SharedReferenceSetter : MonoBehaviour
	{
#region Fields
		public SharedReferenceProperty sharedReferanceProperty;
		public Component referanceComponent;
#endregion

#region UnityAPI
		private void OnEnable()
		{
			sharedReferanceProperty.SetValue( referanceComponent );
		}

		private void OnDisable()
		{
			sharedReferanceProperty.SetValue( null );
		}
#endregion
	}
}