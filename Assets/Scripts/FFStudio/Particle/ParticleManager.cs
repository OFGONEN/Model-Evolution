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

		[SerializeField] private ParticleEffectPool[] particleEffectPools;
		private Dictionary<string, ParticleEffectPool> particleEffectDictionary;
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

			particleEffectDictionary = new Dictionary<string, ParticleEffectPool>( particleEffectPools.Length );

			for( int i = 0; i < particleEffectPools.Length; i++ )
			{
				particleEffectPools[ i].InitPool( transform, false );
				particleEffectDictionary.Add( particleEffectPools[ i ].poolEntity.alias, particleEffectPools[ i ] );
			}
		}
#endregion

#region Implementation

		void SpawnParticle()
		{
			var spawnEvent = spawnParticleListener.gameEvent as ParticleSpawnEvent;

			ParticleEffectPool pool;

			if( !particleEffectDictionary.TryGetValue( spawnEvent.particleAlias, out pool ) )
			{
				FFLogger.Log( "Particle:" + spawnEvent.particleAlias + " is missing!" );
				return;
			}

			var effect = pool.GiveEntity( transform, false );
			effect.PlayParticle( spawnEvent );
		}
#endregion
	}
}