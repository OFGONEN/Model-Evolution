/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class ParticleEffect : MonoBehaviour
	{
#region Fields
		[ BoxGroup( "Setup" ) ] public MultipleEventListenerDelegateResponse level_finish_listener;
		[ BoxGroup( "Setup" ) ] public string alias;

		// Private Fields \\
		private ParticleEffectPool particle_pool;
		private ParticleEffectStopped particleEffectStopped;
		private ParticleSystem particles;
#endregion

#region UnityAPI
		private void OnEnable()
		{
			level_finish_listener.OnEnable();
		}

		private void OnDisable()
		{
			level_finish_listener.OnDisable();
		}

		private void Awake()
		{
			particles = GetComponentInChildren< ParticleSystem >();

			var mainParticle             = particles.main;
			    mainParticle.stopAction  = ParticleSystemStopAction.Callback;
			    mainParticle.playOnAwake = false;

			level_finish_listener.response = OnParticleSystemStopped;
		}

		private void OnParticleSystemStopped()
		{
			particleEffectStopped( this );
			particle_pool.ReturnEntity( this );
			transform.localScale = Vector3.one;
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
			
			transform.position   = particleEvent.particle_spawn_point;
			transform.localScale = Vector3.one * particleEvent.particle_spawn_size;

			if( particleEvent.particle_spawn_parent != null )
				transform.SetParent( particleEvent.particle_spawn_parent );

			particles.Play();
		}
#endregion

	}
}