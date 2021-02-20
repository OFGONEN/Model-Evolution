using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedBool", menuName = "FF/Data/Shared/Bool" )]
	public class SharedBool : ScriptableObject
	{
		public bool sharedValue;
	}
}