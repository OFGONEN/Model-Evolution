/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "GameEvent", menuName = "FF/Event/GameEvent" ) ]
	public class GameEvent : ScriptableObject
	{
#region Fields
		public bool canRaiseOtherEvents;
		[ ShowIf( "canRaiseOtherEvents" ) ]
		public List< GameEvent > eventsThatWillAlsoBeRaised;
		
        private readonly List< EventListener > eventListeners = new List< EventListener >();
#endregion

#region API
		[ Button ]
		public void Raise()
		{
			for( int i = eventListeners.Count - 1; i >= 0; i-- )
				eventListeners[ i ].OnEventRaised();
		}

		[ ShowIf( "canRaiseOtherEvents" ), Button( "Raise Both Self & Others" ) ]
		public void Raise_BothSelfAndOthers()
		{
			Raise();

			if( canRaiseOtherEvents && eventsThatWillAlsoBeRaised != null )
				for( var i = 0; i < eventsThatWillAlsoBeRaised.Count; i++ )
					eventsThatWillAlsoBeRaised[ i ].Raise();
		}

		[ ShowIf( "canRaiseOtherEvents" ), Button( "Raise Both Self & Others [Recursive]" ) ]
		public void Raise_BothSelfAndOthers_Recursive()
		{
			Raise();

			if( canRaiseOtherEvents && eventsThatWillAlsoBeRaised != null )
				for( var i = 0; i < eventsThatWillAlsoBeRaised.Count; i++ )
					eventsThatWillAlsoBeRaised[ i ].Raise_BothSelfAndOthers_Recursive();
		}

		public void RegisterListener( EventListener listener )
		{
			if( !eventListeners.Contains( listener ) )
				eventListeners.Add( listener );
		}

		public void UnregisterListener( EventListener listener )
		{
			if( eventListeners.Contains( listener ) )
				eventListeners.Remove( listener );
		}
#endregion
	}
}