/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "ParticleGameEvent", menuName = "FF/Event/ParticleGameEvent" ) ]
	public class ParticleSpawnEvent : GameEvent
	{
		public string particleAlias;
		[ HideInInspector ]
		public bool changePosition = true;
		public Vector3 spawnPoint;
	}
}