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
//ObjectHelper class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UniMerge.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
using PrefabUtility = UnityEditor.EditorUtility;
#endif

namespace UniMerge.Editor.Helpers {
	public class GameObjectHelper : Helper {
		struct StackHelper {
			public readonly GameObjectHelper helper;
			public readonly int siblingIndex;

			public StackHelper(GameObjectHelper helper, int siblingIndex) {
				this.helper = helper;
				this.siblingIndex = siblingIndex;
			}
		}

		const string PrefabDialogTitle = "Cannot restructure Prefab instance";
		const string PrefabDialogText =
			"Children of a Prefab instance cannot be deleted or moved, and components "
			+ "cannot be reordered.\nYou can open the Prefab in Prefab Mode to restructure "
			+ "the Prefab Asset itself, or unpack the Prefab instance to remove its Prefab connection.";
		const string PrefabDialogConfirm = "Open Prefab";
		const string PrefabDialogCancel = "Cancel";

		const int ObjectWithChildrenPadding = 2;
		const int ObjectWithoutChildrenPadding = 3;
		const int FandPButtons = 40;
#if Unity3
		const int ObjectPadding = -1;
#else
		const int ObjectPadding = -3;
#endif

		public readonly List<ComponentHelper> components = new List<ComponentHelper>(1);
		public readonly List<PropertyHelper> attributes = new List<PropertyHelper>(7);
		public List<GameObjectHelper> children { get; private set; }
		public bool attributesSelected;

		ObjectContainer objectContainer;
		SerializedProperty mineProp, theirsProp;

		static readonly GUILayoutOption Width19 = GUILayout.Width(19);

		public GameObject mine { get { return objectContainer.mine; } }
		public GameObject theirs { get { return objectContainer.theirs; } }
		public override bool invalid { get { return mine == null && theirs == null; } }

		protected override bool hasMine { get { return mine; } }
		protected override bool hasTheirs { get { return theirs; } }

		public override bool hasChildren { get { return children != null; } }
		public override Helper this[int i] { get { return children[i]; } }
		public override int count { get { return children.Count; } }

		bool sameComponents;
		public bool sameAttributes;
		public bool showComponents;
		public bool showAttributes;

		new readonly GameObjectHelper parent; // GameObjectHelpers can only be children of each other
		new readonly ObjectMerge window; // GameObjectHelpers can only be in ObjectMerge windows

		SerializedObject mySO, theirSO;

#if UNITY_2018_3_OR_NEWER
		
#endif

		//Used as arguments to static delegated methods
		static GameObject gameObjectArg;
		static GameObjectHelper thisArg;
		static float indentArg;

		//Local use only for GC. Might break during multiple simultaneous refresh because of shared static instance
		static readonly List<GameObject> MyChildren = new List<GameObject>();
		static readonly List<GameObject> TheirChildren = new List<GameObject>();
		static readonly List<Component> MyComponents = new List<Component>();
		static readonly List<Component> TheirComponents = new List<Component>();
		static readonly List<GameObject> MineList = new List<GameObject>();
		static readonly List<GameObject> TheirsList = new List<GameObject>();
		static readonly List<GameObjectHelper> TmpList = new List<GameObjectHelper>();
		static readonly List<ComponentHelper> TmpComponentList = new List<ComponentHelper>();

		static readonly Stack<StackHelper> HelperStack = new Stack<StackHelper>();
		static readonly Stack<StackHelper> HelperCountStack = new Stack<StackHelper>();
		static readonly Stack<StackHelper> HelperDrawStack = new Stack<StackHelper>();

		static readonly Stack<GameObjectHelper> GameObjectHelperStack = new Stack<GameObjectHelper>();

		static readonly List<GameObject> SourceObjs = new List<GameObject>();
		static readonly List<GameObject> CopyObjs = new List<GameObject>();
		static readonly List<SerializedProperty> SrcProps = new List<SerializedProperty>();
		static readonly List<SerializedProperty> CopyProps = new List<SerializedProperty>();
		static readonly List<Component> SourceComps = new List<Component>();
		static readonly List<Component> CopyComps = new List<Component>();
		static readonly List<PropertyHelper> Properties = new List<PropertyHelper>();
		static readonly List<SerializedProperty> Props = new List<SerializedProperty>();
		static readonly List<SerializedProperty> OtherProps = new List<SerializedProperty>();
		static readonly List<object> Objs = new List<object>();
		static readonly List<GameObject> OtherObjs = new List<GameObject>();
		static readonly List<object> Comps = new List<object>();
		static readonly List<Component> OtherComps = new List<Component>();
		static readonly List<GameObjectHelper> SearchList = new List<GameObjectHelper>();

		public GameObjectHelper(ObjectMerge window, GameObjectHelper parent = null) :
			base(parent, window, window, null) {
			this.parent = parent;
			this.window = window;
		}

		public void SetGameObjects(GameObject mine, GameObject theirs) {
			if (objectContainer == null)
				objectContainer = ObjectContainer.Create(out mineProp, out theirsProp);

			mineProp.objectReferenceValue = mine;
			theirsProp.objectReferenceValue = theirs;

			mineProp.serializedObject.ApplyModifiedProperties();
		}

