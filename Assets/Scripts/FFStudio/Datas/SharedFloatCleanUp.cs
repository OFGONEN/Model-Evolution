using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedFloatCleanUp", menuName = "FF/Data/Shared/FloatCleanUp" )]
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
		void CleanUp()
		{
			sharedValue = defaultValue;
		}
	}
}