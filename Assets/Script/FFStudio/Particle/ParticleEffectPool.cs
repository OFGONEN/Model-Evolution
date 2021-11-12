/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "ParticlePool", menuName = "FF/Data/Pool/ParticlePool" ) ]
	public class ParticleEffectPool : ComponentPool< ParticleEffect >
	{

#region API
		public void InitPool( Transform parent, bool active, ParticleEffectStopped effectStoppedDelegate )
		{
			InitPool();

			foreach( var element in stack )
			{
				element.InitIntoPool( parent, active, effectStoppedDelegate );
			}
		}
#endregion
	}
}