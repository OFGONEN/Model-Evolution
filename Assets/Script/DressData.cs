/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[CreateAssetMenu( fileName = "dress_data", menuName = "FF/Game/DressData" )]
public class DressData : ScriptableObject
{
	[ ReadOnly ] public string dress_rootBone;
	[ ReadOnly ] public string[] dress_bone_names;
	[ ReadOnly ] public Material[] dress_sharedMaterials;
	[ ReadOnly ] public Bounds dress_localBounds;
	[ ReadOnly ] public Mesh dress_mesh;

	[ BoxGroup( "Setup" ) ] public Vector3 dress_offset_position;
	[ BoxGroup( "Setup" ) ] public DressType dress_type;
	[ BoxGroup( "Setup" ) ] public bool isAccessory;
	[ BoxGroup( "Setup" ) ] public bool override_top;
    [ BoxGroup( "Setup" ) ] public bool override_bottom;
    [ BoxGroup( "Setup" ) ] public bool override_shoe;
}