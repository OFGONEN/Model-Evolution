using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedReferance", menuName = "FF/Data/Shared/Referance" )]
	public class SharedReferance : ScriptableObject
	{
		public object sharedValue;
	}
}