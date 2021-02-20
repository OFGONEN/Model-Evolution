using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedBoolProperty", menuName = "FF/Data/Shared/BoolProperty" )]
	public class SharedBoolProperty : ScriptableObject
	{
		public BoolGameEvent changeEvent;
		private bool sharedValue;
		public bool Value
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