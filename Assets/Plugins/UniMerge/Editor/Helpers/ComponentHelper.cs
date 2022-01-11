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
//ComponentHelper class

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

namespace UniMerge.Editor.Helpers {
	public class ComponentHelper : Helper {
		ComponentContainer componentContainer;
		SerializedProperty mineProp, theirsProp;

		public Component mine { get { return componentContainer.mine; } }
		public Component theirs { get { return componentContainer.theirs; } }
		public override bool invalid { get { return mine == null && theirs == null; } }

		public override bool hasChildren { get { return true; } }
		public override Helper this[int i] { get { return properties[i]; } }
		public override int count { get { return properties.Count; } }

		public readonly Type type;
		public readonly List<PropertyHelper> properties = new List<PropertyHelper>(4);

		protected override bool hasMine { get { return mine; } }
		protected override bool hasTheirs { get { return theirs; } }

		SerializedObject mySO, theirSO;
		new readonly GameObjectHelper parent; // ComponentHelpers can only be children of GameObjectHelpers
		new readonly ObjectMerge window; // ComponentHelpers can only be in ObjectMerge windows

		//Used as an arguments to delegated static methods
		static Component componentArg;
		static ComponentHelper thisArg;
		static float indentArg;

		static readonly List<SerializedProperty> SrcProps = new List<SerializedProperty>();
		static readonly List<SerializedProperty> CopyProps = new List<SerializedProperty>();
		static readonly List<PropertyHelper> Properties = new List<PropertyHelper>();
		static readonly List<SerializedProperty> Props = new List<SerializedProperty>();
		static readonly List<SerializedProperty> OtherProps = new List<SerializedProperty>();
		static readonly List<object> Objs = new List<object>();
		static readonly List<GameObject> OtherObjs = new List<GameObject>();
		static readonly List<object> Comps = new List<object>();
		static readonly List<Component> OtherComps = new List<Component>();
		static readonly List<GameObjectHelper> SearchList = new List<GameObjectHelper>();

		public ComponentHelper(Component mine, Component theirs, GameObjectHelper parent = null,
			ObjectMerge window = null) : base(parent, window, window, null) {
			this.parent = parent;
			this.window = window;
			SetComponents(mine, theirs);

			type = mine ? mine.GetType() : theirs.GetType();
		}

		public void SetComponents(Component mine, Component theirs) {
			if (componentContainer == null)
				componentContainer = ComponentContainer.Create(out mineProp, out theirsProp);

			mineProp.objectReferenceValue = mine;
			theirsProp.objectReferenceValue = theirs;

			theirsProp.serializedObject.ApplyModifiedProperties();
		}

		public Component GetComponent(bool isMine) { return isMine ? mine : theirs; }

		public override IEnumerator Refresh() {
			var mine = this.mine;
			if (mine)
				mySO = new SerializedObject(mine);

			var theirs = this.theirs;
			if (theirs)
				theirSO = new SerializedObject(theirs);

			var enumerator = PropertyHelper.UpdatePropertyList(properties, mySO, theirSO, null, this, null, objectMerge, sceneMerge, this, window);
			while (enumerator.MoveNext())
				yield return null;

			Same = enumerator.Current;

			if (!mine || !theirs)
				Same = false;
		}

		public void Draw(float indent, GUILayoutOption colWidth, GUILayoutOption indentOption) {
			if (window.drawAbort)
				return;

			if (window.ScrollCheck()) {
				window.StartRow(Same, this == window.selected);
				//Store foldout state before doing GUI to check if it changed
				var foldoutState = showChildren;
				indentArg = indent;
				thisArg = this;
				DrawComponent(true, indentOption, colWidth);
				//Swap buttons
				var parentMine = parent.mine;
				var parentTheirs = parent.theirs;
				if (parentMine && parentTheirs)
					DrawMidButtons(mine, theirs, parentMine, parentTheirs, LeftButton, RightButton, LeftDeleteButton, RightDeleteButton);
				else
					GUILayout.Space(UniMergeConfig.DoubleMidWidth);
				//Display theirs
				DrawComponent(false, indentOption, colWidth);

				if (showChildren != foldoutState) {
					InvalidateDrawCount();
					//If foldout state changed and user was holding alt, set all child foldout states to this state
					if (Event.current.alt) {
						foreach (var property in properties)
							property.SetExpandedRecursively(showChildren);
					}
				}
				window.EndRow(Same);
			}

			if (showChildren) {
				var tmp = new List<PropertyHelper>(properties);

				var newWidth = indent + Util.TabSize;
				var newIndent = GUILayout.Width(newWidth);
				foreach (var property in tmp)
					property.Draw(newWidth, colWidth, newIndent);
			}

			if (mySO != null && mySO.targetObject != null)
				if (mySO.ApplyModifiedProperties())
					window.update = BubbleRefresh();

			if (theirSO != null && theirSO.targetObject != null)
				if (theirSO.ApplyModifiedProperties())
					window.update = BubbleRefresh();
		}

