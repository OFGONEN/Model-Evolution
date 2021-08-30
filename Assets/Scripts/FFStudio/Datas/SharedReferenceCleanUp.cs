/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedReferenceCleanUp", menuName = "FF/Data/Shared/ReferenceCleanUp" ) ]
	public class SharedReferenceCleanUp : SharedReference
	{
		public object defaultValue = null;
		public EventListenerDelegateResponse cleanUpListener;

		private void OnEnable()
		{
			cleanUpListener.OnEnable();
			cleanUpListener.response = CleanUp;
		}
		
		private void OnDisable()
		{
			cleanUpListener.OnDisable();
		}
		
		private void CleanUp()
		{
			sharedValue = defaultValue;
		}
	}
}