		public override IEnumerator Refresh() {
			drawCountDirty = true;
			var mine = this.mine;
			if (mine) {
				MineList.Clear();
				Util.GameObjectToList(mine, MineList);
				window.totalUpdateNum = MineList.Count;
			}

			var theirs = this.theirs;
			if (theirs) {
				TheirsList.Clear();
				Util.GameObjectToList(theirs, TheirsList);
				window.totalUpdateNum += TheirsList.Count;
			}

			window.updateCount = 0;
			var enumerator = DeepRefresh(this);
			while (enumerator.MoveNext())
				yield return null;
		}

		static IEnumerator DeepRefresh(GameObjectHelper root) {
			//Clear stacks in case of canceled refresh
			HelperStack.Clear();

			HelperStack.Push(new StackHelper(root, 0));
			while (HelperStack.Count > 0) {
				var pop = HelperStack.Pop();
				var helper = pop.helper;
				var i = pop.siblingIndex;

				int count;
				var children = helper.children;
				if (i == 0) {
					if (children != null) {
						TmpList.Clear();
						TmpList.AddRange(children);
						count = TmpList.Count;
						for (var j = count - 1; j >= 0; j--) {
							var obj = TmpList[j];
							if (obj.mine == null && obj.theirs == null)
								children.RemoveAt(j);
						}

						children.Sort(delegate (GameObjectHelper a, GameObjectHelper b) {
							var aMine = a.mine;
							var bMine = b.mine;
							if (aMine && bMine)
								return string.Compare(aMine.name, bMine.name, StringComparison.Ordinal);
							var bTheirs = b.theirs;
							if (aMine && bTheirs)
								return string.Compare(aMine.name, bTheirs.name, StringComparison.Ordinal);
							var aTheirs = a.theirs;
							if (aTheirs && bMine)
								return string.Compare(aTheirs.name, bMine.name, StringComparison.Ordinal);
							if (aTheirs && bTheirs)
								return string.Compare(aTheirs.name, bTheirs.name, StringComparison.Ordinal);
							return 0;
						});
					}

					helper.Same = true;
					MyChildren.Clear();
					TheirChildren.Clear();
					MyComponents.Clear();
					TheirComponents.Clear();
					//Get lists of components and children
					var window = helper.window;
					var mine = helper.mine;
					if (mine) {
						window.updateCount++;
						var myTransform = mine.transform;
						var myChildCount = myTransform.childCount;
						for(var j = 0; j < myChildCount; j++) { MyChildren.Add(myTransform.GetChild(j).gameObject); }
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
						mine.GetComponents(MyComponents);
#else
						MyComponents.AddRange(mine.GetComponents<Component>());
#endif
					}

					var theirs = helper.theirs;
					if (theirs) {
						window.updateCount++;
						var theirTransform = theirs.transform;
						var theirChildCount = theirTransform.childCount;
						for (var j = 0; j < theirChildCount; j++) { TheirChildren.Add(theirTransform.GetChild(j).gameObject); }
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
						theirs.GetComponents(TheirComponents);
#else
						TheirComponents.AddRange(theirs.GetComponents<Component>());
#endif
					}

					// Clear empty components
					var components = helper.components;
					var componentCount = components.Count;
					TmpComponentList.Clear();
					for (var j = 0; j < componentCount; j++)
						TmpComponentList.Add(components[j]);

					for (var j = componentCount - 1; j >= 0; j--) {
						var component = TmpComponentList[j];
						if (component.mine == null && component.theirs == null)
							components.RemoveAt(j);
					}

					//Merge Components
					helper.sameComponents = true;

					count = MyComponents.Count;
					for (var j = 0; j < count; j++) {
						var myComponent = MyComponents[j];
						// Missing scripts show up as null
						if (myComponent == null)
							continue;

						var type = myComponent.GetType();
						Component match = null;
						var matchIndex = -1;
						var theirCount = TheirComponents.Count;
						for (var k = 0; k < theirCount; k++) {
							var theirComponent = TheirComponents[k];
							if (theirComponent == null)
								continue;

							if (theirComponent.GetType() == type) {
								match = theirComponent;
								matchIndex = k;
								break;
							}
						}

						ComponentHelper ch = null;
						componentCount = components.Count;
						for (var k = 0; k < componentCount; k++) {
							var component = components[k];
							if (component.mine == myComponent || match != null && component.theirs == match) {
								ch = component;
								break;
							}
						}

						if (ch == null) {
							ch = new ComponentHelper(myComponent, match, helper, window);
							components.Add(ch);
						} else {
							ch.SetComponents(myComponent, match);
						}

						var enumerator = ch.Refresh();
						while (enumerator.MoveNext())
							yield return null;

						if (!helper.ComponentIsFiltered(ch.type) && !ch.Same) {
							helper.Same = false;
							helper.sameComponents = false;
						}

						if (matchIndex >= 0)
							TheirComponents.RemoveAt(matchIndex);
					}

					if (TheirComponents.Count > 0) {
						count = TheirComponents.Count;
						for (var j = 0; j < count; j++) {
							var theirComponent = TheirComponents[j];
							// Missing scripts show up as null
							if (theirComponent == null)
								continue;

							ComponentHelper ch = null;
							componentCount = components.Count;
							for (var k = 0; k < componentCount; k++) {
								var component = components[k];
								if (component.theirs == theirComponent) {
									ch = component;
									break;
								}
							}

							if (ch == null) {
								ch = new ComponentHelper(null, theirComponent, helper, window);
								var enumerator = ch.Refresh();
								while (enumerator.MoveNext())
									yield return null;

								components.Add(ch);
							}

							if (!helper.ComponentIsFiltered(ch.type) && !ch.Same) {
								helper.Same = false;
								helper.sameComponents = false;
							}
						}
					}

					//Merge Children
					GameObjectHelper oh;
					count = MyChildren.Count;
					for(var j = 0; j < count; j++) {
						var myChild = MyChildren[j];
						GameObject match = null;
						var matchIndex = -1;
						var theirCount = TheirChildren.Count;
						for (var k = 0; k < theirCount; k++) {
							var theirChild = TheirChildren[k];
							if (SameObject(myChild, theirChild)) {
								match = theirChild;
								matchIndex = k;
								break;
							}
						}

						oh = null;
						if (children != null) {
							var childCount = children.Count;
							for (var k = 0; k < childCount; k++) {
								var search = children[k];
								if (search.mine == myChild || match != null && search.theirs == match) {
									oh = search;
									break;
								}
							}
						}

						if (oh == null) {
							oh = new GameObjectHelper(window, helper);
							oh.SetGameObjects(myChild, match);

							if (children == null) {
								children = new List<GameObjectHelper>(1);
								helper.children = children;
							}

							children.Add(oh);
						} else {
							oh.SetGameObjects(myChild, match);
						}

						if (matchIndex >= 0)
							TheirChildren.RemoveAt(matchIndex);
					}

					if (TheirChildren.Count > 0) {
						helper.Same = false;
						count = TheirChildren.Count;
						for (var j = 0; j < count; j++) {
							var g = TheirChildren[j];
							oh = null;
							if (children != null) {
								var childCount = children.Count;
								// ReSharper disable once LoopVariableIsNeverChangedInsideLoop
								for (var k = 0; k < childCount; k++) {
									var search = children[k];
									if (search.theirs == g) {
										oh = search;
										break;
									}
								}
							}

							if (oh == null) {
								if (children == null) {
									children = new List<GameObjectHelper>(1);
									helper.children = children;
								}

								oh = new GameObjectHelper(window, helper);
								oh.SetGameObjects(null, g);
								children.Add(oh);
							}
						}
					}

					var mySO = helper.mySO;
					if (mine)
						helper.mySO = mySO = new SerializedObject(mine);

					var theirSO = helper.theirSO;
					if (theirs)
						helper.theirSO = theirSO = new SerializedObject(theirs);

					var attributes = helper.attributes;
					var e = PropertyHelper.UpdatePropertyList(attributes, mySO, theirSO, helper, null, null, window, null, helper, window, true);
					while (e.MoveNext())
						yield return null;

					helper.sameAttributes = true;
					count = attributes.Count;
					// ReSharper disable once ForCanBeConvertedToForeach
					for (var j = 0; j < count; j++) {
						var attribute = attributes[j];
						if (!attribute.Same)
							helper.sameAttributes = false;
					}

					if (!helper.sameAttributes && window.compareAttributes)
						helper.Same = false;
				}

				if (children != null) {
					if (children.Count > i) {
						HelperStack.Push(new StackHelper(helper, i + 1));
						HelperStack.Push(new StackHelper(children[i], 0));
					} else {
						count = children.Count;
						for (var j = 0; j < count; j++) {
							if (children[j].Same)
								continue;

							helper.Same = false;
							break;
						}
					}
				}

				yield return null;
			}

#if UNIMERGE_ASSERTS
			Debug.Assert(HelperStack.Count == 0);
#endif
		}

