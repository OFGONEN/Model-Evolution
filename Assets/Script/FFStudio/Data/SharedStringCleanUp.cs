/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedStringCleanUp", menuName = "FF/Data/Shared/StringCleanUp" ) ]
	public class SharedStringCleanUp : SharedString
	{
		public string defaultValue = string.Empty;
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