using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedReferanceProperty", menuName = "FF/Data/Shared/ReferanceProperty" )]
	public class SharedReferanceProperty : ScriptableObject
	{
		public ReferanceGameEvent changeEvent;
		private object sharedValue;
		public object Value
		{
			get
			{
				return sharedValue;
			}
			set
			{
				if( sharedValue != value )
				{
					sharedValue = value;

					changeEvent.eventValue = value;
					changeEvent.Raise();

				}
			}
		}
	}
}