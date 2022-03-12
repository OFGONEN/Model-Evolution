/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class Dress : MonoBehaviour
{
#region Fields
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

        if( levelData.cloth_start_special )
        {
			SpawnMesh( levelData.cloth_start_cloth );
		}
	}
#endregion

#region API
#endregion

#region Implementation
    private void SpawnMesh( DressData dressData )
    {
		dress_mesh_renderer.sharedMaterials   = dressData.dress_sharedMaterials;
		dress_mesh_filter.mesh                = dressData.dress_mesh;
		dress_mesh_filter.transform.position += dressData.dress_offset_position;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}