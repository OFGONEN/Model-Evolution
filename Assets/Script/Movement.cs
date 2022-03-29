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
    [ BoxGroup( "Setup" ) ] public SharedPath movement_path;
    [ BoxGroup( "Setup" ) ] public SharedFloat movement_input_lateral;
    [ BoxGroup( "Setup" ) ] public Transform movement_transform;
    [ BoxGroup( "Setup" ) ] public Transform rotate_transform;
    [ BoxGroup( "Setup" ) ] public Transform animation_transform;

    [ BoxGroup( "Shared" ) ] public SharedReferenceNotifier notifier_modelTransform;
    [ BoxGroup( "Shared" ) ] public SharedReferenceNotifier notifier_clothTransform;

    [ FoldoutGroup( "Animation - Moving" ) ] public float anim_moving_position_up;
    [ FoldoutGroup( "Animation - Moving" ) ] public float anim_moving_position_down;
    [ FoldoutGroup( "Animation - Moving" ) ] public float anim_moving_position_offset;
    [ FoldoutGroup( "Animation - Moving" ) ] public float anim_moving_speed;
    [ FoldoutGroup( "Animation - Moving" ) ] public Ease[] anim_moving_ease;

    [ FoldoutGroup( "Animation - Evolve" ) ] public float anim_evolve_position_up;
    [ FoldoutGroup( "Animation - Evolve" ) ] public float anim_evolve_position_offset;
    [ FoldoutGroup( "Animation - Evolve" ) ] public float anim_evolve_duration_up;
    [ FoldoutGroup( "Animation - Evolve" ) ] public float anim_evolve_duration_down;
    [ FoldoutGroup( "Animation - Evolve" ) ] public float anim_evolve_duration_down_wait;
    [ FoldoutGroup( "Animation - Evolve" ) ] public Ease  anim_evolve_ease_up;
    [ FoldoutGroup( "Animation - Evolve" ) ] public Ease  anim_evolve_ease_down;
    [ FoldoutGroup( "Animation - Evolve" ) ] public Ease  anim_evolve_ease_rotation;

	// Private \\
	private Tween movement_tween;
    private UnityMessage movement_delegate_lateral;
	private float movement_rotate;

	// Recycled
	private RecycledSequence animation_sequence = new RecycledSequence();
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
	public void OnLookAHead()
	{
		FFLogger.Log( "Look Ahead", this );
		transform.DORotate( Vector3.zero, GameSettings.Instance.movement_lookAhead_duration );
	}

    public void StartPath()
    {
		movement_tween = transform.DOPath( movement_path.points, GameSettings.Instance.movement_speed_forward, PathType.Linear )
			.SetEase( GameSettings.Instance.movement_path_ease )
            .SetLookAt( 0 , false )
            .OnComplete( StopPath )
			.SetSpeedBased();

		movement_delegate_lateral = MovementLateral;

		MovingAnimation();
	}

    public void IncreaseSpeed()
    {
		FFLogger.Log( "Speed Up: " + GameSettings.Instance.IncreaseSpeedCofactor , this );
		movement_tween.timeScale = GameSettings.Instance.IncreaseSpeedCofactor;
	}

    public void DefaultSpeed()
    {
		FFLogger.Log( "Speed Down", this );
		movement_tween.timeScale = 1f;
	}

    public void StopPath()
    {
		movement_tween = movement_tween.KillProper();
	}

    [ Button() ]
	public void MovingAnimation()
	{
		var sequence = animation_sequence.Recycle();

		sequence.Append( animation_transform.DOLocalMoveY(
			anim_moving_position_up.ReturnRandomOffset( anim_moving_position_offset ),
			anim_moving_speed )
			.SetSpeedBased()
			.SetEase( anim_moving_ease.ReturnRandom() )
		 );
		sequence.Append( animation_transform.DOLocalMoveY(
			anim_moving_position_down.ReturnRandomOffset( anim_moving_position_offset ),
			anim_moving_speed ).SetSpeedBased() );
		sequence.OnComplete( MovingAnimation );
	}

	[ Button() ]
	public void EvolveAnimation()
	{
		animation_sequence.Kill();

		var sequence = animation_sequence.Recycle();

		var positionUp = anim_evolve_position_up.ReturnRandomOffset( anim_evolve_position_offset );
		var durationUp = Mathf.Abs( positionUp - animation_transform.localPosition.y ) * anim_evolve_duration_up / positionUp;

		sequence.Append( animation_transform.DOLocalMoveY(
			positionUp,
			durationUp 
		).SetEase( anim_evolve_ease_up ) );

		sequence.Join( animation_transform.DOLocalRotate( Vector3.up * 360,
			durationUp, RotateMode.FastBeyond360 )
			.SetEase( anim_evolve_ease_rotation )
			// .SetRelative() 
		);

		sequence.Append( animation_transform.DOLocalMoveY( 0, anim_evolve_duration_down ).SetEase( anim_evolve_ease_down ) );
		sequence.AppendInterval( anim_evolve_duration_down_wait );

		sequence.OnComplete( MovingAnimation );
	}

	public Tween OnFinishLine()
	{
		StopPath();
		animation_sequence.Kill();
		movement_delegate_lateral = ExtensionMethods.EmptyMethod;

		notifier_clothTransform.SharedValue = animation_transform;

		var target = notifier_modelTransform.sharedValue as Transform;

		animation_transform.DORotate( Vector3.zero, GameSettings.Instance.movement_finishLine_duration / 2f );
		return animation_transform.DOMove( target.position, GameSettings.Instance.movement_finishLine_duration )
			.SetEase( GameSettings.Instance.movement_finishLine_ease );
	}
