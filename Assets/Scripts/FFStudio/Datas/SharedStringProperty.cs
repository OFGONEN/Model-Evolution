using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedStringProperty", menuName = "FF/Data/Shared/Property/StringProperty" )]
	public class SharedStringProperty : SharedString
	{
		public event ChangeEvent changeEvent;

		public void SetValue(string value)
		{
			if( !string.Equals( sharedValue, value, System.StringComparison.Ordinal ) )
			{
				sharedValue = value;

				changeEvent?.Invoke();
			}
		}
	}
}