		bool ComponentIsFiltered(Type type) {
#if Unity3
			for(int i = 0; i < ObjectMerge.filterTypes.Count; i++) {
				if(type == ObjectMerge.filterTypes[i])
					return true;
			}
#else
			var componentTypes = ObjectMerge.componentTypes;
			var length = componentTypes.Length;
			for (var i = 0; i < length; i++) {
				var typeMask = window.typeMask[i];
				if (typeMask == -1) //This has everything, continue
					continue;
				var idx = ArrayUtility.IndexOf(componentTypes[i], type);
				if (idx != -1)
					return ((typeMask >> idx) & 1) == 0;
			}
#endif
			return false; //Assume not filtered
		}

		/// <summary>
		/// Draw the row for this GameObject
		/// </summary>
		/// <param name="colWidth"></param>
		/// <param name="indent"></param>
		public void Draw(float colWidth, float indent =  4) {
			DeepDraw(this, window, GUILayout.Width(colWidth), indent);
		}

		static void DeepDraw(GameObjectHelper root, ObjectMerge window, GUILayoutOption colWidth, float indent = 4) {
			HelperDrawStack.Clear();
			HelperDrawStack.Push(new StackHelper(root, 0));
			while (HelperDrawStack.Count > 0) {
				var pop = HelperDrawStack.Pop();
				var helper = pop.helper;
				var i = pop.siblingIndex;

				var children = helper.children;
				if (i == 0) {
					if (window.drawAbort)
						break;

					var draw = window.ScrollCheck();
					var showComponents = helper.showComponents;
					var components = helper.components;
					if (draw) {
						//This object
						window.StartRow(helper.Same, !helper.attributesSelected && helper == window.selected);

						//Display mine
						GUILayout.BeginVertical();
						GUILayout.Space(ObjectPadding);

						thisArg = helper;
						indentArg = indent;
						var foldoutState = helper.showChildren;
						var indentOption = GUILayout.Width(helper == root ? 0 : indent);
						helper.DrawObject(true, indentOption, colWidth);

						GUILayout.EndVertical();

						GUILayout.Space(-3); // Because the gameobject buttons are offset for some reason

						var isRoot = window.root == helper;
						//Swap buttons
						var parent = helper.parent;
						DrawMidButtons(helper.mine, helper.theirs, isRoot || parent != null && parent.mine, isRoot || parent != null && parent.theirs,
							LeftButton, RightButton, LeftDeleteButton, RightDeleteButton);
						//Display theirs
						GUILayout.BeginVertical();
						GUILayout.Space(ObjectPadding);
						GUILayout.BeginHorizontal();

						helper.DrawObject(false, indentOption, colWidth);

						var showChildren = helper.showChildren;
						if (foldoutState != showChildren) {
							helper.InvalidateDrawCount();
							if (Event.current.alt)
								helper.SetExpandedRecursively(showChildren);
						}

						if (GUILayout.Button(showComponents ? "-" : "+", Width19)) {
							helper.InvalidateDrawCount();
							showComponents = !showComponents;
							helper.showComponents = showComponents;
							if (Event.current.alt) {
								helper.showAttributes = showComponents;
								foreach (var component in components)
									component.SetExpandedRecursively(showComponents);
							}
						}
						GUILayout.EndHorizontal();
						GUILayout.EndVertical();

						window.EndRow(helper.sameAttributes && helper.sameComponents);
					}

					if (showComponents) {
						var newWidth = indent + Util.DoubleTabSize;
						var newIndent = GUILayout.Width(newWidth);
						helper.DrawAttributes(newWidth, colWidth, newIndent);

						TmpComponentList.Clear();
						foreach (var component in components)
							TmpComponentList.Add(component);

						foreach (var component in TmpComponentList)
							component.Draw(newWidth, colWidth, newIndent);
					}
				}

				if (children != null && helper.showChildren) {
					if (children.Count > i) {
						HelperDrawStack.Push(new StackHelper(helper, i + 1));
						HelperDrawStack.Push(new StackHelper(children[i], 0));

						indent += Util.TabSize;
					} else {
						indent -= Util.TabSize;
					}
				} else {
					indent -= Util.TabSize;
				}
			}
		}

