/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedReferenceProperty", menuName = "FF/Data/Shared/Property/ReferenceProperty" ) ]
	public class SharedReferenceProperty : SharedReference
	{
		public event ChangeEvent changeEvent;

		public void SetValue( object value )
		{
			if( sharedValue != value )
			{
				sharedValue = value;

				changeEvent?.Invoke();
			}
		}
	}
}