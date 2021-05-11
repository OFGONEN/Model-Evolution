using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class RunTimeStack<T> : ScriptableObject
	{
		#region Fields
		public int stackSize;
		private Stack< T > stack;

        public Stack< T > Stack
		{
			get
			{
				if( stack == null )
                    return stack = new Stack<T>(stackSize);
                else
                    return stack;
            }
		}
		#endregion
	}
}
