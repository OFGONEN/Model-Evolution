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
//ObjectMerge Window

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#define Unity4_0To4_2
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UniMerge.Editor.Helpers;
using UnityEditor;
using UnityEngine;

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
using PrefabUtility = UnityEditor.EditorUtility;
#endif

namespace UniMerge.Editor.Windows {
	public class ObjectMerge : UniMergeWindow {
		struct StackHelper {
			public readonly Helper helper;
			public readonly int siblingIndex;
			public readonly int componentIndex;

			public StackHelper(Helper helper, int siblingIndex, int componentIndex) {
				this.helper = helper;
				this.siblingIndex = siblingIndex;
				this.componentIndex = componentIndex;
			}
		}

		internal const string MineTestName = "Mine", TheirsTestName = "Theirs";

		string mineName = string.Empty, theirName = string.Empty;

#if Unity3
		static string filters = "", lastFilters;
		public static List<System.Type> filterTypes;
		List<string> badTypes;
		List<string> notComponents;
		System.Reflection.Assembly[] assemblies;
#else
		public static Type[][] componentTypes { get; private set; }
		string[][] componentTypeStrings;
#endif

		public bool deepCopy { get; private set; }
		public bool log { get; private set; }
		public bool compareAttributes { get; private set; }
		public int[] typeMask { get; private set; }

		public GameObjectHelper root { get; private set; }

		public SceneMerge sceneMerge;

#if !Unity3
		static readonly GUILayoutOption Width75 = GUILayout.Width(75);
#endif
		Action draw;

		protected override int headerSize { get { return 167; } }

		static readonly Stack<StackHelper> HelperStack = new Stack<StackHelper>();

#if UNITY_EDITOR_OSX // Minimize window conflicts with cmd-m
		[MenuItem("Window/UniMerge/Object Merge %#m")]
#else
		[MenuItem("Window/UniMerge/Object Merge %m")]
#endif
		static void Init() {
			GetWindow(typeof(ObjectMerge), false, "ObjectMerge");
		}

		protected override void OnEnable() {
			minSize = new Vector2(600, 300);
			draw = Draw;
			base.OnEnable();
			blockRefresh = false;
			root = new GameObjectHelper(this);

#if Unity3
			//Component filters
			assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
			//NB: For some reason, after a compile, filters starts out as "", though the field retains the value.  Then when it's modified the string is set... as a result sometime you see filter text with nothing being filtered
			ParseFilters();
#else
			//Component filters
			var subclasses = new List<Type>();
			try {
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
					try {
						// ReSharper disable once LoopCanBeConvertedToQuery
						foreach (var type in assembly.GetTypes()) {
							if (type.IsSubclassOf(typeof(Component)))
								subclasses.Add(type);
						}
					} catch (Exception e) {
						Debug.LogError(string.Format("Error calling GetTypes() on {0} while setting up Component Filters. Please check your project setup. Error message: {1}",
							assembly.FullName, e.Message));
					}
				}
			} catch (Exception e) {
				Debug.LogError(string.Format("Error calling GetAssemblies() on {0} while setting up Component Filters. Please check your project setup. Error message: {1}",
					AppDomain.CurrentDomain.FriendlyName, e.Message));
			}

			var compTypeStrs = new List<List<string>> { new List<string>() };
			var compTypes = new List<List<Type>> { new List<Type>() };
			var setCount = 0;
			foreach (var t in subclasses) {
				if (compTypes[setCount].Count == 31) {
					setCount++;
					compTypeStrs.Add(new List<string>());
					compTypes.Add(new List<Type>());
				}
				compTypeStrs[setCount].Add(t.Name);
				compTypes[setCount].Add(t);
			}
			var types = new Type[setCount + 1][];
			componentTypes = types;
			componentTypeStrings = new string[setCount + 1][];
			var mask = new int[setCount + 1];
			typeMask = mask;
			for (var i = 0; i < setCount + 1; i++) {
				mask[i] = -1;
				types[i] = compTypes[i].ToArray();
				componentTypeStrings[i] = compTypeStrs[i].ToArray();
			}
#endif

#if DEV
			SetGameObjects(GameObject.Find(MineTestName), GameObject.Find(TheirsTestName));
#else
			SetGameObjects(null, null);
#endif

			deepCopy = true;
			compareAttributes = true;

			update = Refresh();

