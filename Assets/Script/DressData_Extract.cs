/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class DressData_Extract : MonoBehaviour
{
#region Fields
    [ LabelText( "Extract Renderer" ) ] public SkinnedMeshRenderer skinnedMeshRenderer_Extract;
    [ LabelText( "Dress Renderer" ) ] public SkinnedMeshRenderer skinnedMeshRenderer_Dress;
    public DressData dressData;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void ExtractDressData()
    {
        dressData.dress_bone_names = new string[ skinnedMeshRenderer_Extract.bones.Length ];

        dressData.dress_mesh            = skinnedMeshRenderer_Extract.sharedMesh;
        dressData.dress_localBounds     = skinnedMeshRenderer_Extract.localBounds;
        dressData.dress_sharedMaterials = skinnedMeshRenderer_Extract.sharedMaterials;
        dressData.dress_rootBone        = skinnedMeshRenderer_Extract.rootBone.name;

        for( var i = 0; i < skinnedMeshRenderer_Extract.bones.Length; i++ )
            dressData.dress_bone_names[ i ] = skinnedMeshRenderer_Extract.bones[ i ].name;
    }

    [ Button() ]
    public void DressUp()
    {
		gameObject.UpdateSkinnedMeshRenderer( skinnedMeshRenderer_Dress, dressData );
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}