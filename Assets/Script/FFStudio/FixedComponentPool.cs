/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public abstract class FixedComponentPool< T > : ComponentPool< T > where T: Component
	{
#region Fields
        [ SerializeField ] protected List< T > activeEntities;

		public List< T > ActiveEntities => activeEntities;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
        public override void InitPool()
        {
			base.InitPool();

            activeEntities = new List< T >( stackSize );
		}

		public override T GiveEntity()
        {
            T entity;

			if( stack.Count > 0 )
            {
				entity = stack.Pop();
				activeEntities.Add( entity );
			}
            else if( activeEntities.Count > 0 ) 
            {
				entity = activeEntities[ 0 ];
				activeEntities.RemoveAt( 0 );
				activeEntities.Add( entity );
			}
            else 
				entity = GameObject.Instantiate( poolEntity );

			return entity;
        }


		public override void ReturnEntity( T entity )
        {
			activeEntities.Remove( entity );
			stack.Push( entity );
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}