/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedIntCleanUp", menuName = "FF/Data/Shared/IntCleanUp" ) ]
	public class SharedIntCleanUp : SharedInt
	{
		public int defaultValue;
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