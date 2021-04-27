using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace FFStudio
{
	public class ParticleManager : MonoBehaviour
	{
		#region Fields
		[Header( "Event Listeners" )]
		public EventListenerDelegateResponse spawnParticleListener;

		[SerializeField] private ParticleEffect[] particleEffects;
		private Dictionary<string, ParticleEffect> particleEffectDictionary;
		#endregion

		#region UnityAPI

		private void OnEnable()
		{
			spawnParticleListener.OnEnable();
		}

		private void OnDisable()
		{
			spawnParticleListener.OnDisable();
		}

		private void Awake()
		{
			spawnParticleListener.response = SpawnParticle;

			particleEffectDictionary = new Dictionary<string, ParticleEffect>( particleEffects.Length );

			for( int i = 0; i < particleEffects.Length; i++ )
			{
				particleEffectDictionary.Add( particleEffects[ i ].alias, particleEffects[ i ] );
			}


		}
		#endregion

		#region Implementation

		void SpawnParticle()
		{
			var spawnEvent = spawnParticleListener.gameEvent as ParticleSpawnEvent;

			ParticleEffect effect;

			if( !particleEffectDictionary.TryGetValue( spawnEvent.particleAlias, out effect ) )
			{
				FFLogger.Log( "Particle:" + spawnEvent.particleAlias + " is missing!" );
				return;
			}

			effect.PlayParticle( spawnEvent );
		}
		#endregion
	}
}