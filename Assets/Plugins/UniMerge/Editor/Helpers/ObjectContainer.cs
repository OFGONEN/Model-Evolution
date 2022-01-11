//Matt Schoen
//1-20-2019
//
// This software is the copyrighted material of its author, Matt Schoen, and his company Defective Studios.
// It is available for sale on the Unity Asset store and is subject to their restrictions and limitations, as well as
// the following: You shall not reproduce or re-distribute this software without the express written (e-mail is fine)
// permission of the author. If permission is granted, the code (this file and related files) must bear this license
// in its entirety. Anyone who purchases the script is welcome to modify and re-use the code at their personal risk
// and under the condition that it not be included in any distribution builds. The software is provided as-is without
// warranty and the author bears no responsibility for damages or losses caused by the software.
// This Agreement becomes effective from the day you have installed, copied, accessed, downloaded and/or otherwise used
// the software.

//UniMerge 1.11
//ObjectContainer class

using UnityEditor;
using UnityEngine;

namespace UniMerge.Editor.Helpers {
	public class ObjectContainer : ScriptableObject {
#pragma warning disable 649
		[SerializeField]
		GameObject _mine;

		[SerializeField]
		GameObject _theirs;
#pragma warning restore 649

		public GameObject mine { get { return _mine; } }
		public GameObject theirs { get { return _theirs; } }

		public static ObjectContainer Create(out SerializedProperty mineProp, out SerializedProperty theirsProp) {
			var objectContainer = CreateInstance<ObjectContainer>();
			var serializedObject = new SerializedObject(objectContainer);
			mineProp = serializedObject.FindProperty("_mine");
			theirsProp = mineProp.Copy();
			theirsProp.Next(false);
			return objectContainer;
		}
	}
}
