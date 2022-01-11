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
//ObjecMerge Tests

#if UNITY_5_6_OR_NEWER && UNIMERGE_TESTS
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UniMerge.Editor.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace UniMerge.Editor.Tests {
	public class ObjecMergeTests {
		readonly Stopwatch stopwatch = new Stopwatch();

		[UnityTest]
		public IEnumerator TestObjectMerge() {
			SceneMergeTests.CloseAllUniMergeWindows();

			var testScenePath = UniMergeConfig.DefaultPath + "/Demo/Object Merge/ObjectCompare.unity";
			EditorSceneManager.OpenScene(testScenePath);
			var objectMerge = EditorWindow.GetWindow<ObjectMerge>();
			objectMerge.SetGameObjects(GameObject.Find(ObjectMerge.MineTestName), GameObject.Find(ObjectMerge.TheirsTestName));
			objectMerge.Show();

			var count = 0;
			const float maxMergeFrames = 1000;
			while (objectMerge.IsUpdating()) {
				Assert.That(count++ < maxMergeFrames, "ObjectMerge.Merge failed to refresh");
				yield return null;
			}

			var root = objectMerge.root;
			objectMerge.update = root.Copy(true);

			count = 0;
			while (objectMerge.IsUpdating()) {
				Assert.That(count++ < maxMergeFrames, "ObjectMerge.root.Copy failed to refresh");
				yield return null;
			}

			Assert.That(root.Same);
			// Note: The SceneMerge and ObjectMerge windows won't show all green becasue of the Undo which is applied after tests

			SceneMergeTests.CloseAllUniMergeWindows();
		}

		/// <summary>
		/// Test general performance
		/// </summary>
		[Test]
		public void PerfTest() {
			SceneMergeTests.CloseAllUniMergeWindows();

			var trials = new List<long>();
			const int trialCount = 10;
			for (var i = 0; i < trialCount; i++) {
				EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
				UnityEngine.Random.InitState(0);
				const int objectCount = 1000;
				var mine = FillScene.GenerateObjects(objectCount);
				var theirs = FillScene.GenerateObjects(objectCount);
				var objectMerge = EditorWindow.GetWindow<ObjectMerge>();
				objectMerge.Show();

				var root = objectMerge.root;
				root.SetGameObjects(mine, theirs);

				while (objectMerge.IsUpdating()) {
					if (!objectMerge.UpdateMoveNext())
						objectMerge.update = null;
				}

				GC.Collect();
				stopwatch.Reset();
				stopwatch.Start();
				var enumerator = root.Copy(true);
				while (enumerator.MoveNext()) { }

				stopwatch.Stop();
				trials.Add(stopwatch.Elapsed.Ticks);

				Assert.That(root.Same);
				// Note: The SceneMerge and ObjectMerge windows won't show all green because of the Undo which is applied after tests

				SceneMergeTests.CloseAllUniMergeWindows();
			}

			var average = trials.Average();
			Debug.Log(string.Format("Average: {0}", new TimeSpan((long)average)));
			var stdDev = CalculateStdDev(trials);
			Debug.Log(string.Format("StDev: {0:0.0%} {1}", stdDev / average, new TimeSpan((long)stdDev)));
		}

		static double CalculateStdDev(List<long> values) {
			var ret = 0d;
			if (values.Count > 1) {
				var avg = values.Average();
				var sum = values.Sum(d => Math.Pow(d - avg, 2));
				ret = Math.Sqrt(sum / (values.Count - 1));
			}
			return ret;
		}
	}
}
#endif
