/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

public class Dress : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared" ) ] public SharedIntNotifier_Aritmetic notify_time;
    [ BoxGroup( "Shared" ) ] public Pool_UIPopUpText pool_UIPopUpText;
    [ BoxGroup( "Shared" ) ] public DressData_GameEvent event_dressUp;
    [ BoxGroup( "Shared" ) ] public GameEvent event_level_complete;

    [ BoxGroup( "Setup" ) ] public MeshRenderer dress_mesh_renderer;
    [ BoxGroup( "Setup" ) ] public MeshFilter dress_mesh_filter;
    [ BoxGroup( "Setup" ) ] public Movement dress_movement;
    [ BoxGroup( "Setup" ) ] public TextMeshProUGUI indicator_text_renderer;

    [ BoxGroup( "Component" ) ] public ParticleSpawner particleSpawner; // { evolve_positive, evolve_negative }

    // Private Field \\
	private EvolveData cloth_current_data;
	private int cloth_current_index;
	private int cloth_current_time;
	private RecycledSequence indicator_sequence = new RecycledSequence();

	// Private Delegates \\
	private UnityMessage onNotifyTime;
#endregion

#region Properties
	private EvolveData NextEvolveData    => CurrentLevelData.Instance.levelData.cloth_evolve_datas[ Mathf.Min( 
		cloth_current_index + 1, 
		CurrentLevelData.Instance.levelData.cloth_evolve_datas.Length - 1 ) ];
	private EvolveData PrevEvolveData    => CurrentLevelData.Instance.levelData.cloth_evolve_datas[ Mathf.Max( 
		cloth_current_index - 1,
		0 ) ];
	private EvolveData CurrentEvolveData => CurrentLevelData.Instance.levelData.cloth_evolve_datas[ cloth_current_index ];
#endregion

#region Unity API
    private void Awake()
    {
		var levelData = CurrentLevelData.Instance.levelData;
		EvolveData evolveData;

		if( levelData.cloth_start_special )
		{
			evolveData   = levelData.cloth_start_cloth;
			onNotifyTime = OnNotifyTime_PreEvolve;
		}
        else
		{
			evolveData   = levelData.cloth_evolve_datas[ 0 ];
			onNotifyTime = OnNotifyTime_PostEvolve;
		}

		SpawnMesh( evolveData );

		// Setup up current cloth data
		cloth_current_data  = evolveData;
		cloth_current_index = 0;

		var time = evolveData.evolve_dress_time;

		notify_time.SharedValue       = time;
		indicator_text_renderer.text  = time.ToString();
		indicator_text_renderer.color = cloth_current_data.evolve_dress_color;
		cloth_current_time            = time;
		// UpdateTimeIndicator( time, evolveData.evolve_dress_color );
	}
#endregion

#region API
	//info: called from Fire_UnityEvent component with evolve_set event
	public void EvolveToIndex( IntGameEvent gameEvent )
	{
		cloth_current_index = gameEvent.eventValue;

		cloth_current_data = CurrentEvolveData;
		SpawnMesh( cloth_current_data );

		var time = cloth_current_data.evolve_dress_time;

		notify_time.sharedValue = time;
		UpdateTimeIndicator( time, cloth_current_data.evolve_dress_color );
		SpawnPopUpText( cloth_current_data );

		dress_movement.EvolveAnimation();

		onNotifyTime = OnNotifyTime_PostEvolve;
	}

	//Info: Called from NotifyUpdate component for notify_time
	public void NotifyUpdate_Time()
	{
		onNotifyTime();
	}

	public void OnFinishLine()
	{
		onNotifyTime = ExtensionMethods.EmptyMethod;
		dress_movement.OnFinishLine().OnComplete( OnClothReachedModel );
	}
#endregion

