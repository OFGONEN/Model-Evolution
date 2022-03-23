/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using Sirenix.OdinInspector;

public class ModifyPlacer : MonoBehaviour
{
#region Fields
    public Transform path_parent;
	public Transform placement_parent;

	public int placement_index;
    public int placement_count;
    public Vector3 placement_offset;
    [ PreviewField, AssetSelector( Paths = "Assets/Prefab/Collectable") ] public GameObject placement_object;

	private List< Transform > lastAdd = new List< Transform >( 32 );
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void Place()
    {
		lastAdd.Clear();

		for( var i = 0; i < placement_count; i++ )
        {
			var index = placement_index + i;
			var startPosition = path_parent.GetChild( index ).position;
			var targetPosition = path_parent.GetChild( index + 1 ).position;
			var direction = targetPosition - startPosition;

			var placement = ( PrefabUtility.InstantiatePrefab( placement_object ) as GameObject ).transform;

			placement.position = startPosition;
			placement.forward  = direction;
			placement.position = placement.TransformPoint( placement_offset );

			placement.SetParent( placement_parent );

			lastAdd.Add( placement );
		}
    }

	[ Button() ]
	public void DeleteLastAdded()
	{
		for( var i = 0; i < lastAdd.Count; i++ )
		{
			DestroyImmediate( lastAdd[ i ].gameObject );
		}
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
        for( var i = 0; i < placement_count; i++ )
        {
			var index = placement_index + i;
			var child = path_parent.GetChild( index );
			var startPosition = child.position;
			var targetPosition = path_parent.GetChild( index + 1 ).position;
			var direction = targetPosition - startPosition;

			child.forward = direction;

			Shapes.Draw.Cone( child.TransformPoint( placement_offset.AddY( 0.25f ) ), direction, 0.1f, 0.35f, Color.red );
		}
	}
#endif
#endregion
}
