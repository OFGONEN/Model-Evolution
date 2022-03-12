/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Dress : MonoBehaviour
{
#region Fields
    [ BoxGroup( "Shared" ) ] public SharedIntNotifier_Aritmetic notify_time;

    [ BoxGroup( "Setup" ) ] public Animator animator;
    [ BoxGroup( "Setup" ) ] public MeshRenderer dress_mesh_renderer;
    [ BoxGroup( "Setup" ) ] public MeshFilter dress_mesh_filter;

    // Private Field \\
    private bool canEvolve;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		var levelData = CurrentLevelData.Instance.levelData;
		EvolveData evolveData;

		if( levelData.cloth_start_special )
			evolveData = levelData.cloth_start_cloth;
        else
			evolveData = levelData.cloth_evolve_datas[ 0 ];

		notify_time.SharedValue = evolveData.evolve_dress_time;
		SpawnMesh( evolveData );
	}
#endregion

#region API
#endregion

#region Implementation
    private void SpawnMesh( EvolveData evolveData )
    {
		var dress_data = evolveData.evolve_dress_data;

		dress_mesh_renderer.sharedMaterials   = dress_data.dress_sharedMaterials;
		dress_mesh_filter.mesh                = dress_data.dress_mesh;
		dress_mesh_filter.transform.position += dress_data.dress_offset_position;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}