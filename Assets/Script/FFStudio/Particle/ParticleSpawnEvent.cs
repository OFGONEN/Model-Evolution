/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "event_pfx_spawn", menuName = "FF/Event/ParticleGameEvent" ) ]
	public class ParticleSpawnEvent : GameEvent
	{
		public string particle_alias;
		public Vector3 particle_spawn_point;
		public float particle_spawn_size;
		[ HideInInspector ] public Transform particle_spawn_parent;

		public void Raise( string alias, Vector3 position, Transform parent = null, float size = 1f )
		{
			particle_alias        = alias;
			particle_spawn_size   = size;
			particle_spawn_point  = position;
			particle_spawn_parent = parent;

			Raise();
		}
	}
}