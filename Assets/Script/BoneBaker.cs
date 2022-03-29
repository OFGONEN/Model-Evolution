/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;
using UnityEditor;

public class BoneBaker : MonoBehaviour
{

#if UNITY_EDITOR
	public Transform baseObject;
	public Transform targetObject;

	public SkinnedMeshRenderer baseSkin;
	public SkinnedMeshRenderer targetSkin;

	public Vector3 offset;
	public float rotate;
	public string assetPath;

	[Button()]
	public void LogBoneTransform()
	{
		if( baseSkin.bones.Length == targetSkin.bones.Length ) FFLogger.Log( "Bone Lenth Matches" );

		for( var i = 0; i < baseSkin.bones.Length; i++ )
		{
			var bone_Base = baseSkin.bones[ i ];

			for( var x = 0; x < baseSkin.bones.Length; x++ )
			{
				var bone_Target = targetSkin.bones[ x ];

				if( bone_Base.name == bone_Target.name )
				{
					FFLogger.Log( bone_Base.name + " position offset: " + ( bone_Target.position - bone_Base.position ), bone_Base );
					FFLogger.Log( bone_Base.name + " rotation offset: " + ( bone_Target.eulerAngles - bone_Base.eulerAngles ), bone_Base );
				}
			}
		}
	}

	[ Button() ]
	public void LogBoneAllTransform()
	{
		var baseBones = baseObject.GetComponentsInChildren<Transform>();
		var targetBones = targetObject.GetComponentsInChildren<Transform>();

		if( baseBones.Length != targetBones.Length )
		{
			FFLogger.Log( "BONE COUNT DOES NOT MATCH" );
			return;
		}

		for( var i = 0; i < baseBones.Length; i++ )
		{
			if( baseBones[ i ].name == targetBones[ i ].name )
			{
					FFLogger.Log( baseBones[ i ].name + " position offset: " + ( targetBones[ i ].position - baseBones[ i ].position ), baseBones[ i ] );
					FFLogger.Log( baseBones[ i ].name + " rotation offset: " + ( targetBones[ i ].eulerAngles - baseBones[ i ].eulerAngles ), baseBones[ i ] );
			}
			else
				FFLogger.Log( baseBones[ i ].name + " DOES NOT MATCH " + targetBones[ i ].name );
		}
	}

	[Button()]
	public void EvenUpBones()
	{
		var baseBones = baseObject.GetComponentsInChildren<Transform>();
		var targetBones = targetObject.GetComponentsInChildren<Transform>();

		if( baseBones.Length != targetBones.Length )
		{
			FFLogger.Log( "BONE COUNT DOES NOT MATCH" );
			return;
		}

		for( var i = 0; i < baseBones.Length; i++ )
		{
			if( baseBones[ i ].name == targetBones[ i ].name )
			{
				targetBones[ i ].position = baseBones[ i ].position;
				targetBones[ i ].rotation = baseBones[ i ].rotation;
			}
		}
	}

	[ Button() ]
	public void BakeAndSet()
	{
		List<Matrix4x4> poses = new List<Matrix4x4>();
		targetSkin.sharedMesh.GetBindposes( poses );

		var mesh = GetMeshFromSkinnedMeshRenderer( targetSkin );
		mesh.boneWeights = targetSkin.sharedMesh.boneWeights;
		mesh.bindposes = poses.ToArray();

		baseSkin.sharedMesh      = mesh;
		baseSkin.sharedMaterials = targetSkin.sharedMaterials;
		baseSkin.localBounds     = targetSkin.localBounds;
		// baseSkin.bones = targetSkin.bones;
	}

    [ Button() ]
    public void CreateAsset()
    {
		AssetDatabase.CreateAsset( baseSkin.sharedMesh, assetPath );
	}

	private Mesh GetMeshFromSkinnedMeshRenderer( SkinnedMeshRenderer skinnedMeshRenderer )
	{
		Mesh newMesh = new Mesh();
		skinnedMeshRenderer.BakeMesh( newMesh );
		//update with scaling scale
		Vector3[] verts = newMesh.vertices;
		float scaleX = skinnedMeshRenderer.transform.lossyScale.x;
		float scaleY = skinnedMeshRenderer.transform.lossyScale.y;
		float scaleZ = skinnedMeshRenderer.transform.lossyScale.z;
		for( int i = 0; i < verts.Length; i++ )
		{
			verts[ i ] = new Vector3( verts[ i ].x / scaleX, verts[ i ].y / scaleY, verts[ i ].z / scaleZ );
			verts[ i ] = Quaternion.AngleAxis( rotate, Vector3.right ) * verts[ i ];
			verts[ i ] += offset;
		}
		newMesh.vertices = verts;
		newMesh.RecalculateBounds();
		newMesh.RecalculateNormals();
		return newMesh;
	}
#endif
}