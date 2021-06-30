/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedVector3", menuName = "FF/Data/Shared/Vector3" ) ]
	public class SharedVector3 : ScriptableObject
	{
		public Vector3 sharedValue;
	}
}
