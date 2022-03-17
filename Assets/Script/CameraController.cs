/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CameraController : MonoBehaviour
{
#region Fields (Inspector Interface)
	[ BoxGroup( "Setup" ) ] public SharedReferenceNotifier reference_transform_target;
#endregion

#region Fields (Private)
	private Transform transform_target;
	private Vector3 followOffset;

	private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
	private void Awake()
	{
		updateMethod = ExtensionMethods.EmptyMethod;
	}

	private void Update()
	{
		updateMethod();
	}
#endregion

#region API
	public void LevelStartedResponse()
	{
		transform_target = reference_transform_target.SharedValue as Transform;
		followOffset     = transform_target.InverseTransformPoint( transform.position );

		updateMethod = FollowPlayer;
	}

	public void LevelCompleteResponse()
	{
		updateMethod = ExtensionMethods.EmptyMethod;
	}
#endregion

#region Implementation
	private void FollowPlayer()
	{
		var player_position = transform_target.position;
		var target_position = transform_target.TransformPoint( followOffset );

		// target_position.x = 0;
		target_position.x = Mathf.Lerp( transform.position.x, target_position.x, Time.deltaTime * GameSettings.Instance.camera_follow_speed_lateral );
		target_position.z = Mathf.Lerp( transform.position.z, target_position.z, Time.deltaTime * GameSettings.Instance.camera_follow_speed_depth );
		transform.position = target_position;

		// transform.LookAtAxis( player_position, Vector3.up );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR

#endif
#endregion
}