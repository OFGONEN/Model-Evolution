using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class RunTimeStack<T> : ScriptableObject
	{
		#region Fields
		public int stackSize;
		protected Stack< T > stack;

        public Stack< T > Stack
		{
			get
			{
                return stack;
            }
		}
		#endregion
	}
}
