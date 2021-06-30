/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedBool", menuName = "FF/Data/Shared/Bool" ) ]
	public class SharedBool : ScriptableObject
	{
		public bool sharedValue;
	}
}