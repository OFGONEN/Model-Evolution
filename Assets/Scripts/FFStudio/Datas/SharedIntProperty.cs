using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedIntProperty", menuName = "FF/Data/Shared/Property/IntProperty" )]
	public class SharedIntProperty : SharedInt
	{
		public event ChangeEvent changeEvent;

		public void SetValue(int value)
		{
			if( sharedValue != value )
			{
				sharedValue = value;

				changeEvent?.Invoke();
			}
		}
	}
}