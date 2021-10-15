/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedBoolCleanUp", menuName = "FF/Data/Shared/BoolCleanUp" ) ]
	public class SharedBoolCleanUp : SharedBool
	{
		public bool defaultValue;
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