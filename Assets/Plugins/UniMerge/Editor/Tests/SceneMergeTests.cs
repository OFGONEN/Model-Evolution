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
//SceneMerge Tests

#if UNITY_5_6_OR_NEWER && UNIMERGE_TESTS
using NUnit.Framework;
using System.Collections;
using UniMerge.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace UniMerge.Editor.Tests {
	public class SceneMergeTests {
		[UnityTest]
		public IEnumerator TestSceneMerge() {
			CloseAllUniMergeWindows();

			var sceneMerge = EditorWindow.GetWindow<SceneMerge>();
			sceneMerge.Show();
			sceneMerge.SetupTestMerge();
			sceneMerge.Merge();

			var count = 0;
			const float maxMergeFrames = 1000;
			const int sceneDataCategories = 6;
			var properties = sceneMerge.properties;
			while (properties.Count == 0) {
				Assert.That(count++ < maxMergeFrames, "SceneMerge.Merge did not complete");
				yield return null;
			}

			count = 0;
			while (properties.Count < sceneDataCategories) {
				Assert.That(count++ < maxMergeFrames, "SceneMerge.Merge had too few categories");
				yield return null;
			}

			foreach (var property in properties) {
				sceneMerge.update = property.Copy(true);

				count = 0;
				while (sceneMerge.IsUpdating()) {
					Assert.That(count++ < maxMergeFrames, "SceneMerge.Merge failed to refresh");
					yield return null;
				}
			}

			foreach (var property in properties) {
				Assert.That(property.Same, "Scene Merge failed to apply " + property.property.name);
			}

			var objectMerge = EditorWindow.GetWindow<ObjectMerge>();
			var root = objectMerge.root;
			objectMerge.update = root.Copy(true);

			count = 0;
			while (objectMerge.IsUpdating()) {
				Assert.That(count++ < maxMergeFrames, "ObjectMerge.root.Copy failed to refresh");
				yield return null;
			}

			Assert.That(root.Same);
			//Note: The SceneMerge and ObjectMerge windows won't show all green becasue of the Undo which is applied after tests

			CloseAllUniMergeWindows();
		}

		public static void CloseAllUniMergeWindows() {
			foreach (var window in Resources.FindObjectsOfTypeAll<UniMergeWindow>())
				window.Close();
		}
	}
}
#endif