		IEnumerator Delete(bool isMine) {
			if (window.selected == this)
				window.SelectPreviousRow(false);

			var component = GetComponent(isMine);
			if (component is Camera)
			{
				var enumerator = parent.DestroyAndClearRefs(component.GetComponent<AudioListener>(), true);
				while (enumerator.MoveNext())
					yield return null;

#if !UNITY_2019_3_OR_NEWER
#pragma warning disable 618
				enumerator = parent.DestroyAndClearRefs(component.GetComponent<GUILayer>(), true);
				while (enumerator.MoveNext())
					yield return null;
#pragma warning restore 618
#endif

				enumerator = parent.DestroyAndClearRefs(component.GetComponent("FlareLayer"), true);
				while (enumerator.MoveNext())
					yield return null;
			}
			var e = parent.DestroyAndClearRefs(component, true);
			while (e.MoveNext())
				yield return null;

			window.update = BubbleRefresh();
		}

		void DrawComponent(bool isMine, GUILayoutOption indent, GUILayoutOption colWidth) {
			GUILayout.BeginVertical(colWidth);

#if Unity3
			GUILayout.Space(3);
#endif

			componentArg = GetComponent(isMine);
			thisArg = this;
			Util.Indent(indent, DrawComponentRow);

#if Unity3
			GUILayout.Space(-4);
#endif

			GUILayout.EndVertical();
		}

		static void LeftButton() { thisArg.window.update = thisArg.Copy(true); }

		static void RightButton() { thisArg.window.update = thisArg.Copy(false); }

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

		static void DrawComponentRow() {
			if (componentArg) {
				var showChildren = thisArg.showChildren;
				var lastState = showChildren;
				GUILayout.BeginHorizontal();
				var colWidth = thisArg.window.columnWidth - indentArg;
				GUILayout.BeginHorizontal(GUILayout.Width(colWidth));
#if UNITY_5_5_OR_NEWER
				showChildren = EditorGUILayout.Foldout(showChildren, string.Empty, true);
#else
				showChildren = EditorGUILayout.Foldout(showChildren, string.Empty);
#endif
				GUILayout.EndHorizontal();
				// For some reason, the texture doens't show up in a foldout
				GUILayout.Space(-colWidth + 8);
#if Unity3
				var guiContent = thisArg.type.Name;
#else
				var guiContent = new GUIContent(thisArg.type.Name, AssetPreview.GetMiniThumbnail(componentArg));
#endif
				GUILayout.Label(guiContent, Util.LabelHeight);
				GUILayout.EndHorizontal();
				if (lastState != showChildren)
					thisArg.InvalidateDrawCount();

				thisArg.showChildren = showChildren;
			} else {
				GUILayout.Label("");
				GUILayout.Space(EmptyRowSpace);
			}
		}

		public void GetFullPropertyList(List<PropertyHelper> list) {
			for (var i = 0; i < properties.Count; i++)
				properties[i].ToList(list);
		}

		public int GetDrawCount() {
			var count = 1;
			if (showChildren)
				foreach (var property in properties) { count += property.GetDrawCount(); }

			return count;
		}

		public override void Transfer(bool toMine) {
			var source = toMine ? theirs : mine;
			if (!parent.mine || !parent.theirs)
				return;

			window.update = source ? Copy(toMine) : Delete(toMine);
		}

