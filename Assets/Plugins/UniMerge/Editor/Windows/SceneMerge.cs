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
//SceneMerge Window

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#define Unity4_0To4_2
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniMerge.Editor.Helpers;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

#if !(UNITY_5_3 || UNITY_5_3_OR_NEWER)
using EditorSceneManager = UnityEditor.EditorApplication;
#endif

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
using PrefabUtility = UnityEditor.EditorUtility;
#endif

namespace UniMerge.Editor.Windows {
	[InitializeOnLoad]
	public class SceneMerge : UniMergeWindow {
		struct StackHelper {
			public readonly PropertyHelper helper;
			public readonly int siblingIndex;

			public StackHelper(PropertyHelper helper, int siblingIndex) {
				this.helper = helper;
				this.siblingIndex = siblingIndex;
			}
		}

		const string MyTestSceneName = "Mine", TheirTestSceneName = "Theirs";

		string myName = string.Empty, theirName = string.Empty;

		const string MessagePath = "Assets/merges.txt";

		public GameObject myContainer { private get; set; }
		public GameObject theirContainer { private get; set; }
		public bool compareLightingData { get; private set; }

		UnityObject mine, theirs;

		internal readonly List<PropertyHelper> properties = new List<PropertyHelper>();

		bool loading;
		bool merged;

		SceneData mySceneData, theirSceneData;
		SerializedObject mySO, theirSO;

		Action draw;
		ObjectMerge childWindow;

		public GameObjectHelper root {
			get {
				// ReSharper disable once ConvertIfStatementToReturnStatement
				if (childWindow)
					return childWindow.root;

				return null;
			}
		}

		protected override int headerSize { get { return 163; } }

		static readonly Stack<StackHelper> HelperStack = new Stack<StackHelper>();

		static SceneMerge() {
			EditorApplication.update -= CheckForMessageFile;
			EditorApplication.update += CheckForMessageFile;
		}

		// ReSharper disable once UnusedMember.Local
		[MenuItem("Window/UniMerge/Scene Merge %&m")]
		static void Init() {
			GetWindow<SceneMerge>(false, "SceneMerge").Show();
		}

		protected override void OnEnable() {
			minSize = new Vector2(700, 300);
			draw = Draw;
			base.OnEnable();
			if (!merged)
				SceneData.Cleanup();

#if DEV
			SetupTestMerge();
#endif

			if (EditorPrefs.HasKey(RowHeightKey))
				selectedRowHeight = EditorPrefs.GetInt(RowHeightKey);

			loading = false;
		}

		protected override void OnDisable() {
			base.OnDisable();
			EditorPrefs.SetInt("RowHeight", selectedRowHeight);
		}

		internal void SetupTestMerge() {
			if (Directory.Exists(UniMergeConfig.DefaultPath + "/Demo/Scene Merge")) {
				var assets = Directory.GetFiles(UniMergeConfig.DefaultPath + "/Demo/Scene Merge");
				foreach (var asset in assets)
					if (asset.EndsWith(".unity")) {
						if (asset.Contains(MyTestSceneName))
							mine = AssetDatabase.LoadAssetAtPath(asset.Replace('\\', '/'), typeof(UnityObject));
						if (asset.Contains(TheirTestSceneName))
							theirs = AssetDatabase.LoadAssetAtPath(asset.Replace('\\', '/'), typeof(UnityObject));
					}
			}
		}

