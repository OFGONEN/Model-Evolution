/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace FFStudio
{
    [ CreateAssetMenu( fileName = "event_", menuName = "FF/Event/IntGameEvent" ) ]
    public class IntGameEvent : GameEvent
    {
        public int eventValue;

		public float cooldown;
		[ ShowInInspector, ReadOnly ] private bool canRaise = true;

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
			DOVirtual.DelayedCall( cooldown, OnCoolDownComplete );
		}

        private void OnCoolDownComplete()
        {
			canRaise = true;
		}
    }
}
