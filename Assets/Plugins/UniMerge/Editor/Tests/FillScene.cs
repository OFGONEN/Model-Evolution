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
//FillScene Window

using System.Text;
using UnityEditor;
using UnityEngine;

namespace UniMerge.Editor.Tests {
	public class FillScene : EditorWindow {
		const int TotalObjects = 50000;
		const int ObjectsPerGroup = 15;
		const int MaxDepth = 5;
		const float Density = 0.5f;
		const float ColliderDensity = 0.005f;

		// ReSharper disable once UnusedMember.Local
		[MenuItem("Window/UniMerge/Fill large scene")]
		static void Init() {
			GetWindow(typeof(FillScene));
		}

		// ReSharper disable once UnusedMember.Local
		void OnGUI() {
			if (GUILayout.Button("Generate Objects")) { GenerateObjects(TotalObjects); }
		}

		public static GameObject GenerateObjects(int totalObjects) {
			Transform currentParent = null;
			var count = 0;
			var depth = 0;
			var first = true;
			GameObject result = null;
			while (count++ < totalObjects) {
				var g = new GameObject { name = RandString(15) };

				if (first)
					result = g;

				g.transform.parent = currentParent;

				if (depth < MaxDepth)
					if (first || Random.value > Density) {
						first = false;
						currentParent = g.transform;
						depth++;
					}

				if (Random.value < ColliderDensity)
					g.AddComponent<BoxCollider>();

				if (!currentParent)
					continue;

				if (currentParent.childCount <= ObjectsPerGroup)
					continue;

				currentParent = currentParent.parent;
				depth--;
			}

			return result;
		}

		static string RandString(int length) {
			var sb = new StringBuilder(length);
			for (var i = 0; i < length; i++)
				sb.Append((char) Random.Range(97, 122));
			return sb.ToString();
		}

	}
}
