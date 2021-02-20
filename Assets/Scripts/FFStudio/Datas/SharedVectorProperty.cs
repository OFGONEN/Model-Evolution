using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedVector2Property", menuName = "FF/Data/Shared/Vector2Property" )]
	public class SharedVector2Property : ScriptableObject
	{
		public Vector2GameEvent changeEvent;
		private Vector2 sharedValue;
		public Vector2 Value
		{
			get
			{
				return sharedValue;
			}
			set
			{
				if( sharedValue != value )
				{
					sharedValue = value;

					changeEvent.eventValue = value;
					changeEvent.Raise();
				}
			}
		}
	}
}