			if (EditorPrefs.HasKey(RowHeightKey))
				selectedRowHeight = EditorPrefs.GetInt(RowHeightKey);
		}

		protected override IEnumerator Refresh() {
			//This is where we initiate the merge for the first time
			var root = this.root;
			root.components.Clear();
			root.attributes.Clear();
			var children = root.children;
			if (children != null)
				children.Clear();

			var rootMine = root.mine;
			var rootTheirs = root.theirs;
			if (!rootMine || !rootTheirs)
				yield break;

#if !Unity3
			//Check if the objects are prefabs
#if UNITY_2018_3_OR_NEWER
			if (PrefabUtility.IsPartOfPrefabAsset(rootMine)) {
				switch (PrefabUtility.GetPrefabAssetType(rootMine)) {
					case PrefabAssetType.Model:
					case PrefabAssetType.Regular:
						if (PrefabUtility.IsPartOfPrefabAsset(rootTheirs)) {
							switch (PrefabUtility.GetPrefabAssetType(rootTheirs)) {
								case PrefabAssetType.Model:
								case PrefabAssetType.Regular:
#else
			switch (PrefabUtility.GetPrefabType(rootMine)) {
				case PrefabType.ModelPrefab:
				case PrefabType.Prefab:
					switch (PrefabUtility.GetPrefabType(rootTheirs)) {
						case PrefabType.ModelPrefab:
						case PrefabType.Prefab:
#endif
									if (EditorUtility.DisplayDialog("Instantiate prefabs?",
										"In order to merge prefabs, you must instantiate them and merge the instances. Then you must apply the changes.",
										"Instantiate", "Cancel"))
										SetGameObjects(PrefabUtility.InstantiatePrefab(rootMine) as GameObject,
											PrefabUtility.InstantiatePrefab(rootTheirs) as GameObject);
									else
										SetGameObjects(null, null);
									break;
								default:
									Debug.LogWarning("Sorry, you must compare a prefab with a prefab");
									break;
							}
#if UNITY_2018_3_OR_NEWER
						}
#endif

						break;
#if UNITY_2018_3_OR_NEWER
					case PrefabAssetType.NotAPrefab:
						switch (PrefabUtility.GetPrefabAssetType(rootTheirs)) {
							case PrefabAssetType.NotAPrefab:
#else
				case PrefabType.DisconnectedPrefabInstance:
				case PrefabType.PrefabInstance:
				case PrefabType.ModelPrefabInstance:
				case PrefabType.None:
					switch (PrefabUtility.GetPrefabType(rootTheirs)) {
						case PrefabType.DisconnectedPrefabInstance:
						case PrefabType.PrefabInstance:
						case PrefabType.ModelPrefabInstance:
						case PrefabType.None:
#endif
								break;
							default:
								Debug.LogWarning("Sorry, this prefab type is not supported");
								break;
						}

						break;
					default:
						Debug.LogWarning("Sorry, this prefab type is not supported");
						break;
				}
#if UNITY_2018_3_OR_NEWER
			}
#endif
#endif

				updateType = RefreshType.Updating;
			update = root.Refresh();
		}

		public void SetGameObjects(GameObject mine, GameObject theirs) {
			root.SetGameObjects(mine, theirs);

			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (mine)
				mineName = mine.name;
			else
				mineName = string.Empty;

			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (theirs)
				theirName = theirs.name;
			else
				theirName = string.Empty;
		}

		protected override int GetDrawCount() {
			return root.GetDrawCount();
		}

		// ReSharper disable once UnusedMember.Local
		void OnGUI() {
			if (InitGUI())
				return;

			/*
			 * BEGIN GUI
			 */

			var root = this.root;
			GUILayout.BeginHorizontal();
			{
				/*
				 * Options
				 */
				GUILayout.BeginVertical();
				{
					const string tooltip = "When enabled, copying GameObjects or Components will search for references"
						+ " to them and try to set them.  Disable if you do not want this behavior or if the window "
						+ "locks up on copy (too many objects)";
					deepCopy = EditorGUILayout.Toggle(new GUIContent("Deep Copy", tooltip), deepCopy);
				}

				{
					const string tooltip = "When enabled, non-obvious events (like deep copy reference setting) will be logged";
					log = EditorGUILayout.Toggle(new GUIContent("Log", tooltip), log);
				}

				{
					const string tooltip = "When disabled, attributes will not be included in comparison algorithm."
						+ "  To choose which components are included, use the drop-downs to the right.";
					compareAttributes = EditorGUILayout.Toggle(new GUIContent("Compare Attributes", tooltip), compareAttributes);
				}

				GUI.enabled = !IsUpdating();
				if (GUILayout.Button("Expand Differences")) {
					updateCount = 0;
					totalUpdateNum = GameObjectHelper.GetCount(false, root);
					updateType = RefreshType.Expanding;
					update = root.ExpandDifferences();
				}

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Prev Difference")) {
					SelectPreviousDifference();
				}
				if (GUILayout.Button("Next Difference")) {
					SelectNextDifference();
				}
				GUILayout.EndHorizontal();

				if (GUILayout.Button("Refresh"))
					update = root.BubbleRefresh();

				GUI.enabled = true;

				DrawRowHeight();

				GUILayout.Space(10); //Padding between controls and merge space
				GUILayout.EndVertical();

				/*
				 * Comparison Filters
				 */
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();

#if !Unity3
				GUILayout.FlexibleSpace();
#endif

#if Unity3
				GUILayout.BeginVertical();
				GUILayout.Label("Enter a list of component types to exclude, separated by commas");
				filters = EditorGUILayout.TextField("Filters", filters);
				if(filters != lastFilters){
					ParseFilters();
				}
				lastFilters = filters;
				string filt = "Filtering: ";
				if(filterTypes.Count > 0){
					foreach(System.Type bad in filterTypes)
						filt += bad.Name + ", ";
					GUILayout.Label(filt.Substring(0, filt.Length - 2));
				}
				string err = "Sorry, the following types are invalid: ";
				if(badTypes.Count > 0){
					foreach(string bad in badTypes)
						err += bad + ", ";
					GUILayout.Label(err.Substring(0, err.Length - 2));
				}
				string cerr = "Sorry, the following types aren't components: ";
				if(notComponents.Count > 0){
					foreach(string bad in notComponents)
						cerr += bad + ", ";
					GUILayout.Label(cerr.Substring(0, cerr.Length - 2));
				}
				GUILayout.EndVertical();
#else
				GUILayout.Label(new GUIContent("Comparison Filters",
					"Select which components should be included in"
					+ " the comparison. You can't filter more"
					+ " than 31 things :("));
				if (componentTypeStrings != null) {
					var mask = typeMask;
					for (var i = 0; i < componentTypeStrings.Length; i++) {
						mask[i] = EditorGUILayout.MaskField(mask[i], componentTypeStrings[i], Width75);
						if (i % 3 == 2) {
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal();
							GUILayout.FlexibleSpace();
						}
					}
				}
#endif
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			var colWdithOption = GUILayout.Width(colWidth);
			var mine = root.mine;
			var theirs = root.theirs;
			var lastVal = mine;
			GUILayout.BeginVertical(colWdithOption);
#if !(Unity3 || Unity4_0To4_2)
			GUILayout.BeginHorizontal();
			var labelWidthOption = GUILayout.Width(EditorGUIUtility.labelWidth);
			mineName = GUILayout.TextField(mineName, labelWidthOption);
			mine = (GameObject) EditorGUILayoutExt.ObjectField(mine, typeof(GameObject), true);
			GUILayout.EndHorizontal();
#else
			mine = (GameObject) EditorGUILayoutExt.ObjectField(mineName, mine, typeof(GameObject), true);
#endif
			if (mine != lastVal) {
				SetGameObjects(mine, theirs);
				if (!blockRefresh)
					update = Refresh();
			}

			lastVal = theirs;
			GUILayout.EndVertical();
			GUILayout.Space(UniMergeConfig.DoubleMidWidth);
			GUILayout.BeginVertical(colWdithOption);
#if !(Unity3 || Unity4_0To4_2)
			GUILayout.BeginHorizontal();
			theirName = GUILayout.TextField(theirName, labelWidthOption);
			theirs = (GameObject) EditorGUILayoutExt.ObjectField(theirs, typeof(GameObject), true);
			GUILayout.EndHorizontal();
#else
			theirs = (GameObject)EditorGUILayoutExt.ObjectField(theirName, theirs, typeof(GameObject), true);
#endif
			if (theirs != lastVal) {
				SetGameObjects(mine, theirs);
				if (!blockRefresh)
					update = Refresh();
			}

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

#if !(Unity3 || Unity4_0To4_2)
			EditorGUIUtility.labelWidth = 75; //Make labels just a bit tighter for compactness
#endif
			if (mine && theirs)
				CustomScroll(draw);

			ProgressBar();
		}

		void Draw() {
			root.Draw(colWidth);
		}

		protected override void SelectNextRow(bool expandChildren) {
			if (BeginMoveSelect())
				return;

			var helper = selected as GameObjectHelper;
			if (helper != null && helper.showComponents) {
				if (helper.attributesSelected) {
					if (helper.showAttributes)
						selected = helper.attributes[0];
					else
						selected = helper.components[0];
				} else { helper.attributesSelected = true; }

				return;
			}

			if ((expandChildren || selected.showChildren) && selected.hasChildren) {
				selected.showChildren = true;
				selected = selected[0];
				return;
			}

			var search = selected;
			var parent = selected.parent;
			var isComponent = selected is ComponentHelper;
			if (isComponent) {
				var components = ((GameObjectHelper) parent).components;
				var count = components.Count;
				for (var i = 0; i < count; i++) {
					var child = components[i];
					if (child == search && i < count - 1) {
						selected = components[i + 1];
						return;
					}
				}

				if (parent.hasChildren) {
					selected = parent[0];
					return;
				}
			}

			var isProperty = selected is PropertyHelper;
			if (isProperty) {
				helper = parent as GameObjectHelper;
				if (helper != null) {
					var attributes = helper.attributes;
					var count = attributes.Count;
					for (var i = 0; i < count; i++) {
						var child = attributes[i];
						if (child == search) {
							if (i == count - 1)
								selected = helper.components[0];
							else
								selected = attributes[i + 1];

							return;
						}
					}
				}
			}

			while (parent != null) {
				if (parent.hasChildren && parent.showChildren) {
					var count = parent.count;
					for (var i = 0; i < count; i++) {
						var child = parent[i];
						if (child == search) {
							if (i == count - 1) {
								if (parent is ComponentHelper) {
									search = parent;
									parent = parent.parent;
									var components = ((GameObjectHelper) parent).components;
									var componentCount = components.Count;
									for (var j = 0; j < componentCount; j++) {
										var component = components[j];
										if (component == search && j < componentCount - 1) {
											selected = components[j + 1];
											return;
										}
									}

									if (parent.hasChildren && parent.showChildren) {
										selected = parent[0];
										return;
									}
								}
							} else {
								selected = parent[i + 1];
								return;
							}
						}
					}
				}

				search = parent;
				parent = parent.parent;
			}
		}

		public override void SelectPreviousRow(bool collapseChildren) {
			if (BeginMoveSelect())
				return;

			var helper = selected as GameObjectHelper;
			if (helper != null && helper.showComponents) {
				if (helper.attributesSelected) {
					helper.attributesSelected = false;
					return;
				}
			}

			int count;
			var search = selected;
			var parent = selected.parent;
			var isComponent = selected is ComponentHelper;
			if (isComponent) {
				helper = (GameObjectHelper) parent;
				var components = helper.components;
				count = components.Count;
				for (var i = 0; i < count; i++) {
					var child = components[i];
					if (child == search) {
						if (i == 0) {
							if (helper.showAttributes) {
								var attributes = helper.attributes;
								selected = attributes[attributes.Count - 1];
							} else {
								selected = helper;
								helper.attributesSelected = true;
							}
						} else {
							selected = components[i - 1];
							while (selected.showChildren && selected.hasChildren)
								selected = selected[selected.count - 1];
						}

						return;
					}
				}
			} else {
				if (parent != null) {
					var isProperty = selected is PropertyHelper;
					helper = parent as GameObjectHelper;
					if (isProperty && helper != null) {
						var attributes = helper.attributes;
						count = attributes.Count;
						for (var i = 0; i < count; i++) {
							var child = attributes[i];
							if (child == search) {
								if (i == 0) {
									selected = helper;
									helper.attributesSelected = true;
								} else {
									selected = attributes[i - 1];
								}

								return;
							}
						}
					}

					count = parent.count;
					for (var i = 0; i < count; i++) {
						if (parent[i] == selected) {
							if (i == 0) {
								selected = parent;
								if (collapseChildren)
									selected.showChildren = false;
							} else {
								parent = parent[i - 1];

								while (parent != null) {
									selected = parent;
									if (parent.hasChildren && parent.showChildren && parent.count > 1)
										parent = parent[parent.count - 1];
									else
										parent = null;
								}
							}

							helper = selected as GameObjectHelper;
							if (helper != null && helper.showComponents) {
								var components = helper.components;
								selected = components[components.Count - 1];
								while (selected.showChildren && selected.hasChildren)
									selected = selected[selected.count - 1];
							}

							return;
						}
					}
				}
			}
		}

		protected override void SelectNextDifference() {
			if (BeginMoveSelect())
				return;

			var gameObject = selected as GameObjectHelper;
			if (gameObject != null && gameObject.mine && gameObject.theirs) {
				if (!gameObject.sameAttributes) {
					if (!gameObject.attributesSelected) {
						gameObject.showComponents = true;
						gameObject.attributesSelected = true;
						gameObject.InvalidateDrawCount();
						ScrollToRow(selected);
						return;
					}

					var attributes = gameObject.attributes;
					var count = attributes.Count;
					for (var i = 0; i < count; i++) {
						var attribute = attributes[i];
						if (!attribute.Same) {
							selected = attribute;
							ExpandParents(selected);
							ScrollToRow(selected);
							return;
						}
					}
				}
			}

			HelperStack.Clear();
			var found = false;
			HelperStack.Push(new StackHelper(root, 0, 0));
			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var index = pop.siblingIndex;
				var componentIndex = pop.componentIndex;
				gameObject = helper as GameObjectHelper;
				var isGameObject = gameObject != null;
				if (index == 0 && componentIndex == 0) {
					if (found && !helper.Same && (helper is ComponentHelper && helper.parent.hasBoth || isGameObject || helper.hasBoth)) {
						if (isGameObject)
							gameObject.attributesSelected = false;
						selected = helper;
						ExpandParents(selected);
						ScrollToRow(selected);
						return;
					}

					if (helper == selected)
						found = true;
				}

				if (isGameObject) {
					var components = gameObject.components;
					if (componentIndex < components.Count) {
						if (index == 0 && componentIndex == 0 && helper.hasBoth) {
							var attributes = gameObject.attributes;
							var count = attributes.Count;
							for (var i = 0; i < count; i++) {
								var attribute = attributes[i];
								if (found && !attribute.Same) {
									selected = attribute;
									ExpandParents(selected);
									ScrollToRow(selected);
									return;
								}

								if (attribute == selected)
									found = true;
							}
						}

						// Enter component
						HelperStack.Push(new StackHelper(helper, index, componentIndex + 1));
						HelperStack.Push(new StackHelper(gameObject.components[componentIndex], 0, 0));
					} else if (helper.hasChildren && index < helper.count) {
						//Enter child
						HelperStack.Push(new StackHelper(helper, index + 1, componentIndex));
						HelperStack.Push(new StackHelper(helper[index], 0, 0));
					}
				} else if (helper.hasChildren && index < helper.count) {
					// Enter property child
					HelperStack.Push(new StackHelper(helper, index + 1, 0));
					HelperStack.Push(new StackHelper(helper[index], 0, 0));
				}
			}
		}

		protected override void SelectPreviousDifference() {
			if (BeginMoveSelect())
				return;

			var gameObject = selected as GameObjectHelper;
			var isGameObject = gameObject != null;
			if (isGameObject && gameObject.attributesSelected) {
				gameObject.attributesSelected = false;
				ScrollToRow(selected);
				return;
			}

			HelperStack.Clear();
			Helper lastMatch = null;
			HelperStack.Push(new StackHelper(root, 0, 0));
			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var index = pop.siblingIndex;
				var componentIndex = pop.componentIndex;
				gameObject = helper as GameObjectHelper;
				isGameObject = gameObject != null;
				if (index == 0 && componentIndex == 0) {
					if (helper == selected && lastMatch != null) {
						selected = lastMatch;
						ExpandParents(selected);
						ScrollToRow(selected);
						return;
					}

					if (!helper.Same && (isGameObject || helper is ComponentHelper && helper.parent.hasBoth || helper.hasBoth))
						lastMatch = helper;

					if (isGameObject && helper.hasBoth) {
						var attributes = gameObject.attributes;
						var count = attributes.Count;
						for (var i = 0; i < count; i++) {
							var attribute = attributes[i];
							if (!attribute.Same)
								lastMatch = attribute;
						}
					}
				}

				if (isGameObject) {
					var components = gameObject.components;
					if (componentIndex < components.Count) {
						if (index == 0 && componentIndex == 0 && helper.hasBoth) {
							var attributes = gameObject.attributes;
							var count = attributes.Count;
							var found = false;
							for (var i = count - 1; i >= 0; i--) {
								var attribute = attributes[i];
								if (found && !attribute.Same) {
									selected = attribute;
									ExpandParents(selected);
									ScrollToRow(selected);
									return;
								}

								if (attribute == selected)
									found = true;
							}

							if (found) {
								selected = gameObject;
								gameObject.attributesSelected = true;
								ExpandParents(selected);
								ScrollToRow(selected);
								return;
							}
						}

						// Enter component
						HelperStack.Push(new StackHelper(helper, index, componentIndex + 1));
						HelperStack.Push(new StackHelper(gameObject.components[componentIndex], 0, 0));
					} else if (helper.hasChildren && index < helper.count) {
						//Enter child
						HelperStack.Push(new StackHelper(helper, index + 1, componentIndex));
						HelperStack.Push(new StackHelper(helper[index], 0, 0));
					}
				} else if (helper.hasChildren && index < helper.count) {
					// Enter property child
					HelperStack.Push(new StackHelper(helper, index + 1, 0));
					HelperStack.Push(new StackHelper(helper[index], 0, 0));
				}
			}
		}

		protected override void SelectNextRowWithChildren(bool expandComponents) {
			if (BeginMoveSelect())
				return;

			HelperStack.Clear();
			var found = false;
			HelperStack.Push(new StackHelper(root, 0, 0));
			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var index = pop.siblingIndex;
				var componentIndex = pop.componentIndex;
				if (index == 0 && componentIndex == 0) {
					if (found && helper.hasChildren) {
						selected = helper;
						ScrollToRow(selected);
						return;
					}

					if (helper == selected)
						found = true;
				}

				var gameObject = helper as GameObjectHelper;
				if (gameObject != null && expandComponents) {
					var components = gameObject.components;
					if (componentIndex < components.Count) {
						if (index == 0 && componentIndex == 0 && !gameObject.attributesSelected && found) {
							selected = gameObject;
							ScrollToRow(selected);
							gameObject.showComponents = true;
							gameObject.attributesSelected = true;
							return;
						}

						// Enter component
						HelperStack.Push(new StackHelper(helper, index, componentIndex + 1));
						HelperStack.Push(new StackHelper(gameObject.components[componentIndex], 0, 0));
					} else if (helper.hasChildren && index < helper.count) {
						//Enter child
						HelperStack.Push(new StackHelper(helper, index + 1, componentIndex));
						HelperStack.Push(new StackHelper(helper[index], 0, 0));
					}
				} else if (helper.hasChildren && index < helper.count) {
					// Enter child
					HelperStack.Push(new StackHelper(helper, index + 1, 0));
					HelperStack.Push(new StackHelper(helper[index], 0, 0));
				}
			}
		}