		static void LeftButton() {
			//NB: This still throws a SerializedProperty error (at least in Unity 3) gonna have to do a bit more poking.
			thisArg.window.update = thisArg.Copy(true);
		}

		static void RightButton() {
			thisArg.window.update = thisArg.Copy(false);
		}

		static void LeftDeleteButton() {
			var window = thisArg.window;
			window.updateType = RefreshType.Deleting;
			window.update = thisArg.Delete(true);
		}

		static void RightDeleteButton() {
			var window = thisArg.window;
			window.updateType = RefreshType.Deleting;
			window.update = thisArg.Delete(false);
		}

		void DrawAttributes(float indent, GUILayoutOption colWidth, GUILayoutOption indentOption) {
			if (window.ScrollCheck()) {
				window.StartRow(sameAttributes, attributesSelected && this == window.selected);

				DrawAttribute(true, indentOption, colWidth);

				if (GetObject(true) && GetObject(false))
					DrawMidButtons(LeftAttributeButton, RightAttributeButton);
				else
					GUILayout.Space(UniMergeConfig.DoubleMidWidth);

				DrawAttribute(false, indentOption, colWidth);

				window.EndRow(sameAttributes);
			}

			if (showAttributes) {
				var newWidth = indent + Util.TabSize;
				var newIndent = GUILayout.Width(newWidth);
				foreach (var attribute in attributes)
					attribute.Draw(newWidth, colWidth, newIndent);
			}
		}

		void DrawAttribute(bool isMine, GUILayoutOption indent, GUILayoutOption colWidth) {
			GUILayout.BeginVertical(colWidth);
			{
#if Unity3
				GUILayout.Space(3);
#endif
				gameObjectArg = GetObject(isMine);
				var lastVal = showAttributes;
				Util.Indent(indent, DrawAttributeRow);
				if (showAttributes != lastVal)
					drawCountDirty = true;
#if Unity3
				GUILayout.Space(-4);
#endif
			}
			GUILayout.EndVertical();
		}

		static void LeftAttributeButton() {
			thisArg.window.update = thisArg.SetAttributes(true);
		}

		static void RightAttributeButton() {
			thisArg.window.update = thisArg.SetAttributes(false);
		}

		static void DrawAttributeRow() {
			if (gameObjectArg) {
				var showAttributes = thisArg.showAttributes;
#if UNITY_5_5_OR_NEWER
				showAttributes = EditorGUILayout.Foldout(showAttributes, "Attributes", true);
#else
				showAttributes = EditorGUILayout.Foldout(showAttributes, "Attributes");
#endif
				thisArg.showAttributes = showAttributes;
			} else {
				GUILayout.Label("");
				GUILayout.Space(EmptyRowSpace);
			}
		}

