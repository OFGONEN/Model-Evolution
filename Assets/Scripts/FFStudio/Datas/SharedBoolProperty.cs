/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedBoolProperty", menuName = "FF/Data/Shared/Property/BoolProperty" ) ]
	public class SharedBoolProperty : SharedBool
	{
		public event ChangeEvent changeEvet;

		public void SetValue( bool value )
		{
			if( sharedValue != value )
			{
				sharedValue = value;
				changeEvet?.Invoke();
			}
		}
	}
}