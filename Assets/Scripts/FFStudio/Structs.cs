/* Created by and for usage of FF Studios (2021). */

using System;
using UnityEngine;

namespace FFStudio
{
	// Write structs here

	[Serializable]
	public struct TransformData
	{
		public Vector3 position;
		public Vector3 rotation; // euler angles
		public Vector3 scale; // local scale
	}
}
