using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedStringCleanUp", menuName = "FF/Data/Shared/StringCleanUp" )]
	public class SharedStringCleanUp : ScriptableObject
	{
		public string sharedValue;
		public string defaultValue;
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