		// ReSharper disable once UnusedMember.Local
		void OnGUI() {
			if (loading) {
				GUILayout.Label("Loading...");
				return;
			}

#if Unity3 || Unity4_0To4_2 //Layout fix for older versions?
#else
			EditorGUIUtility.labelWidth = 150;
#endif

			if (InitGUI())
				return;

#if UNITY_5 || UNITY_5_3_OR_NEWER
			if (mine == null || theirs == null)
#else
			if (mine == null || theirs == null
				|| mine.GetType() != typeof(UnityObject) || mine.GetType() != typeof(UnityObject)
				) //|| !AssetDatabase.GetAssetPath(mine).Contains(".unity") || !AssetDatabase.GetAssetPath(theirs).Contains(".unity"))
#endif
				merged = GUI.enabled = false;

			if (GUILayout.Button("Merge")) {
				loading = true;
				Merge(mine, theirs);
				GUIUtility.ExitGUI();
			}

			GUI.enabled = merged;
			GUILayout.BeginHorizontal();
			{
				GUI.enabled = myContainer;
				if (!GUI.enabled)
					merged = false;

				DrawSaveGUI(true);

				GUI.enabled = theirContainer;
				if (!GUI.enabled)
					merged = false;

				DrawSaveGUI(false);
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			if (GUILayout.Button("Expand Differences")) {
				updateCount = 0;
				totalUpdateNum = GameObjectHelper.GetCount(false, root);
				updateType = RefreshType.Expanding;
				for (var i = 0; i < properties.Count; i++) {
					properties[i].ExpandDifferences();
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Prev Difference")) {
				SelectPreviousDifference();
			}
			if (GUILayout.Button("Next Difference")) {
				SelectNextDifference();
			}
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Refresh")) {
				updateType = RefreshType.Updating;
				update = Refresh();
			}
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUI.enabled = true;
#if !Unity3
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
#endif

#if !(UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4)
			EditorGUI.BeginChangeCheck();
			compareLightingData = EditorGUILayout.Toggle("Compare Lighting Data", compareLightingData);
			if (EditorGUI.EndChangeCheck())
				update = Refresh();
#endif

			DrawRowHeight();

#if !Unity3
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
#endif

			GUILayout.BeginHorizontal();
			{
				DrawObjectFieldGUI(ref myName, ref mine);
				GUILayout.Space(UniMergeConfig.DoubleMidWidth);
				DrawObjectFieldGUI(ref theirName, ref theirs);
			}
			GUILayout.EndHorizontal();

			if (mine == null || theirs == null)
				merged = false;

			if (!merged)
				return;

			CustomScroll(draw);

			ProgressBar();
		}

		void Draw() {
			GUILayout.Space(0); // To provide a rect for ObjectDrawCheck

			var colWidthOption = GUILayout.Width(colWidth);
			var indentOption = GUILayout.Width(1);
			foreach (var property in properties) {
				if (property.property.name == "m_Script")
					continue;

				property.Draw(1, colWidthOption, indentOption);
			}
		}

		void DrawSaveGUI(bool isMine) {
			var name = isMine ? "Mine" : "Theirs";
			var saveAs = GUILayout.Button(string.Format("Save {0} as...", name));
			if (GUILayout.Button(string.Format("Save To {0}", name)) || saveAs) {
				var scene = isMine ? mine : theirs;
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
				var path = AssetDatabase.GetAssetOrScenePath(scene);
#else
				var path = AssetDatabase.GetAssetPath(scene);
#endif

				if (saveAs) {
					var fileName = Path.GetFileNameWithoutExtension(path);
					path = EditorUtility.SaveFilePanelInProject("Save Mine", fileName, "unity", string.Empty);
				}

				if (!string.IsNullOrEmpty(path)) {
					var myContainer = this.myContainer;
					var theirContainer = this.theirContainer;
					DestroyImmediate(isMine ? theirContainer : myContainer);

					var tmp = new List<Transform>();
					var container = isMine ? myContainer : theirContainer;
					foreach (Transform t in container.transform)
						tmp.Add(t);

					foreach (var t in tmp)
						t.parent = null;

					DestroyImmediate(container);

					(isMine ? mySceneData : theirSceneData).ApplySettings();

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
					EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path);
#else
					EditorApplication.SaveScene(path);
#endif
					GUIUtility.ExitGUI();
				}
			}
		}

