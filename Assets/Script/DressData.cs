/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "dress_data", menuName = "FF/Game/DressData" )]
public class DressData : ScriptableObject
{
	public string dress_rootBone;
	public string[] dress_bone_names;
	public Material[] dress_sharedMaterials;
	public Bounds dress_localBounds;
	public Mesh dress_mesh;

    public bool override_top;
    public bool override_bottom;
    public bool override_shoe;
}