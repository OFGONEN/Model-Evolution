/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class ParticleEffect : MonoBehaviour
	{
#region Fields
		[ Header( "Fired Events" ) ]
		public string alias;
		[ HideInInspector ] public Transform parent;

		// Private Fields \\
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
			// mainParticle.loop = false;
		}

		private void OnParticleSystemStopped()
		{
			transform.SetParent( parent );
			gameObject.SetActive( false );
			particleEffectStopped( this );
		}
#endregion

#region API
		public void InitIntoPool( Transform parent, bool active, ParticleEffectStopped effectStoppedDelegate )
		{
			gameObject.SetActive( active );
			transform.SetParent( parent );

			this.parent = parent;
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