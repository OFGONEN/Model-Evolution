using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedInt", menuName = "FF/Data/Shared/Int" )]
	public class SharedInt : ScriptableObject
	{
		public int sharedValue;
	}
}