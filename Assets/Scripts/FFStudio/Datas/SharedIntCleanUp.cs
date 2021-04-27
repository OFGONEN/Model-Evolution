using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedIntCleanUp", menuName = "FF/Data/Shared/IntCleanUp" )]
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
		void CleanUp()
		{
			sharedValue = defaultValue;
		}
	}
}