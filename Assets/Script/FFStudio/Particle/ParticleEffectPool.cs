/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "pool_pfx_", menuName = "FF/Data/Pool/ParticlePool" ) ]
	public class ParticleEffectPool : ComponentPool< ParticleEffect >
	{
		private ParticleEffectStopped initial_delegate;
#region API
		public void InitPool( Transform parent, bool active, ParticleEffectStopped effectStoppedDelegate )
		{
			initial_delegate = effectStoppedDelegate;

			InitPool( parent, active );
		}
#endregion

#region Implementation
		protected override ParticleEffect InitEntity()
        {
			var entity = base.InitEntity();
			entity.InitIntoPool( this, initial_delegate );

			return entity;
		}
#endregion

	}
}