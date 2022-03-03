/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace FFStudio
{
	public class ParticleSpawner : MonoBehaviour
	{
#region Fields
		public ParticleData[] particleDatas;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public void Spawn( int index )
		{
			var data = particleDatas[ index ];

			Transform parent = data.parent ? transform : null;
			data.particle_event.Raise( data.alias, transform.position + data.offset, parent, data.size );
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			for( var i = 0; i < particleDatas.Length; i++ )
			{
				var data = particleDatas[ i ];
				Handles.Label( transform.position + data.offset, "Particle Spawn:" + data.alias );
				Handles.DrawWireCube( transform.position + data.offset, Vector3.one / 4f );
			}
		}
#endif
#endregion
	}
}