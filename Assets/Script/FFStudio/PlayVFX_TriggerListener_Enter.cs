/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

namespace FFStudio
{
	public class PlayVFX_TriggerListener_Enter : PlayVFX_Base
	{
#region Fields (Inspector Interface)
	[ Header( "Event Listeners" ) ]
		[ SerializeField ] private TriggerListener_Enter eventListener;
#endregion

#region Unity API    
		protected override void OnEnable()
		{
			eventListener.Subscribe( OnCustomTriggerEnter );
		}
		
		protected override void OnDisable()
		{
			eventListener.Unsubscribe( OnCustomTriggerEnter );
		}
#endregion

#region Implementation
		private void OnCustomTriggerEnter( Collider other )
		{
			EventResponse();
		}
#endregion
	}
}