		IEnumerator SetAttributes(bool toMine) {
			IEnumerator enumerator;
			foreach (var attribute in attributes) {
				enumerator = attribute.Copy(toMine);
				while (enumerator.MoveNext())
					yield return null;
			}

			enumerator = thisArg.BubbleRefresh();
			while (enumerator.MoveNext())
				yield return null;

			sameAttributes = true;
		}

		IEnumerator Delete(bool isMine) {
			if (window.selected == this)
				window.SelectPreviousRow(false);

			var enumerator = DestroyAndClearRefs(GetObject(isMine), isMine);
			while (enumerator.MoveNext())
				yield return null;

			var children = parent.children;
			if (children != null)
				children.Remove(this);

			window.update = BubbleRefresh();
		}

		public int GetDrawCount() {
			if (drawCountDirty) {
				drawCountDirty = false;
				drawCount = GetCount(true, this);
			}

			return drawCount;
		}

		internal static int GetCount(bool draw, GameObjectHelper root) {
#if UNIMERGE_ASSERTS
			Debug.Assert(HelperCountStack.Count == 0);
#endif

			var count = 0;
			HelperCountStack.Push(new StackHelper(root, 0));
			while (HelperCountStack.Count > 0) {
				var pop = HelperCountStack.Pop();
				var helper = pop.helper;
				var i = pop.siblingIndex;

				var children = helper.children;
				if (i == 0) {
					count++; //Start with 1 because we're drawing this row
					if (draw && helper.showComponents) {
						count++; //For attributes row

						if (helper.showAttributes) {
							var attributes = helper.attributes;
							var attributesCount = attributes.Count;
							for (var j = 0; j < attributesCount; j++) {
								count += attributes[j].GetDrawCount();
							}
						}

						var components = helper.components;
						var componentsCount = components.Count;
						for (var j = 0; j < componentsCount; j++) {
							count += components[j].GetDrawCount();
						}
					}
				}

				if (children != null && (!draw || helper.showChildren)) {
					if (children.Count > i) {
						HelperCountStack.Push(new StackHelper(helper, i + 1));
						HelperCountStack.Push(new StackHelper(children[i], 0));
					}
				}
			}

#if UNIMERGE_ASSERTS
			Debug.Assert(HelperCountStack.Count == 0);
#endif

			return count;
		}

		public IEnumerator DestroyAndClearRefs(UnityObject obj, bool isMine) {
			SearchList.Clear();
			window.root.ToList(SearchList);

			var count = SearchList.Count;
			for (var i = 0; i < count; i++) {
				var searchComponents = SearchList[i].components;
				var componentsCount = searchComponents.Count;
				for (var j = 0; j < componentsCount; j++) {
					var properties = searchComponents[j].properties;
					var propertiesCount = properties.Count;
					for (var k = 0; k < propertiesCount; k++) {
						var property = properties[k];
						var prop = property.GetProperty(isMine);
						if (prop == null || prop.propertyType != SerializedPropertyType.ObjectReference)
							continue;

						if (prop.objectReferenceValue == obj) {
							prop.objectReferenceValue = null;
							if (window.log) {
								Debug.Log("Set reference to null in " + prop.serializedObject.targetObject
									+ "." + prop.name, prop.serializedObject.targetObject);
							}

							if (prop.serializedObject.targetObject != null)
								prop.serializedObject.ApplyModifiedProperties();
						}
					}
				}
				yield return null;
			}

			if (isMine)
				SetGameObjects(null, theirs);
			else
				SetGameObjects(mine, null);

#if UNITY_2018_3_OR_NEWER
			if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Regular) {
				if (EditorUtility.DisplayDialog(PrefabDialogTitle, PrefabDialogText, PrefabDialogConfirm, PrefabDialogCancel)) {
					AssetDatabase.OpenAsset(
						AssetDatabase.LoadMainAssetAtPath(
							PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj)));
				}
			} else {
#endif
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER)
				UnityObject.DestroyImmediate(obj);
#else
				Undo.DestroyObjectImmediate(obj);
#endif
#if UNITY_2018_3_OR_NEWER
			}
#endif
		}

		public override void Transfer(bool toMine) {
			if (attributesSelected) {
				if (mine && theirs)
					window.update = SetAttributes(toMine);
			} else {
				var source = toMine ? theirs : mine;
				window.update = source ? Copy(toMine) : Delete(toMine);
			}
		}

		/// <summary>
		/// Copy an entire object from one side to the other
		/// </summary>
		/// <param name="toMine">Whether we are copying theirs to mine (true) or mie to theirs (false)</param>
		/// <returns>Iterator, for  coroutine update</returns>
		internal IEnumerator Copy(bool toMine) {
			if (Same)
				yield break;

			//Clear out old object
			var original = toMine ? theirs : mine;
			var replace = toMine ? mine : theirs;
			if (replace)
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER)
				UnityObject.DestroyImmediate(replace);
#else
				Undo.DestroyObjectImmediate(replace);
