/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    [ CreateAssetMenu( fileName = "SwipeInputEvent", menuName = "FF/Event/Input/SwipeInputEvent" ) ]
    public class SwipeInputEvent : Vector2GameEvent
    {
        public float angleThreshold;
        [ HideInInspector ] public Vector2 inputValue;

		public void ReceiveInput( Vector2 swipeDelta )
        {
            eventValue = swipeDelta;
			inputValue = DecideDirection( Vector2.Angle( Vector2.right, swipeDelta ), swipeDelta );

			if( inputValue != Vector2.zero )
                Raise();
        }

		Vector2 DecideDirection( float unsignedAngle, Vector2 delta )
        {
			if( unsignedAngle > 180 - angleThreshold )
				return Vector2.left;
			else if( angleThreshold <= unsignedAngle && unsignedAngle <= 180 - angleThreshold )
			{
				if( delta.y >= 0 )
					return Vector2.up;
				else
					return Vector2.down;
			}
			else if( unsignedAngle < angleThreshold )
				return Vector2.right;
			else
				return Vector2.zero;
		}
    }
}