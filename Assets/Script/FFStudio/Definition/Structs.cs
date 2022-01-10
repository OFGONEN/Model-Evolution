/* Created by and for usage of FF Studios (2021). */

using System;
using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
	[ Serializable ]
	public struct TransformData
	{
		public Vector3 position;
		public Vector3 rotation; // Euler angles.
		public Vector3 scale; // Local scale.
	}
	
	public struct RecycledSequence
	{
		private UnityMessage onComplete;
		private Sequence sequence;
		
		public void Recycle( UnityMessage onComplete )
		{
			sequence = sequence.KillProper();

			this.onComplete = onComplete;

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );
		}

		public void Recycle()
		{
			sequence = sequence.KillProper();

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );
		}

		private void OnComplete_Safe()
		{
			onComplete?.Invoke();

			sequence = null;
		}
	}

	public struct RecycledTween
	{
		private UnityMessage onComplete;
		private Tween tween;

		public void Recycle( Tween tween_unsafe, UnityMessage onComplete )
		{
			tween = tween.KillProper();
			tween = tween_unsafe;

			this.onComplete = onComplete;

			tween.OnComplete( OnComplete_Safe );
		}

		public void Recycle( Tween tween_unsafe )
		{
			tween = tween.KillProper();
			tween = tween_unsafe;

			tween.OnComplete( OnComplete_Safe );
		}

		private void OnComplete_Safe()
		{
			onComplete?.Invoke();

			tween = null;
		}
	}
}
