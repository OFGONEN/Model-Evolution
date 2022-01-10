/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;

public abstract class PlayVFX_Base : MonoBehaviour
{
#region Fields (Inspector Interface)
[ Header( "VFX" ) ]
    [ SerializeField ] protected ParticleSystem[] vfxToPlay;
    [ SerializeField ] protected float delay;
	
[ Header( "On Complete" ) ]
	[ SerializeField ] protected GameEvent[] alsoFireTheseEvents;
#endregion

#region Unity API    
    protected virtual void OnEnable()
    {
	}
    
    protected virtual void OnDisable()
    {
	}
	
    protected virtual void Awake()
    {
	}
#endregion

#region API
#endregion

#region Base (Protected) API
	protected void EventResponse()
	{
		if( delay > 0 )
			DOVirtual.DelayedCall( delay, Play );
		else
			Play();
	}
#endregion
    
#region Implementation
	private void Play()
	{
		foreach( var vfx in vfxToPlay )
			vfx.Play();
			
		foreach( var eventToFire in alsoFireTheseEvents )
			eventToFire.Raise();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
