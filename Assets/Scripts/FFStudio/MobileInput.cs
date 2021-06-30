/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    public class MobileInput : MonoBehaviour
    {
		[ Header( "Fired Events" ) ]
		public SwipeInputEvent swipeInputEvent;
		public IntGameEvent tapInputEvent;
		
		private int swipeThreshold;
		
#region Unity API
		private void Awake()
		{
			swipeThreshold = Screen.width * GameSettings.Instance.swipeThreshold / 100;
		}
#endregion
		
#region API
		public void Swiped( Vector2 delta )
		{
			swipeInputEvent.ReceiveInput( delta );
		}
		
		public void Tapped( int count )
		{
			tapInputEvent.eventValue = count;

			tapInputEvent.Raise();
		}
#endregion
    }
}