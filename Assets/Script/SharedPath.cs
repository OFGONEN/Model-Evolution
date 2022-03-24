/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ CreateAssetMenu( fileName = "shared_path_", menuName = "FF/Game/Path" ) ]
public class SharedPath : ScriptableObject
{
	public Vector3[] points;
}