using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedVector2", menuName = "FF/Data/Shared/Vector2" )]
	public class SharedVector2 : ScriptableObject
	{
		public Vector2 sharedValue;
	}
}