using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class RunTimeStack<T> : ScriptableObject
	{
		public int stackSize;
		public Stack< T > stack;

		private void Awake()
		{
			stack = new Stack< T >( stackSize );
		}
	}
}
