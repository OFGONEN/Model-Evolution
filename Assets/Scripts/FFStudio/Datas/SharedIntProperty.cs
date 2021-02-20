using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedIntProperty", menuName = "FF/Data/Shared/IntProperty" )]
	public class SharedIntProperty : ScriptableObject
	{
		public IntGameEvent changeEvent;
		private int sharedValue;
		public int Value
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