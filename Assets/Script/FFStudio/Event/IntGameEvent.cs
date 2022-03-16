/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
    [ CreateAssetMenu( fileName = "event_", menuName = "FF/Event/IntGameEvent" ) ]
    public class IntGameEvent : GameEvent
    {
        public int eventValue;

        public void Raise( int value )
        {
			eventValue = value;
			Raise();
		}
    }
}
