/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "ParticleGameEvent", menuName = "FF/Event/ParticleGameEvent" ) ]
	public class ParticleSpawnEvent : GameEvent
	{
		public string particleAlias;
		public Vector3 spawnPoint;
		[ HideInInspector ] public Transform particleParent;

		public void Raise( string alias, Vector3 position, Transform parent = null )
		{
			particleAlias  = alias;
			spawnPoint     = position;
			particleParent = parent;

			Raise();
		}
	}
}