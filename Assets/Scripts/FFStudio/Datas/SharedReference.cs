using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedReferance", menuName = "FF/Data/Shared/Referance" )]
	public class SharedReference : ScriptableObject
	{
		public object sharedValue;
	}
}