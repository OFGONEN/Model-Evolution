/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class ParticleManager : MonoBehaviour
	{
#region Fields
		[ Header( "Event Listeners" ) ]
		public EventListenerDelegateResponse spawnParticleListener;

		[ SerializeField ]
		private ParticleEffectPool[] particleEffectPools;
		private Dictionary< string, ParticleEffectPool > particleEffectDictionary;
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

			particleEffectDictionary = new Dictionary< string, ParticleEffectPool >( particleEffectPools.Length );

			for( int i = 0; i < particleEffectPools.Length; i++ )
			{
				particleEffectPools[ i ].InitPool( transform, false, ParticleEffectStopped );
				particleEffectDictionary.Add( particleEffectPools[ i ].pool_entity.alias, particleEffectPools[ i ] );
			}
		}
#endregion

#region Implementation
		private void SpawnParticle()
		{
			var spawnEvent = spawnParticleListener.gameEvent as ParticleSpawnEvent;

			ParticleEffectPool pool;

			if( !particleEffectDictionary.TryGetValue( spawnEvent.particleAlias, out pool ) )
			{
				FFLogger.Log( "Particle:" + spawnEvent.particleAlias + " is missing!" );
				return;
			}

			var effect = pool.GetEntity();
			effect.PlayParticle( spawnEvent );
		}

		private void ParticleEffectStopped( ParticleEffect particleEffect )
		{
			ParticleEffectPool pool;

			if( !particleEffectDictionary.TryGetValue( particleEffect.alias, out pool ) )
			{
				FFLogger.Log( "Particle:" + particleEffect.alias + " is missing!", particleEffect.gameObject );
				return;
			}

			pool.ReturnEntity( particleEffect );
		}
#endregion
	}
}