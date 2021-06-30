/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedFloatCleanUp", menuName = "FF/Data/Shared/FloatCleanUp" ) ]
	public class SharedFloatCleanUp : SharedFloat
	{
		public float defaultValue;
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