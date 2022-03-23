/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PathExtracter : MonoBehaviour
{
#region Fields
    public DOTweenPath path;

    [ BoxGroup( "Setup" ) ] public Transform path_parent;
    [ BoxGroup( "Setup" ) ] public Vector3 path_offset;
    [ BoxGroup( "Setup" ) ] public Vector3[] path_points;

#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void ExtractPath()
    {
		path_points = new Vector3[ path_parent.childCount ];

		for( var i = 0; i < path_parent.childCount; i++ )
        {
			path_points[ i ] = path_parent.GetChild( i ).localPosition;
		}
    }

    private void OffsetPath()
    {
		for( var i = 0; i < path_points.Length; i++ )
        {
			path_points[ i ] += path_offset;
		}
    }

    [ Button() ]
    public void AddPath()
    {
		OffsetPath();

		path.wps.AddRange( path_points );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}