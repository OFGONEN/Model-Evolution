using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedStringProperty", menuName = "FF/Data/Shared/StringProperty" )]
	public class SharedStringProperty : ScriptableObject
	{
		public StringGameEvent changeEvent;
		private string sharedValue;
		public string Value
		{
			get
			{
				return sharedValue;
			}
			set
			{
				if( !string.Equals( sharedValue, value, System.StringComparison.Ordinal ) )
				{
					sharedValue = value;

					changeEvent.eventValue = value;
					changeEvent.Raise();
				}
			}
		}
	}
}