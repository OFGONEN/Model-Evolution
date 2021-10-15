/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class RunTimePool< T > : RunTimeStack< T > where T: Component
	{
#region Fields
        public T poolEntity; // GameObject.
#endregion

#region Unity API
#endregion

#region API
		public void InitPool( Transform parent, bool active )
		{
			stack = new Stack< T >( stackSize );

			for( var i = 0; i < stackSize; i++ )
			{
				var entity = GameObject.Instantiate( poolEntity );
				entity.transform.SetParent( parent );
				entity.gameObject.SetActive( active );
				stack.Push( entity );
			}
		}

		public T GiveEntity( Transform parent, bool active )
		{
			T entity;

			if( stack.Count > 0 )
				entity = stack.Pop();
			else 
			{
				entity = GameObject.Instantiate( poolEntity );
				entity.transform.SetParent( parent );
			}

			entity.gameObject.SetActive( active );
			return entity;
		}
#endregion

#region Implementation
#endregion
	}
}