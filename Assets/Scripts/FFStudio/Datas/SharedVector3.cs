using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedVector3", menuName = "FF/Data/Shared/Vector3" )]
	public class SharedVector3 : ScriptableObject
	{
		public Vector3 sharedValue;
	}
}
