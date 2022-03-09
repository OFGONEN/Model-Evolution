/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

public class DressData_Extract : MonoBehaviour
{
#region Fields
    public DressData dressData;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void ExtractBoneName()
    {
        var skinnedMeshRenderer = GetComponent< SkinnedMeshRenderer >();

        dressData.dress_bone_names = new string[ skinnedMeshRenderer.bones.Length ];

        dressData.dress_mesh            = skinnedMeshRenderer.sharedMesh;
        dressData.dress_localBounds     = skinnedMeshRenderer.localBounds;
        dressData.dress_sharedMaterials = skinnedMeshRenderer.sharedMaterials;
        dressData.dress_rootBone        = skinnedMeshRenderer.rootBone.name;

        for( var i = 0; i < skinnedMeshRenderer.bones.Length; i++ )
            dressData.dress_bone_names[ i ] = skinnedMeshRenderer.bones[ i ].name;
    }
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}