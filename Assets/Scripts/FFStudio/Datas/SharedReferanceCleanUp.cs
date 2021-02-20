using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedReferanceCleanUp", menuName = "FF/Data/Shared/ReferanceCleanUp" )]
	public class SharedReferanceCleanUp : ScriptableObject
	{
		public object sharedValue;
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
			sharedValue = null;
		}
	}
}