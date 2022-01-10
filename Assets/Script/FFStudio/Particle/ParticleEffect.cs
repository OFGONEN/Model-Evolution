/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ParticleEffect : MonoBehaviour
	{
#region Fields
		[ Header( "Fired Events" ) ]
		public string alias;

		// Private Fields \\
		private ParticleEffectPool particle_pool;
		private ParticleEffectStopped particleEffectStopped;
		private ParticleSystem particles;
#endregion

#region UnityAPI

		private void Awake()
		{
			particles = GetComponent< ParticleSystem >();

			var mainParticle             = particles.main;
			    mainParticle.stopAction  = ParticleSystemStopAction.Callback;
			    mainParticle.playOnAwake = false;
		}

		private void OnParticleSystemStopped()
		{
			particleEffectStopped( this );
			particle_pool.ReturnEntity( this );
		}
#endregion

#region API
		public virtual void InitIntoPool( ParticleEffectPool pool, ParticleEffectStopped effectStoppedDelegate )
		{
			particle_pool         = pool;
			particleEffectStopped = effectStoppedDelegate;
		}

		public void PlayParticle( ParticleSpawnEvent particleEvent )
		{
			gameObject.SetActive( true );

			if( particleEvent.particleParent != null )
				transform.SetParent( particleEvent.particleParent );

			transform.position = particleEvent.spawnPoint;
			particles.Play();
		}
#endregion

	}
}