		protected override void SelectParentRow(bool collapseComponents) {
			if (BeginMoveSelect())
				return;

			var gameObject = selected as GameObjectHelper;
			if (gameObject != null && (collapseComponents || gameObject.parent == null)) {
				gameObject.showComponents = false;
				gameObject.attributesSelected = false;
			}

			if (selected.parent != null)
				selected = selected.parent;

		}

		static void ExpandParents(Helper helper) {
			var parent = helper.parent;
			while (parent != null) {
				var gameObject = parent as GameObjectHelper;
				if (gameObject != null) {
					if (helper is PropertyHelper)
						gameObject.showComponents = gameObject.showAttributes = true;
					else if (helper is ComponentHelper)
						gameObject.showComponents = true;
					else
						parent.showChildren = true;
				} else {
					parent.showChildren = true;
				}

				parent.InvalidateDrawCount();
				helper = parent;
				parent = helper.parent;
			}
		}

		protected override void ToggleSelectedComponentSection() {
			var helper = selected as GameObjectHelper;
			if (helper != null) {
				helper.showComponents = !helper.showComponents;
				helper.InvalidateDrawCount();
			}
		}

		bool BeginMoveSelect() {
			if (selected == null) {
				SelectRow(0);
				return true;
			}

			return false;
		}

		protected override void ScrollToRow(Helper row) {
			var drawCount = 0;

			var gameObjectRow = row as GameObjectHelper;
			HelperStack.Clear();
			HelperStack.Push(new StackHelper(root, 0, 0));
			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;

				if (helper == row) {
					if (gameObjectRow != null && gameObjectRow.attributesSelected && gameObjectRow.showComponents)
						drawCount++;

					ScrollToRow(drawCount);
					return;
				}

				var index = pop.siblingIndex;
				var componentIndex = pop.componentIndex;
				if (index == 0 && componentIndex == 0)
					drawCount++;

				var gameObject = helper as GameObjectHelper;
				if (gameObject != null) {
					var components = gameObject.components;
					if (gameObject.showComponents && componentIndex < components.Count) {
						if (componentIndex == 0) {
							drawCount++; // Add one for attribute row
							if (gameObject.showAttributes) {
								var attributes = gameObject.attributes;
								var count = attributes.Count;
								for (var i = 0; i < count; i++) {
									if (attributes[i] == row) {
										ScrollToRow(drawCount);
										return;
									}

									drawCount++;
								}
							}
						}

						HelperStack.Push(new StackHelper(helper, index, componentIndex + 1));
						HelperStack.Push(new StackHelper(gameObject.components[componentIndex], 0, 0));
					} else if (helper.hasChildren && helper.showChildren && index < helper.count) {
						HelperStack.Push(new StackHelper(helper, index + 1, componentIndex));
						HelperStack.Push(new StackHelper(helper[index], 0, 0));
					}
				} else if (helper.hasChildren && helper.showChildren && index < helper.count) {
					HelperStack.Push(new StackHelper(helper, index + 1, 0));
					HelperStack.Push(new StackHelper(helper[index], 0, 0));
				}
			}
		}

		protected override void SelectRow(int row) {
			row += (int)objectDrawOffset;
			var drawCount = 0;

			//Clear stack because of early-out
			HelperStack.Clear();
			HelperStack.Push(new StackHelper(root, 0, 0));
			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var gameObject = helper as GameObjectHelper;

				var index = pop.siblingIndex;
				var componentIndex = pop.componentIndex;

				if (index == 0 && componentIndex == 0) {
					var attributeRow = gameObject != null && drawCount + 1 == row && gameObject.showComponents;
					var match = drawCount == row || attributeRow;

					if (match) {
						selected = helper;
						if (gameObject != null)
							gameObject.attributesSelected = attributeRow;
						return;
					}

					drawCount++;
				}

				if (gameObject != null) {
					var components = gameObject.components;
					if (gameObject.showComponents && componentIndex < components.Count) {
						if (componentIndex == 0) {
							drawCount++; // Add one for attribute row
							if (gameObject.showAttributes) {
								var attributes = gameObject.attributes;
								var count = attributes.Count;
								for (var i = 0; i < count; i++) {
									if (drawCount == row) {
										selected = attributes[i];
										return;
									}

									drawCount++;
								}
							}
						}

						HelperStack.Push(new StackHelper(helper, index, componentIndex + 1));
						HelperStack.Push(new StackHelper(gameObject.components[componentIndex], 0, 0));
					} else if (helper.hasChildren && helper.showChildren && index < helper.count) {
						HelperStack.Push(new StackHelper(helper, index + 1, componentIndex));
						HelperStack.Push(new StackHelper(helper[index], 0, 0));
					}
				} else if (helper.hasChildren && helper.showChildren && index < helper.count) {
					HelperStack.Push(new StackHelper(helper, index + 1, componentIndex));
					HelperStack.Push(new StackHelper(helper[index], 0, 0));
				}
			}
		}

#if Unity3
		void ParseFilters(){
			filterTypes = new List<System.Type>();
			badTypes = new List<string>();
			notComponents = new List<string>();
			string[] tmp = filters.Replace(" ", "").Split(',');
			foreach(string filter in tmp){
				if(!string.IsNullOrEmpty(filter)){
					bool found = false;
					foreach(System.Reflection.Assembly asm in assemblies){
						foreach(System.Type t in asm.GetTypes()){
							if(t.Name.ToLower() == filter.ToLower()){
								if(t.IsSubclassOf(typeof(Component))){
									filterTypes.Add(t);
								} else notComponents.Add(filter);
								found = true;
								break;
							}
						}
						if(found)
							break;
					}
					if(!found)
						badTypes.Add(filter);
				}
			}
		}
#endif
	}
}
