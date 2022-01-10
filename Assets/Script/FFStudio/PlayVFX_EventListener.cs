/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

namespace FFStudio
{
	public class PlayVFX_EventListener : PlayVFX_Base
	{
#region Fields (Inspector Interface)
	[ Header( "Event Listeners" ) ]
		[ SerializeField ] private MultipleEventListenerDelegateResponse eventListener;
#endregion

#region Unity API    
		protected override void OnEnable()
		{
			eventListener.OnEnable();
		}
		
		protected override void OnDisable()
		{
			eventListener.OnDisable();
		}
		
		protected override void Awake()
		{
			eventListener.response = EventResponse;
		}
#endregion
	}
}