		void DrawObjectFieldGUI(ref string name, ref UnityObject scene) {
			GUILayout.BeginVertical(GUILayout.Width(colWidth));
			{
				var lastVal = scene;
#if !(Unity3 || Unity4_0To4_2)
				GUILayout.BeginHorizontal();
				name = GUILayout.TextField(name, GUILayout.Width(EditorGUIUtility.labelWidth));
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				scene = EditorGUILayoutExt.ObjectField(scene, typeof(SceneAsset), true);
#else
				scene = EditorGUILayoutExt.ObjectField(scene, typeof(UnityObject), true);
#endif
				GUILayout.EndHorizontal();
#else
				scene = EditorGUILayoutExt.ObjectField(name, scene, typeof(UnityObject), true);
#endif
				if (scene != lastVal) {
					// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
					if (scene)
						name = scene.name;
					else
						name = string.Empty;
				}
			}
			GUILayout.EndVertical();
		}

		// ReSharper disable once UnusedMember.Global
		public static void CliIn() {
			var args = Environment.GetCommandLineArgs();
			foreach (var arg in args)
				Debug.Log(arg);
			Merge(
				args[args.Length - 2].Substring(
					args[args.Length - 2].IndexOf("Assets", StringComparison.Ordinal)).Replace("\\", "/").Trim(),
				args[args.Length - 1].Substring(
					args[args.Length - 1].IndexOf("Assets", StringComparison.Ordinal)).Replace("\\", "/").Trim());
		}

		static void CheckForMessageFile() {
			var mergeFile = (TextAsset) AssetDatabase.LoadAssetAtPath(MessagePath, typeof(TextAsset));
			if (mergeFile) {
				var files = mergeFile.text.Split('\n');
				AssetDatabase.DeleteAsset(MessagePath);
				for (var i = 0; i < files.Length; i++)
					if (!files[i].StartsWith("Assets"))
						if (files[i].IndexOf("Assets", StringComparison.Ordinal) > -1)
							files[i] = files[i].Substring(
								files[i].IndexOf("Assets", StringComparison.Ordinal)).Replace("\\", "/").Trim();
				DoMerge(files);
			}
		}

		static void PrefabMerge(string myPath, string theirPath) {
			((ObjectMerge) GetWindow(typeof(ObjectMerge))).SetGameObjects(
				(GameObject) AssetDatabase.LoadAssetAtPath(myPath, typeof(GameObject)),
				(GameObject) AssetDatabase.LoadAssetAtPath(theirPath, typeof(GameObject)));
		}

		static void DoMerge(string[] paths) {
			if (paths.Length > 2)
				Merge(paths[0], paths[1]);
			else
				Debug.LogError("need at least 2 paths, " + paths.Length + " given");
		}

		/// <summary>
		/// Merge the contents of this window (for testing)
		/// </summary>
		internal void Merge() {
			Merge(mine, theirs);
		}

		static void Merge(UnityObject myScene, UnityObject theirScene) {
			if (myScene == null || theirScene == null)
				return;

			Merge(AssetDatabase.GetAssetPath(myScene), AssetDatabase.GetAssetPath(theirScene));
		}

		static void Merge(string myPath, string theirPath) {
			var window = GetWindow<SceneMerge>(false, "SceneMerge");
			window.updateType = RefreshType.Comparing;
			window.update = window.MergeAsync(myPath, theirPath);
		}

