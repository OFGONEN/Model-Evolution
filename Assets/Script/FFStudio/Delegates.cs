/* Created by and for usage of FF Studios (2021). */

using Lean.Touch;
using UnityEngine;

namespace FFStudio
{
	public delegate void ChangeEvent();
    public delegate void TriggerMessage( Collider other );
    public delegate void CollisionMessage( Collision collision );
	public delegate void UnityMessage();
	public delegate void LeanFingerDelegate( LeanFinger finger );
	public delegate void ParticleEffectStopped( ParticleEffect effect );
}
