using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedFloat", menuName = "FF/Data/Shared/Float" )]
	public class SharedFloat : ScriptableObject
	{
		public float sharedValue;
	}
}