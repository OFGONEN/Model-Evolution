/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedReferanceProperty", menuName = "FF/Data/Shared/Property/ReferanceProperty" ) ]
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