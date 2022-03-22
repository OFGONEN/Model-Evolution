/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class SpecialDressExtract : MonoBehaviour
{
	public DressData dressData;

    [ Button() ]
    public void ExtractData()
    {
        var filter   = GetComponent< MeshFilter >();
        var renderer = GetComponent< MeshRenderer >();

		dressData.dress_mesh            = filter.sharedMesh;
		dressData.dress_sharedMaterials = renderer.sharedMaterials;
	}
}
