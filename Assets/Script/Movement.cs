/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;

public class Movement : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Setup" ) ] public Vector3[] path_points;

	// Private \\
	private Tween path_tween;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void StartPath()
    {
		path_tween = transform.DOPath( path_points, GameSettings.Instance.movement_speed_forward, PathType.CatmullRom )
			.SetEase( GameSettings.Instance.movement_path_ease )
            .SetLookAt( 0 , false )
			.SetSpeedBased();
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    public void ExportPath()
    {
        var path = GetComponent< DOTweenPath >();
		path.wps.Clear();
		path.wps.InsertRange( 0, path_points );
	}

    [ Button() ]
    public void ImportPath()
    {
        path_points = GetComponent< DOTweenPath >().wps.ToArray();
    }
#endif
#endregion
}