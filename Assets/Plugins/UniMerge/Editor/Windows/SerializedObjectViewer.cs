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
//SerializedObjectViewer Window

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#define Unity4_0To4_2
#endif

using System.Collections.Generic;
using UniMerge.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace UniMerge.Editor.Windows {
	public class SerializedObjectViewer : EditorWindow {
		const int MaxCount = 5000;

		static readonly List<SerializedObjectViewer> Windows = new List<SerializedObjectViewer>();

		Object viewObject;
		Object obj;
		SerializedObject so;

		bool fullPath, showDepth, showType, showStringArrays;
		Vector2 scroll;

		// ReSharper disable once UnusedMember.Local
		[MenuItem("Window/UniMerge/Serialized Object Viewer %#o")]
		static void Init() {
			NewWindow().obj = Selection.activeObject;
		}

		static SerializedObjectViewer NewWindow() {
			foreach (var win in new List<SerializedObjectViewer>(Windows)) {
				if (!win) {
					Windows.Remove(win);
					continue;
				}

				if (!win.obj) {
					win.Show();
					return win;
				}
			}

			var window = CreateInstance<SerializedObjectViewer>();
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			window.titleContent = new GUIContent("Serialized Object Viewer");
#endif
			window.Show();
			Windows.Add(window);
			return window;
		}

		// ReSharper disable once UnusedMember.Local
		void OnGUI() {
			//Ctrl + w to close
			if (Event.current.Equals(Event.KeyboardEvent("^w"))) {
				Event.current.Use();
				Close();
				GUIUtility.ExitGUI();
			}

			if (obj == null) {
				obj = viewObject;
				viewObject = null;
			}

			obj = EditorGUILayoutExt.ObjectField("Object", obj, typeof(Object), true);

			GUI.enabled = obj != null;

			fullPath = EditorGUILayout.Toggle("Show Full Paths", fullPath);
			showDepth = EditorGUILayout.Toggle("Show Depths", showDepth);
			showType = EditorGUILayout.Toggle("Show Types", showType);
			showStringArrays = EditorGUILayout.Toggle("Show String Arrays", showStringArrays);

#if !(Unity3 || Unity4_0To4_2)
			EditorGUIUtility.labelWidth = fullPath ? 250 : 150;
#endif

			if (GUILayout.Button("Refresh"))
				so = new SerializedObject(obj);

			if (GUILayout.Button("Print")) {
				var it = so.GetIterator();
				var next = it.Next(true);
				while (next) {
					if (it.name == "m_Script") {
						Debug.Log("Script array hidden for performance reasons");
						var depth = it.depth;
						Debug.Log(it.name);
						while ((next = it.Next(true)) && it.depth > depth || it.name == "Array") { }
					}

					if (!next)
						break;

					Debug.Log(it.depth + " - " + it.propertyPath + " (" + it.propertyType + ")");

					next = it.Next(true);
				}
			}

			if (obj != null) {
				scroll = GUILayout.BeginScrollView(scroll);
				if (so == null || so.targetObject != obj)
					so = new SerializedObject(obj);

				var count = 0;
				var iterator = so.GetIterator();
				var hasNext = iterator.Next(true);
				while (hasNext) {
					if (iterator.name == "m_Script") {
						GUI.enabled = false;
						GUILayout.Label("Script array hidden for performance reasons");
						GUI.enabled = true;
						var depth = iterator.depth;
						while ((hasNext = iterator.Next(true)) && iterator.depth > depth || iterator.name == "Array") { }
					}

					if (!hasNext)
						break;

					GUILayout.BeginHorizontal();
					GUILayout.Space(Util.TabSize * iterator.depth);

					if (showDepth)
						GUILayout.Label(iterator.depth + "", GUILayout.Width(10));

					if (showType)
						GUILayout.Label(iterator.propertyType + "", GUILayout.Width(50));
					try {
#if !UNITY_3_4 && !UNITY_3_3 && !UNITY_3_2 && !UNITY_3_1 && !UNITY_3_0_0 && !UNITY_3_0
						if (fullPath)
							EditorGUILayout.PropertyField(iterator, new GUIContent(iterator.propertyPath), false);
						else
							EditorGUILayout.PropertyField(iterator, false);
#else
						if (fullPath)
							EditorGUILayout.PropertyField(iterator, new GUIContent(iterator.propertyPath));
						else
							EditorGUILayout.PropertyField(iterator);
#endif
					} catch {
						/* ignored */
					}
					GUILayout.EndHorizontal();

					hasNext = iterator.Next(iterator.isExpanded);

					if (count++ > MaxCount)
						break;
				}
				GUILayout.EndScrollView();

				so.ApplyModifiedProperties();
			}
		}

		// ReSharper disable once UnusedMember.Global
		public static void OpenWindowAndViewObject(Object obj) { NewWindow().ViewObject(obj); }

		// ReSharper disable once MemberCanBePrivate.Global
		public void ViewObject(Object obj) { viewObject = obj; }
	}
}
