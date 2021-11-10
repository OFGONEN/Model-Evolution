/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedVector2Property", menuName = "FF/Data/Shared/Property/Vector2Property" ) ]
	public class SharedVector2Property : SharedVector2
	{
		public event ChangeEvent changeEvent;

		public void SetValue( Vector2 value )
		{
			if( sharedValue != value )
			{
				sharedValue = value;

				changeEvent?.Invoke();
			}
		}
	}
}