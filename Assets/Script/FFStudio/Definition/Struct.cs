/* Created by and for usage of FF Studios (2021). */

using System;
using UnityEngine;
using UnityEngine.Events;

namespace FFStudio
{
	[ Serializable ]
	public struct TransformData
	{
		public Vector3 position;
		public Vector3 rotation; // Euler angles.
		public Vector3 scale; // Local scale.
	}

	[Serializable]
	public struct EventPair
	{
		public EventListenerDelegateResponse eventListener;
		public UnityEvent unityEvent;

		public void Pair()
		{
			eventListener.response = unityEvent.Invoke;
		}
	}

	[Serializable]
	public struct ParticleData
	{
		public string alias;
		public bool parent;
		public Vector3 offset;
		public float size;
	}

}
