/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class ComponentPool< T > : RunTimeStack< T > where T: Component
	{
		#region Fields
		public T pool_entity;

		private Transform pool_parent;
		private bool pool_active;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
		public void InitPool( Transform parent, bool active )
		{
			pool_parent = parent;
			pool_active = active;

			stack = new Stack< T >( stackSize );

            for( var i = 0; i < stackSize; i++ )
			{
				var entity = InitEntity();
				stack.Push( entity );
			}
		}

		public T GetEntity()
		{
			T entity;

			if( stack.Count > 0 )
				entity = stack.Pop();
			else
				entity = InitEntity();

			return entity;
		}

		public void ReturnEntity( T entity )
        {
			entity.gameObject.SetActive( pool_active );
			entity.transform.SetParent( pool_parent );
			stack.Push( entity );
		}
#endregion

#region Implementation
		protected virtual T InitEntity()
        {
            var entity = GameObject.Instantiate( pool_entity );
			entity.gameObject.SetActive( pool_active );
			entity.transform.parent = pool_parent;

			return entity;
		}
#endregion
	}
}