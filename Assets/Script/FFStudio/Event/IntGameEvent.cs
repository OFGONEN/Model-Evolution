/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
    [ CreateAssetMenu( fileName = "event_", menuName = "FF/Event/IntGameEvent" ) ]
    public class IntGameEvent : GameEvent
    {
        public int eventValue;

		public float cooldown;
		private bool canRaise = true;

		public void Raise( int value )
        {
            if( canRaise )
            {
			    eventValue = value;
			    Raise();
				CoolDown();
			}
		}

        public void CoolDown()
        {
			canRaise = false;
			DOVirtual.DelayedCall( cooldown, CoolDown );
		}

        private void OnCoolDownComplete()
        {
			canRaise = true;
		}
    }
}