#endif

			//Clear out old helpers
			components.Clear();
			if (children != null)
				children.Clear();

			// ReSharper disable once RedundantCast
			var copy = (GameObject) UnityObject.Instantiate(original); // Cast required for Unity < 5

			var copyTransform = copy.transform;
			if (parent != null)
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
				copyTransform.SetParent(parent.GetObject(toMine).transform);
#else
				copyTransform.parent = parent.GetObject(toMine).transform;
#endif

			// Set transform for legacy versions
			var originalTransform = original.transform;
			copyTransform.localPosition = originalTransform.localPosition;
			copyTransform.localRotation = originalTransform.localRotation;
			copyTransform.localScale = originalTransform.localScale;

#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
			copyTransform.SetSiblingIndex(originalTransform.GetSiblingIndex());
#endif

			copy.name = original.name;

			IEnumerator enumerator;
			UniMergeWindow.blockRefresh = true;
			//Set any references on their side to this object
			if (window.deepCopy) {
				// Deep copy will have issues if this helper references destroyed object
				if (toMine)
					SetGameObjects(copy, original);
				else
					SetGameObjects(original, copy);

				window.updateType = RefreshType.Updating;
				enumerator = Refresh();
				while (enumerator.MoveNext())
					yield return null;

				window.updateType = RefreshType.Copying;
				enumerator = FindAndSetRefs(window, original, copy, !toMine);
				while (enumerator.MoveNext())
					yield return null;
			}

			// Connect prefabs, if applicable. This must happen after all modifications
#if UNITY_5_4_OR_NEWER && !UNITY_2018_3_OR_NEWER
			ConnectPrefabsAfterCopy(toMine);
			copy = toMine ? mine : theirs;
#endif

			// Set SceneMerge window references
			if (this == window.root) {
				var sceneMerge = window.sceneMerge;
				if (sceneMerge) {
					if (toMine)
						sceneMerge.myContainer = copy;
					else
						sceneMerge.theirContainer = copy;
				}
			}

			UniMergeWindow.blockRefresh = false;

			Undo.RegisterCreatedObjectUndo(copy, "UniMerge");

			enumerator = BubbleRefresh();
			while (enumerator.MoveNext())
				yield return null;
		}

#if UNITY_5_4_OR_NEWER && !UNITY_2018_3_OR_NEWER
		/// <summary>
		/// Connect copied objects to prefabs if original object is connected, and sets helper reference back to new prefab reference
		/// Also recursively calls itself in all children
		/// </summary>
		/// <param name="toMine">Whether we are copying theirs to mine (true) or mie to theirs (false)</param>
		void ConnectPrefabsAfterCopy(bool toMine) {
			var original = toMine ? theirs : mine;
			var copy = toMine ? mine : theirs;
			if (Util.IsPrefabParent(original) && PrefabUtility.GetPrefabType(original) == PrefabType.PrefabInstance) {
#if UNITY_2018_2_OR_NEWER
				copy = PrefabUtility.ConnectGameObjectToPrefab(copy, (GameObject) PrefabUtility.GetCorrespondingObjectFromSource(original));
#else
				copy = PrefabUtility.ConnectGameObjectToPrefab(copy, (GameObject) PrefabUtility.GetPrefabParent(original));
#endif
				copy.name = original.name; // ConnectPrefab will rename to prefab name

				// ConnectPrefab will override transform
				var copyTransform = copy.transform;
				var originalTransform = original.transform;
				copyTransform.localPosition = originalTransform.localPosition;
				copyTransform.localRotation = originalTransform.localRotation;
				copyTransform.localScale = originalTransform.localScale;
			}

			if (toMine)
				SetGameObjects(copy, original);
			else
				SetGameObjects(original, copy);

			if (children != null)
				foreach (var child in children)
					child.ConnectPrefabsAfterCopy(toMine);
		}
