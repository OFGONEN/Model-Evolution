using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedString", menuName = "FF/Data/Shared/String" )]
	public class SharedString : ScriptableObject
	{
		public string sharedValue;
	}
}