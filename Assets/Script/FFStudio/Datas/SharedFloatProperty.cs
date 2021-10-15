/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedFloatProperty", menuName = "FF/Data/Shared/Property/FloatProperty" ) ]
	public class SharedFloatProperty : SharedFloat
	{
		public event ChangeEvent changeEvent;

		public void SetValue( float value )
		{
			if( !Mathf.Approximately( sharedValue, value ) )
			{
				sharedValue = value;

				changeEvent?.Invoke();
			}
		}
	}
}