/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using Sirenix.OdinInspector;

public class SpecialDressExtract : MonoBehaviour
{
	public DressData dressData;

    [ Button() ]
    public void ExtractData()
    {
		EditorUtility.SetDirty( dressData );
		var filter   = GetComponent< MeshFilter >();
        var renderer = GetComponent< MeshRenderer >();

		dressData.dress_mesh            = filter.sharedMesh;
		dressData.dress_sharedMaterials = renderer.sharedMaterials;

		AssetDatabase.SaveAssets();
	}
}