#endif

		/// <summary>
		/// Find references of source in mine, and set their counterparts in Theirs to copy. This "start" function calls
		/// FindRefs which searches the whole object's hierarchy, and then calls UnsetFlagRecursive to reset the flag
		/// used to avoid searching the same object twice
		/// </summary>
		/// <param name="window">The ObjectMergeWindow doing the merge</param>
		/// <param name="source">The source object to find references within</param>
		/// <param name="copy">The copy we just made of source</param>
		/// <param name="isMine">Whether the source object is on the mine (left) side</param>
		/// <returns>Iterator, for  coroutine update</returns>
		static IEnumerator FindAndSetRefs(ObjectMerge window, GameObject source, GameObject copy, bool isMine) {
			var root = window.root;
			SourceObjs.Clear();
			CopyObjs.Clear();
			SrcProps.Clear();
			CopyProps.Clear();
			SourceComps.Clear();
			CopyComps.Clear();
			Properties.Clear();
			Props.Clear();
			OtherProps.Clear();
			Objs.Clear();
			OtherObjs.Clear();
			Comps.Clear();
			OtherComps.Clear();
			SearchList.Clear();

			Util.GameObjectToList(source, SourceObjs);
			yield return null;
			Util.GameObjectToList(copy, CopyObjs);
			yield return null;
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
			source.GetComponentsInChildren(SourceComps);
			yield return null;
			copy.GetComponentsInChildren(CopyComps);
#else
			SourceComps.AddRange(source.GetComponents<Component>());
			yield return null;
			CopyComps.AddRange(copy.GetComponents<Component>());
#endif
			yield return null;
			root.ToList(SearchList);
			var count = SearchList.Count;

			window.updateType = RefreshType.Preparing;
			window.totalUpdateNum = count;
			window.updateCount = 0;
			for (var i = 0; i < count; i++) {
				window.updateCount++;
				var searchObj = SearchList[i];
				Objs.Add(searchObj.GetObject(isMine));
				OtherObjs.Add(searchObj.GetObject(!isMine));
				var searchComponents = searchObj.components;
				var componentsCount = searchComponents.Count;
				for (var j = 0; j < componentsCount; j++) {
					var comp = searchComponents[j];
					Comps.Add(comp.GetComponent(isMine));
					OtherComps.Add(comp.GetComponent(!isMine));
					Properties.Clear();
					comp.GetFullPropertyList(Properties);
					var propertiesCount = Properties.Count;
					for (var k = 0; k < propertiesCount; k++) {
						var property = Properties[k];
						var prop = property.GetProperty(isMine);
						var otherProp = property.GetProperty(!isMine);
						if (prop != null && otherProp != null
							&& prop.propertyType == SerializedPropertyType.ObjectReference
							&& prop.objectReferenceValue != null) {
							Props.Add(prop);
							OtherProps.Add(otherProp);
						}
					}
				}

				yield return null;
			}

			window.updateType = RefreshType.Copying;
			window.totalUpdateNum = SourceObjs.Count;
			window.updateCount = 0;
			for (var i = 0; i < SourceObjs.Count; i++) {
				window.updateCount++;
				var sourceObject = SourceObjs[i];
				var copyObject = CopyObjs[i];

				// Find and set refs to the GameObject
				for (var j = 0; j < Props.Count; j++) {
					var prop = Props[j];
					var otherProp = OtherProps[j];
					if (prop.objectReferenceValue == sourceObject) {
						//Sometimes you get an error here in older versions of Unity about using a
						//SerializedProperty after the object has been deleted.  Don't know how else to
						//detect this
						otherProp.objectReferenceValue = copyObject;
						if (window.log) {
							Debug.Log("Set reference to " + copyObject + " in "
								+ prop.serializedObject.targetObject + "." + prop.name,
								prop.serializedObject.targetObject);
						}

						if (prop.serializedObject.targetObject != null)
							prop.serializedObject.ApplyModifiedProperties();
					}
				}

				yield return null;
			}

			window.updateType = RefreshType.Copying;
			window.totalUpdateNum = SourceComps.Count;
			window.updateCount = 0;
			for (var i = 0; i < SourceComps.Count; i++) {
				window.updateCount++;
				var sourceComponent = SourceComps[i];
				// Missing scripts show up as null
				if (sourceComponent == null)
					continue;

				if (sourceComponent is Transform)
					continue;

				var copyComponent = CopyComps[i];

				// Find and set refs to the Component
				for (var l = 0; l < Props.Count; l++) {
					var prop = Props[l];
					var otherProp = OtherProps[l];
					if (prop.objectReferenceValue == sourceComponent) {
						//Sometimes you get an error here in older versions of Unity about using a
						//SerializedProperty after the object has been deleted.  Don't know how else to
						//detect this
						otherProp.objectReferenceValue = copyComponent;
						if (window.log) {
							Debug.Log("Set reference to " + copyComponent + " in "
								+ prop.serializedObject.targetObject + "." + prop.name,
								prop.serializedObject.targetObject);
						}

						if (prop.serializedObject.targetObject != null)
							prop.serializedObject.ApplyModifiedProperties();
					}
				}

				yield return null;

				//Find references outside the copied hierarchy
				SrcProps.Clear();
				CopyProps.Clear();
				PropertyHelper.GetProperties(SrcProps, new SerializedObject(sourceComponent));
				PropertyHelper.GetProperties(CopyProps, new SerializedObject(copyComponent));
				for (var j = 0; j < SrcProps.Count; j++) {
					var srcProp = SrcProps[j];
					if (srcProp.name == "m_Script") //Ignore the script
						continue;

					if (srcProp.propertyType == SerializedPropertyType.ObjectReference
						&& srcProp.objectReferenceValue != null) {
						if (srcProp.objectReferenceValue == null)
							continue;

						var copyProp = CopyProps[j];
						if (srcProp.objectReferenceValue is GameObject) {
							var index = Objs.IndexOf(srcProp.objectReferenceValue);
							if (index >= 0) {
								var otherobj = OtherObjs[index];
								if (window.log) {
									Debug.Log(
										"Set reference to " + otherobj + " in "
										+ copyProp.serializedObject.targetObject + "." + copyProp.name,
										copyProp.serializedObject.targetObject);
								}
								copyProp.objectReferenceValue = otherobj;
							}
						} else {
							var index = Comps.IndexOf(srcProp.objectReferenceValue);
							if (index >= 0) {
								var otherComp = OtherComps[index];
								if (window.log) {
									Debug.Log(
										"Set reference to " + otherComp + " in "
										+ copyProp.serializedObject.targetObject + "." + copyProp.name,
										copyProp.serializedObject.targetObject);
								}
								copyProp.objectReferenceValue = otherComp;
							}
						}

						if (copyProp.serializedObject.targetObject != null)
							copyProp.serializedObject.ApplyModifiedProperties();
					}

					yield return null;
				}
			}
		}

		public void ToList(List<GameObjectHelper> list) {
#if UNIMERGE_ASSERTS
			Debug.Assert(GameObjectHelperStack.Count == 0);
#endif

			GameObjectHelperStack.Push(this);

			while (GameObjectHelperStack.Count > 0) {
				var helper = GameObjectHelperStack.Pop();
				list.Add(helper);
				var children = helper.children;
				if (children != null) {
					var count = children.Count;
					for (var i = 0; i < count; i++)
						GameObjectHelperStack.Push(children[i]);
				}
			}

#if UNIMERGE_ASSERTS
			Debug.Assert(GameObjectHelperStack.Count == 0);
#endif
		}

		/// <summary>
		/// Get the spouse (counterpart) of an object within this tree.
		/// </summary>
		/// <param name="obj">The object we're looking for</param>
		/// <param name="isMine">Whether the object came from mine (left)</param>
		/// <returns></returns>
		public GameObject GetObjectSpouse(GameObject obj, bool isMine) {
			if (obj == GetObject(isMine)) {
				var spouse = GetObject(!isMine);
				return spouse ? spouse : null;
			}

			if (children != null) {
				foreach (var child in children) {
					var spouse = child.GetObjectSpouse(obj, isMine);
					if (spouse)
						return spouse;
				}
			}

			return null;
		}

		public GameObjectHelper FindObject(GameObject obj) {
			if (obj == mine || obj == theirs)
				return this;

			if (children != null) {
				foreach (var child in children) {
					var result = child.FindObject(obj);
					if (result != null)
						return result;
				}
			}

			return null;
		}

		void DrawObject(bool isMine, GUILayoutOption indent, GUILayoutOption colWidth) {
			//Create space with width = colWidth
			GUILayout.BeginVertical(colWidth);
			gameObjectArg = GetObject(isMine);
			Util.Indent(indent, DrawObjectRow);
			GUILayout.EndVertical();
		}

		static void DrawObjectRow() {
			GUILayout.BeginHorizontal();
			if (gameObjectArg) {
				GUILayout.BeginVertical();
				{
#if Unity3
					var guiContent = gameObjectArg.name;
#else
					var guiContent = new GUIContent(gameObjectArg.name, AssetPreview.GetMiniThumbnail(gameObjectArg));
#endif
					if (gameObjectArg.transform.childCount > 0) {
						GUILayout.Space(ObjectWithChildrenPadding);
						var showChildren = thisArg.showChildren;
						GUILayout.BeginHorizontal();
						var colWidth = thisArg.window.columnWidth - indentArg - FandPButtons;
						GUILayout.BeginHorizontal(GUILayout.Width(colWidth));
#if UNITY_5_5_OR_NEWER
						showChildren = EditorGUILayout.Foldout(showChildren, string.Empty, true);
#else
						showChildren = EditorGUILayout.Foldout(showChildren, string.Empty);
#endif
						GUILayout.EndHorizontal();
						// For some reason, the texture doens't show up in a foldout
						GUILayout.Space(-colWidth + 7);
						GUILayout.Label(guiContent, Util.LabelHeight);
						GUILayout.EndHorizontal();

						thisArg.showChildren = showChildren;

					} else {
						GUILayout.Space(ObjectWithoutChildrenPadding);
						GUILayout.BeginHorizontal();
						GUILayout.Space(11);
						GUILayout.Label(guiContent, Util.LabelHeight);
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(new GUIContent("F", "Focus and select the object"), UniMergeConfig.FlatButtonStyleName)) {
					Selection.activeObject = gameObjectArg;
					if (SceneView.lastActiveSceneView)
						SceneView.lastActiveSceneView.FrameSelected();
					EditorGUIUtility.PingObject(gameObjectArg);
				}

				if (GUILayout.Button(new GUIContent("P", "Ping the object in the hierarchy"), UniMergeConfig.FlatButtonStyleName))
					EditorGUIUtility.PingObject(gameObjectArg);
			} else { GUILayout.Label(""); }

			GUILayout.EndHorizontal();
		}

		public override void SetExpandedRecursively(bool expanded) {
			showChildren = expanded;
			if (children != null)
				foreach (var obj in children)
					obj.SetExpandedRecursively(expanded);
		}

		public IEnumerator<bool> ExpandDifferences() {
			drawCountDirty = true;
			window.updateCount++;

			if (mine && theirs) {
				if (!sameAttributes)
					showComponents = showAttributes = true;

				if (!sameComponents) {
					showComponents = true;
					foreach (var component in components)
						component.ExpandDifferences();
				}
			}

			if (children != null) {
				foreach (var child in children) {
					var enumerator = child.ExpandDifferences();
					while (enumerator.MoveNext())
						yield return !Same;

					if (enumerator.Current)
						showChildren = true;
				}
			}

			yield return !Same;
		}

		//Big ??? here.  What do we count as the same needing merge and what do we count as totally different?
		static bool SameObject(UnityObject mine, UnityObject theirs) { return mine.name == theirs.name; }

		public GameObject GetObject(bool isMine) { return isMine ? mine : theirs; }

		public override string ToString() { return string.Format("{0}, {1}", mine, theirs); }
	}
}