		IEnumerator MergeAsync(string myPath, string theirPath) {
			if (string.IsNullOrEmpty(myPath) || string.IsNullOrEmpty(theirPath))
				yield break;

			if (myPath.EndsWith("prefab") || theirPath.EndsWith("prefab")) {
				PrefabMerge(myPath, theirPath);
				yield break;
			}

			if (AssetDatabase.LoadAssetAtPath(myPath, typeof(UnityObject))
				&& AssetDatabase.LoadAssetAtPath(theirPath, typeof(UnityObject))) {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
#else
				if(EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
#endif

					var enumerator = CollectSceneSettings(myPath, theirPath);
					while (enumerator.MoveNext())
						yield return null;

					MergeScenes(myPath, theirPath);

					RecaptureSettings();

					enumerator = Refresh();
					while (enumerator.MoveNext())
						yield return null;

					yield return null;
					childWindow = (ObjectMerge) GetWindow(typeof(ObjectMerge));
					childWindow.SetGameObjects(myContainer, theirContainer);
					childWindow.update = root.Refresh();
					childWindow.sceneMerge = this;
					childWindow.Repaint();
					yield return null;

					merged = true;
				}
			}
			loading = false;
		}

		static IEnumerator CollectSceneSettings(string myPath, string theirPath) {
			yield return null;
			EditorSceneManager.OpenScene(myPath);
			SceneData.Capture(true);

			yield return null;
			EditorSceneManager.OpenScene(theirPath);
			SceneData.Capture(false);
		}

		void MergeScenes(string myPath, string theirPath) {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
			var myScene = EditorSceneManager.OpenScene(myPath, OpenSceneMode.Additive);
#else
#if UNITY_5
			EditorApplication.NewEmptyScene();
#else
			EditorApplication.NewScene();
			var _allObjects = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));
			foreach (var obj in _allObjects)
				if (obj.transform.parent == null && PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab
					&& PrefabUtility.GetPrefabType(obj) != PrefabType.ModelPrefab
					&& obj.hideFlags == 0) //Want a better way to filter out "internal" objects
					DestroyImmediate(obj);
#endif
			EditorApplication.OpenSceneAdditive(myPath);
#endif

			var split = myPath.Split('/');
			var myContainerName = split[split.Length - 1].Replace(".unity", "");
			this.myContainer = new GameObject { name = myContainerName };
			var myContainer = this.myContainer;
			Undo.RegisterCreatedObjectUndo(myContainer, "UniMerge");

			var myTransform = myContainer.transform;
			var allObjects = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));

			foreach (var obj in allObjects)
				if (obj.transform.parent == null
#if UNITY_2018_3_OR_NEWER
					&& string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj))
#else
					&& PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab
					&& PrefabUtility.GetPrefabType(obj) != PrefabType.ModelPrefab
#endif
					&& obj.hideFlags == 0) //Want a better way to filter out "internal" objects
					obj.transform.SetParent(myTransform, false);

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.MergeScenes(myScene, newScene);
#endif

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var theirScene = EditorSceneManager.OpenScene(theirPath, OpenSceneMode.Additive);
			SceneManager.MergeScenes(theirScene, newScene);
#else
			EditorSceneManager.OpenSceneAdditive(theirPath);
