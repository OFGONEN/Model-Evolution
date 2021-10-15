/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using UnityEngine;

namespace FFStudio
{
	public abstract class RoutineGameEvent : GameEvent
	{
		public GameEvent routineEndEvent;
		public GameEvent routineTickEvent;
		protected Coroutine routine;

		public float routineWaitDuration;
		protected WaitForSeconds waitForSeconds;

		protected void StartRoutine( MonoBehaviour routineOwner )
		{
			if( waitForSeconds == null ) waitForSeconds = new WaitForSeconds( routineWaitDuration );

			Raise();
			routine = routineOwner.StartCoroutine( EventRoutine() );
		}
        
		protected void EndRoutine()
		{
			routineEndEvent.Raise();
			routine = null;
		}
        
		protected abstract IEnumerator EventRoutine();
	}
}