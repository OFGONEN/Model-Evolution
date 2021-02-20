using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedFloatProperty", menuName = "FF/Data/Shared/FloatProperty" )]
	public class SharedFloatProperty : ScriptableObject
	{
		public FloatGameEvent changeEvent;
		private float sharedValue;
		public float Value
		{
			get
			{
				return sharedValue;
			}
			set
			{
				if( !Mathf.Approximately( sharedValue, value ) )
				{
					sharedValue = value;

					changeEvent.eventValue = value;
					changeEvent.Raise();
				}
			}
		}
	}
}