#endif

			split = theirPath.Split('/');
			var theirContainerName = split[split.Length - 1].Replace(".unity", "");
			if (theirContainerName == myContainerName) // Ensure unique name for find all objects below
				theirContainerName += "_theirs";

			this.theirContainer = new GameObject { name = theirContainerName };
			var theirContainer = this.theirContainer;
			Undo.RegisterCreatedObjectUndo(theirContainer, "UniMerge");

			allObjects = (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject));

			foreach (var obj in allObjects)
				if (obj.transform.parent == null && obj != myContainer
#if UNITY_2018_3_OR_NEWER
					&& string.IsNullOrEmpty(AssetDatabase.GetAssetPath(obj))
#else
					&& PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab
					&& PrefabUtility.GetPrefabType(obj) != PrefabType.ModelPrefab
#endif
					&& obj.hideFlags == 0) //Want a better way to filter out "internal" objects
					obj.transform.SetParent(theirContainer.transform, false);
		}

		void RecaptureSettings() {
			mySceneData = SceneData.RecaptureSettings(true);
			mySO = new SerializedObject(mySceneData);

			theirSceneData = SceneData.RecaptureSettings(false);
			theirSO = new SerializedObject(theirSceneData);

			SceneData.Cleanup();
		}

		protected override IEnumerator Refresh() {
			var enumerator = PropertyHelper.UpdatePropertyList(properties, mySO, theirSO, null, null, null, null, this, null, this);
			while (enumerator.MoveNext())
				yield return null;

			foreach (var helper in properties) {
				var children = helper.children;
				foreach (var h in children) {
					if (!h.property.propertyPath.Contains("sunPath"))
						continue;

					foreach (var ph in children) {
						if (!ph.property.propertyPath.Contains("m_Sun"))
							continue;

						if (myContainer) // In case of scene reload
							ph.mine.objectReferenceValue = myContainer.transform.Find(h.mine.stringValue).GetComponent<Light>();

						if (theirContainer)
							ph.theirs.objectReferenceValue = theirContainer.transform.Find(h.theirs.stringValue).GetComponent<Light>();

						var refresh = ph.Refresh();
						while (refresh.MoveNext()) { }
						break;
					}

					h.hidden = true;
					break;
				}
			}
		}

		protected override int GetDrawCount() {
			var count = 0;
			foreach (var property in properties) {
				if (property.hidden)
					continue;

				count += property.GetDrawCount();
			}
			return count;
		}

		protected override void SelectNextRow(bool expandChildren) {
			if (BeginMoveSelect())
				return;

			if ((expandChildren || selected.showChildren) && selected.hasChildren) {
				selected.showChildren = true;
				selected = selected[0];
				return;
			}

			var search = selected;
			var parent = selected.parent;


			int count;
			while (parent != null) {
				if (parent.hasChildren && parent.showChildren) {
					count = parent.count;
					for (var i = 0; i < count; i++) {
						var child = parent[i];
						if (child == search) {
							if (i < count - 1) {
								selected = parent[i + 1];
								if (((PropertyHelper) selected).hidden && i < count - 2)
									selected = selected.parent[i + 2];
								return;
							}
						}
					}
				}

				search = parent;
				parent = parent.parent;
			}

			count = properties.Count;
			for (var i = 0; i < count; i++) {
				if (properties[i] == search && i < count - 1) {
					selected = properties[i + 1];
					break;
				}
			}
		}

		public override void SelectPreviousRow(bool collapseChildren) {
			if (BeginMoveSelect())
				return;

			var parent = selected.parent;
			if (parent == null) {
				for (var i = 0; i < properties.Count; i++) {
					if (properties[i] == selected && i > 0) {
						parent = properties[i - 1];
						while (parent != null) {
							if (parent.hasChildren && parent.showChildren) {
								parent = parent[parent.count - 1];
							} else {
								selected = parent;
								return;
							}
						}
					}
				}
			} else {
				var count = parent.count;
				for (var i = 0; i < count; i++) {
					if (parent[i] == selected) {
						if (i == 0) {
							selected = parent;
							if (collapseChildren)
								selected.showChildren = false;
						} else {
							parent = parent[i - 1];
							if (((PropertyHelper) parent).hidden)
								parent = i > 1 ? parent.parent[i - 2] : null;

							while (parent != null) {
								selected = parent;
								if (parent.hasChildren && parent.showChildren && parent.count > 1)
									parent = parent[parent.count - 1];
								else
									parent = null;
							}
						}

						return;
					}
				}
			}
		}

		protected override void SelectNextDifference() {
			if (BeginMoveSelect())
				return;

			HelperStack.Clear();
			var found = false;
			var count = properties.Count;
			for (var i = count - 1; i >= 0; i--)
				HelperStack.Push(new StackHelper(properties[i], 0));

			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var index = pop.siblingIndex;
				if (index == 0 && !helper.hidden) {
					if (found && !helper.Same) {
						selected = helper;
						ExpandParents(selected);
						ScrollToRow(selected);
						return;
					}

					if (helper == selected)
						found = true;
				}

				if (helper.hasChildren && index < helper.count) {
					// Enter property child
					HelperStack.Push(new StackHelper(helper, index + 1));
					HelperStack.Push(new StackHelper(helper.children[index], 0));
				}
			}
		}

		protected override void SelectPreviousDifference() {
			if (BeginMoveSelect())
				return;

			HelperStack.Clear();
			Helper lastMatch = null;
			var count = properties.Count;
			for (var i = count - 1; i >= 0; i--)
				HelperStack.Push(new StackHelper(properties[i], 0));

			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var index = pop.siblingIndex;
				if (index == 0 && !helper.hidden) {
					if (helper == selected && lastMatch != null) {
						selected = lastMatch;
						ExpandParents(selected);
						ScrollToRow(selected);
						return;
					}

					if (!helper.Same)
						lastMatch = helper;
				}

				if (helper.hasChildren && index < helper.count) {
					// Enter property child
					HelperStack.Push(new StackHelper(helper, index + 1));
					HelperStack.Push(new StackHelper(helper.children[index], 0));
				}
			}
		}

		protected override void SelectNextRowWithChildren(bool expandComponents) {
			if (BeginMoveSelect())
				return;

			HelperStack.Clear();
			var found = false;
			var count = properties.Count;
			for (var i = count - 1; i >= 0; i--)
				HelperStack.Push(new StackHelper(properties[i], 0));

			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var index = pop.siblingIndex;
				if (index == 0 && !helper.hidden) {
					if (found && helper.hasChildren) {
						selected = helper;
						ScrollToRow(selected);
						return;
					}

					if (helper == selected)
						found = true;
				}

				if (helper.hasChildren && index < helper.count) {
					// Enter child
					HelperStack.Push(new StackHelper(helper, index + 1));
					HelperStack.Push(new StackHelper(helper.children[index], 0));
				}
			}
		}

		protected override void SelectParentRow(bool collapseComponents) {
			if (BeginMoveSelect())
				return;

			if (selected.parent != null)
				selected = selected.parent;
		}

		static void ExpandParents(Helper helper) {
			var parent = helper.parent;
			while (parent != null) {
				parent.showChildren = true;
				parent.InvalidateDrawCount();
				helper = parent;
				parent = helper.parent;
			}
		}

		protected override void ToggleSelectedComponentSection() { }

		bool BeginMoveSelect() {
			if (selected == null) {
				SelectRow(0);
				return true;
			}

			return false;
		}

		protected override void ScrollToRow(Helper row) {
			var drawCount = 0;

			//Clear stack because of early-out
			HelperStack.Clear();
			var count = properties.Count;
			for (var i = count - 1; i >= 0; i--)
				HelperStack.Push(new StackHelper(properties[i], 0));

			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;

				if (helper == row) {
					ScrollToRow(drawCount);
					return;
				}

				var index = pop.siblingIndex;
				if (index == 0 && !helper.hidden)
					drawCount++;

				if (helper.hasChildren && helper.showChildren && index < helper.count) {
					HelperStack.Push(new StackHelper(helper, index + 1));
					HelperStack.Push(new StackHelper(helper.children[index], 0));
				}
			}
		}

		protected override void SelectRow(int row) {
			row += (int)objectDrawOffset;
			var drawCount = 0;

			//Clear stack because of early-out
			HelperStack.Clear();
			var count = properties.Count;
			for (var i = count - 1; i >= 0; i--)
				HelperStack.Push(new StackHelper(properties[i], 0));

			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;

				var index = pop.siblingIndex;

				if (index == 0 && !helper.hidden) {
					if (drawCount == row) {
						selected = helper;
						return;
					}

					drawCount++;
				}

				if (helper.hasChildren && helper.showChildren && index < helper.count) {
					HelperStack.Push(new StackHelper(helper, index + 1));
					HelperStack.Push(new StackHelper(helper.children[index], 0));
				}
			}
		}
	}
}
