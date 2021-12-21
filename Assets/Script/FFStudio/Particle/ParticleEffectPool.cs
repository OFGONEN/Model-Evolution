/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "ParticlePool", menuName = "FF/Data/Pool/ParticlePool" ) ]
	public class ParticleEffectPool : ComponentPool< ParticleEffect >
	{
		private Transform initial_parent;
		private bool initial_active;
		private ParticleEffectStopped initial_delegate;
#region API
		public void InitPool( Transform parent, bool active, ParticleEffectStopped effectStoppedDelegate )
		{
			initial_parent = parent;
			initial_active = active;
			initial_delegate = effectStoppedDelegate;

			InitPool();
		}
#endregion

#region Implementation
		protected override ParticleEffect InitEntity()
        {
			var entity = base.InitEntity();
			entity.InitIntoPool( initial_parent, initial_active, initial_delegate );

			return entity;
		}
#endregion

	}
}