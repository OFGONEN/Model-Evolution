/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using FFStudio;

namespace FFStudio
{
	public abstract class RunTimePool< T > : RunTimeStack< T >
	{
#region Fields
        public T poolEntity; // GameObject.
#endregion

#region Unity API
#endregion

#region API
		public abstract void InitPool();
		public abstract T GiveEntity();
		public abstract void ReturnEntity( T entity );
#endregion

#region Implementation
		protected abstract void InitEntity();
#endregion
	}
}