#region Implementation
	private void OnClothReachedModel()
	{
		event_dressUp.Raise( cloth_current_data.evolve_dress_data );
		DOVirtual.DelayedCall( GameSettings.Instance.game_level_finish_wait, event_level_complete.Raise );
	}

    private void SpawnMesh( EvolveData evolveData )
    {
		var dress_data = evolveData.evolve_dress_data;

		dress_mesh_renderer.sharedMaterials       = dress_data.dress_sharedMaterials;
		dress_mesh_filter.mesh                    = dress_data.dress_mesh;
		dress_mesh_filter.transform.localPosition = dress_data.dress_offset_position;
	}

	private void OnNotifyTime_PreEvolve()
	{
		var time       = notify_time.SharedValue;
		var targetData = CurrentLevelData.Instance.levelData.cloth_evolve_datas[ 0 ];

		UpdateTimeIndicator( time, ReturnLerpedColor( targetData, time ) );
	}

	private void OnNotifyTime_PostEvolve()
	{
		var levelData = CurrentLevelData.Instance.levelData;
		notify_time.sharedValue = Mathf.Clamp( notify_time.sharedValue, levelData.cloth_time_min, levelData.cloth_time_max );

		var time = notify_time.sharedValue;
		int index;

		if( CanEvolveUp( time, out index ) )
		{
			particleSpawner.Spawn( 0 ); // positive
			cloth_current_index = index;
			cloth_current_data = CurrentEvolveData;

			SpawnMesh( cloth_current_data );
			UpdateTimeIndicator( time, ReturnLerpedColor( NextEvolveData, time ) );

			SpawnPopUpText( cloth_current_data );

			dress_movement.EvolveAnimation();
		}
		else if( CanEvolveDown( time, out index ) )
		{
			particleSpawner.Spawn( 1 ); // negative
			cloth_current_index = index;
			cloth_current_data = CurrentEvolveData;

			SpawnMesh( cloth_current_data );
			UpdateTimeIndicator( time, ReturnLerpedColor( NextEvolveData, time ) );
			SpawnPopUpText( cloth_current_data );

			dress_movement.EvolveAnimation();
		}
		else
			UpdateTimeIndicator( time, ReturnLerpedColor( NextEvolveData, time ) );

	}

	private bool CanEvolveUp( int time, out int index )
	{
		var levelData = CurrentLevelData.Instance.levelData;
		var canEvolveUp = false;
		index = -1;

		for( var i = cloth_current_index + 1; i < levelData.cloth_evolve_datas.Length; i++ )
		{
			if( time >= levelData.cloth_evolve_datas[ i ].evolve_dress_time )
			{
				canEvolveUp = true;
				index = i;
			}
		}

		return canEvolveUp;
	}

	private bool CanEvolveDown( int time, out int index )
	{
		var levelData = CurrentLevelData.Instance.levelData;
		var canEvolveDown = false;
		index = -1;

		for( var i = cloth_current_index; i >= 1; i-- )
		{
			if( time < levelData.cloth_evolve_datas[ i ].evolve_dress_time )
			{
				canEvolveDown = true;
				index = i - 1;
			}
		}

		return canEvolveDown;
	}

	private void UpdateTimeIndicator( int time, Color color )
	{
		var duration = GameSettings.Instance.indicator_update_duration;

		var sequence = indicator_sequence.Recycle();
		sequence.Append( DOTween.To( () => cloth_current_time, x => cloth_current_time = x, time, duration) );
		sequence.Join( indicator_text_renderer.DOColor( color, duration ) );
		sequence.OnUpdate( () => indicator_text_renderer.text = cloth_current_time.ToString() );
	}

	private Color ReturnLerpedColor( EvolveData targetData, int time )
	{
		return Color.Lerp( cloth_current_data.evolve_dress_color,
			targetData.evolve_dress_color,
			Mathf.InverseLerp( cloth_current_data.evolve_dress_time, targetData.evolve_dress_time, time ) );
	}

	private void SpawnPopUpText( EvolveData data )
	{
		var entity = pool_UIPopUpText.GetEntity();
		entity.Spawn( transform.position + GameSettings.Instance.indicator_popUp_offset, 
			data.evolve_dress_time.ToString(), 
			GameSettings.Instance.indicator_popUp_size, 
			data.evolve_dress_color );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}