		/// <summary>
		/// Copy an entire object from one side to the other
		/// </summary>
		/// <param name="toMine">Whether we are copying theirs to mine (true) or mie to theirs (false)</param>
		/// <returns>Iterator, for  coroutine update</returns>
		public IEnumerator Copy(bool toMine) {
			if (Same)
				yield break;

			var original = toMine ? theirs : mine;
			var copy = toMine ? mine : theirs;
			//Create new component if needed
			if (!copy) {
#if !(UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER)
				copy = (toMine ? parent.mine : parent.theirs).AddComponent(original.GetType());
#else
				copy = Undo.AddComponent(toMine ? parent.mine : parent.theirs, original.GetType());
#endif
				if (toMine)
					SetComponents(copy, original);
				else
					SetComponents(original, copy);
			}

			EditorUtility.CopySerialized(original, copy);

			IEnumerator enumerator;
			UniMergeWindow.blockRefresh = true;
			//Set any references on their side to this object
			if (window.deepCopy) {
				window.updateType = RefreshType.Updating;
				enumerator = Refresh();
				while (enumerator.MoveNext())
					yield return null;

				window.updateType = RefreshType.Copying;
				enumerator = FindAndSetRefs(window, original, copy, !toMine);
				while (enumerator.MoveNext())
					yield return null;
			}

			enumerator = BubbleRefresh();
			while (enumerator.MoveNext())
				yield return null;
		}

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
		static IEnumerator FindAndSetRefs(ObjectMerge window, UnityObject source, UnityObject copy, bool isMine) {
			var root = window.root;
			SrcProps.Clear();
			CopyProps.Clear();
			Properties.Clear();
			Props.Clear();
			OtherProps.Clear();
			Objs.Clear();
			OtherObjs.Clear();
			Comps.Clear();
			OtherComps.Clear();
			SearchList.Clear();

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
						if (prop != null && otherProp != null && prop.propertyType == SerializedPropertyType.ObjectReference
							&& prop.objectReferenceValue != null) {
							Props.Add(prop);
							OtherProps.Add(otherProp);
						}
					}
				}

				yield return null;
			}

			window.updateType = RefreshType.Copying;
			window.totalUpdateNum = Props.Count;
			window.updateCount = 0;
			// Find and set refs to the Component
			for (var j = 0; j < Props.Count; j++) {
				var prop = Props[j];
				var otherProp = OtherProps[j];
				if (prop.objectReferenceValue == source) {
					//Sometimes you get an error here in older versions of Unity about using a
					//SerializedProperty after the object has been deleted.  Don't know how else to
					//detect this
					otherProp.objectReferenceValue = copy;
					if (window.log) {
						Debug.Log("Set reference to " + copy + " in " + prop.serializedObject.targetObject + "." + prop.name,
							prop.serializedObject.targetObject);
					}

					if (prop.serializedObject.targetObject != null)
						prop.serializedObject.ApplyModifiedProperties();
				}

				yield return null;
			}

			//Find references in properties
			SrcProps.Clear();
			CopyProps.Clear();
			PropertyHelper.GetProperties(SrcProps, new SerializedObject(source));
			PropertyHelper.GetProperties(CopyProps, new SerializedObject(copy));

			window.updateType = RefreshType.Copying;
			window.totalUpdateNum = SrcProps.Count;
			window.updateCount = 0;
			for (var j = 0; j < SrcProps.Count; j++) {
				window.updateCount++;
				var srcProp = SrcProps[j];
				if (srcProp.name == "m_Script" || srcProp.name == "m_Father") //Ignore the script and transform parent
					continue;

				if (srcProp.propertyType == SerializedPropertyType.ObjectReference && srcProp.objectReferenceValue != null) {
					if (srcProp.objectReferenceValue == null)
						continue;

					var copyProp = CopyProps[j];
					if (srcProp.objectReferenceValue is GameObject) {
						var index = Objs.IndexOf(srcProp.objectReferenceValue);
						if (index >= 0) {
							var otherobj = OtherObjs[index];
							if (window.log) {
								Debug.Log(
									"Set reference to " + otherobj + " in " + copyProp.serializedObject.targetObject + "." + copyProp.name,
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
									"Set reference to " + otherComp + " in " + copyProp.serializedObject.targetObject + "." + copyProp.name,
									copyProp.serializedObject.targetObject);
							}

							copyProp.objectReferenceValue = otherComp;
						}
					}

					if (copyProp.serializedObject.targetObject != null)
						copyProp.serializedObject.ApplyModifiedProperties();

					yield return null;
				}
			}
		}

		public void ExpandDifferences() {
			if (!Same)
				showChildren = true;

			foreach (var property in properties)
				property.ExpandDifferences();
		}

		public override void SetExpandedRecursively(bool expanded) {
			showChildren = expanded;
			foreach (var property in properties)
				property.SetExpandedRecursively(expanded);
		}

		public override string ToString() { return string.Format("{0}, {1}", mine, theirs); }
	}
}
