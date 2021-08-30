/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedReference", menuName = "FF/Data/Shared/Reference" ) ]
	public class SharedReference : ScriptableObject
	{
		public object sharedValue;
	}
}