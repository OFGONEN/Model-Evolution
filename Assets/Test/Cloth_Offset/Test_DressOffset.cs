/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class Test_DressOffset : MonoBehaviour
{
#region Fields
    public Vector3 dressData_offset;
    public DressData dressData;

    private MeshFilter meshFilter;
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    public void SpawnDress()
    {
        if( meshFilter == null )
            meshFilter = GetComponentInChildren< MeshFilter >();

		meshFilter.mesh                    = dressData.dress_mesh;
		meshFilter.transform.localPosition = dressData.dress_offset_position;

		dressData_offset = dressData.dress_offset_position;
	}

    [ Button() ]
    public void UpdateOffSet()
    {
		EditorUtility.SetDirty( dressData );

		meshFilter.transform.localPosition = dressData_offset;
		dressData.dress_offset_position = dressData_offset;

		AssetDatabase.SaveAssets();
	}
#endif
#endregion
}