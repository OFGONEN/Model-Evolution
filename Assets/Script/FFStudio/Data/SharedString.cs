/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedString", menuName = "FF/Data/Shared/String" ) ]
	public class SharedString : ScriptableObject
	{
		public string sharedValue;
	}
}