#endregion

#region Implementation
    private void MovementLateral()
    {
		var localPosition = movement_transform.localPosition;

		localPosition.x = Mathf.Clamp( localPosition.x + GameSettings.Instance.movement_speed_lateral * Time.deltaTime * movement_input_lateral.sharedValue,
			-GameSettings.Instance.movement_clamp_distance,
			GameSettings.Instance.movement_clamp_distance );

		movement_transform.localPosition = localPosition;

		Rotate();
	}

	private void Rotate()
	{
		float input = movement_input_lateral.sharedValue;
		float sign = 0;

		if( Mathf.Approximately( 0 , input ) )
			sign = 0;
		else
			sign = Mathf.Sign( input );

		var drag = Time.deltaTime * GameSettings.Instance.movement_rotate_drag;

		if( drag >= Mathf.Abs( movement_rotate ) )
			drag = movement_rotate;

		var step  = Time.deltaTime * GameSettings.Instance.movement_rotate_speed * sign;
		    drag  = drag * Mathf.Sign( movement_rotate ) * -1f;
		var clamp = CurrentLevelData.Instance.levelData.cloth_rotate_clamp;

		movement_rotate                     = Mathf.Clamp( movement_rotate + step + drag, -clamp, clamp );
		rotate_transform.localEulerAngles = Vector3.up * movement_rotate;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
	[ ShowInInspector, BoxGroup( "EditorOnly" ) ] private bool Gizmos_anim_moving;
	[ ShowInInspector, BoxGroup( "EditorOnly" ) ] private bool Gizmos_anim_evolve;

	private void OnDrawGizmosSelected()
	{
		if( Gizmos_anim_moving )
		{
			var positionDown = transform.localPosition.AddY( anim_moving_position_down );
			var positionUp = transform.localPosition.AddY( anim_moving_position_up );

			Handles.DrawDottedLine( positionDown, positionUp, 1 );
			Handles.DrawWireDisc( positionDown, Vector3.forward, anim_moving_position_offset );
			Handles.DrawWireDisc( positionUp, Vector3.forward, anim_moving_position_offset );
		}

		if( Gizmos_anim_evolve )
		{
			var positionUp = transform.localPosition.AddY( anim_evolve_position_up );

			Handles.Label( positionUp, "Evolve Anim" );
			Handles.DrawWireDisc( positionUp, Vector3.forward, anim_evolve_position_offset );
		}
	}
#endif
#endregion
}