/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
    public class SoundManager : MonoBehaviour
    {
        public SoundEvent[] soundEvents;

        List< EventListenerDelegateResponse > soundEventListeners;
        Dictionary< int, AudioSource > audioSources;
        
#region Unity API
        private void Awake()
		{
			soundEventListeners = new List< EventListenerDelegateResponse >( soundEvents.Length );
			audioSources = new Dictionary< int, AudioSource >( soundEvents.Length );

			for( int i = 0; i < soundEvents.Length; i++ )
			{
				var audioComponent = gameObject.AddComponent<AudioSource>();
				audioComponent.playOnAwake = false;
				audioComponent.loop = false;

				var audioEvent = soundEvents[ i ];

				audioComponent.clip = audioEvent.audioClip;
				audioSources.Add( audioEvent.GetInstanceID(), audioComponent );

				var listener = new EventListenerDelegateResponse();
				listener.gameEvent = audioEvent;
				soundEventListeners.Add( listener );
			}
		}

		private void OnEnable()
		{
			for( int i = 0; i < soundEvents.Length; i++ )
				soundEventListeners[ i ].OnEnable();
		}

		private void OnDisable()
		{
			for( int i = 0; i < soundEvents.Length; i++ )
				soundEventListeners[ i ].OnDisable();
		}

		private void Start()
		{
			foreach( var soundEventListener in soundEventListeners )
				soundEventListener.response = ( () => PlaySound( soundEventListener.gameEvent.GetInstanceID() ) );
        }
#endregion

#region Implementation
        private void PlaySound( int instanceId )
        {
            AudioSource source;
			audioSources.TryGetValue( instanceId, out source );

			if( source != null && !source.isPlaying )
				source.Play();
        }
    }
#endregion
}