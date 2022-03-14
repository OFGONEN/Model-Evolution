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
    [ BoxGroup( "Setup" ) ] public Vector3[] movement_points;
    [ BoxGroup( "Setup" ) ] public SharedFloat movement_input_lateral;
    [ BoxGroup( "Setup" ) ] public Transform movement_transform;

	// Private \\
	private Tween movement_tween;
    private UnityMessage movement_delegate_lateral;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		movement_delegate_lateral = ExtensionMethods.EmptyMethod;
	}

    private void Update()
    {
		movement_delegate_lateral();
	}
#endregion

#region API
    public void StartPath()
    {
		movement_tween = transform.DOPath( movement_points, GameSettings.Instance.movement_speed_forward, PathType.CatmullRom )
			.SetEase( GameSettings.Instance.movement_path_ease )
            .SetLookAt( 0 , false )
            .OnComplete( StopPath )
			.SetSpeedBased();

		movement_delegate_lateral = MovementLateral;
	}

    public void IncreaseSpeed()
    {
		movement_tween.timeScale = GameSettings.Instance.IncreaseSpeedCofactor;
	}

    public void DefaultSpeed()
    {
		movement_tween.timeScale = 1f;
	}

    public void StopPath()
    {
		movement_tween = movement_tween.KillProper();
	}
#endregion

#region Implementation
    private void MovementLateral()
    {
		var localPosition = movement_transform.localPosition;

		localPosition.x = Mathf.Clamp( localPosition.x + GameSettings.Instance.movement_speed_lateral * Time.deltaTime * movement_input_lateral.sharedValue,
			-GameSettings.Instance.movement_clampDistance,
			GameSettings.Instance.movement_clampDistance );

		movement_transform.localPosition = localPosition;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    private void ExportPath()
    {
        var path = GetComponent< DOTweenPath >();
		path.wps.Clear();
		path.wps.InsertRange( 0, movement_points );
	}

    [ Button() ]
    private void ImportPath()
    {
        movement_points = GetComponent< DOTweenPath >().wps.ToArray();
    }
#endif
#endregion
}