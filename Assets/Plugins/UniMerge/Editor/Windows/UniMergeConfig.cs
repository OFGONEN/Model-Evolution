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
//UniMergeConfig Window

using UniMerge.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace UniMerge.Editor.Windows {
	public class UniMergeConfig : EditorWindow {
		public const float Margin = 40; //Amount of extra space
		// GUI Settings
		public const string DefaultGuiSkinFilename = "Skin/UniMerge.guiskin";
		public const string DarkGuiSkinFilename = "Skin/UniMergeDark.guiskin";
		public static string DefaultPath = "Assets/UniMerge";

		// list normal is a list style with normal font size
		public const string ListStyleName = "List";
		public const string ListAltStyleName = "ListAlt";
		public const string ListStyleConflictName = ListStyleName + "Conflict";
		public const string ListAltStyleConflictName = ListAltStyleName + "Conflict";
		public const string FlatButtonStyleName = "FlatButton";
		public const string ConflictSuffix = "Conflict";
		public const float MidWidth = 25;
		public const float DoubleMidWidth = MidWidth * 2;

		static int frameTime = UniMergeWindow.MaxFrameTime;

		Vector2 scroll;

		// ReSharper disable once UnusedMember.Local
		[MenuItem("Window/UniMerge/UniMerge Config")]
		static void Init() {
			GetWindow(typeof(UniMergeConfig));
		}

		// ReSharper disable once UnusedMember.Local
		void OnGUI() {
			//Ctrl + w to close
			if (Event.current.Equals(Event.KeyboardEvent("^w"))) {
				Close();
				Event.current.Use();
				GUIUtility.ExitGUI();
			}

			const string label = "Frame Time (ms)";
			const string tooltip = "Time between yields when processing big scenes. Bigger is faster, but will slow down the UI";
			frameTime = EditorGUILayout.IntSlider(new GUIContent(label, tooltip), frameTime, 1, 1000);
			UniMergeWindow.frameTimeTicks = frameTime * 10000;
			GUILayout.Label("To change these values, set them in UniMergeConfig.cs");
			EditorGUILayout.LabelField("Default Skin", DefaultGuiSkinFilename);
			EditorGUILayout.LabelField("Dark Skin", DarkGuiSkinFilename);
			EditorGUILayout.LabelField("Plugin Path", DefaultPath);
			EditorGUILayout.LabelField("List Style Name", ListStyleName);
			EditorGUILayout.LabelField("List Alt Style Name", ListAltStyleName);
			EditorGUILayout.LabelField("Conflict Suffix", ConflictSuffix);

			GUILayout.Label(new GUIContent(
				"Note: In order to save your changes you must make\nsome change to the skin in the editor.",
				"This is because modifying the skin via script doesn't set it \"dirty\""));

			scroll = EditorGUILayout.BeginScrollView(scroll);
			var defaultSkin =
				AssetDatabase.LoadAssetAtPath(DefaultPath + "/" + DefaultGuiSkinFilename, typeof(GUISkin)) as GUISkin;
			if (defaultSkin) {
				if (defaultSkin.customStyles.Length < 5) {
					var newStyles = new GUIStyle[5];
					defaultSkin.customStyles.CopyTo(newStyles, 0);
					defaultSkin.customStyles = newStyles;
				}
				for (var i = 0; i < defaultSkin.customStyles.Length; i++)
					if (defaultSkin.customStyles[i] == null)
						defaultSkin.customStyles[i] = new GUIStyle();
				defaultSkin.customStyles[0].name = ListStyleName;
				defaultSkin.customStyles[1].name = ListAltStyleName;
				defaultSkin.customStyles[2].name = ListStyleName + ConflictSuffix;
				defaultSkin.customStyles[3].name = ListAltStyleName + ConflictSuffix;
				defaultSkin.customStyles[4].name = FlatButtonStyleName;
				GUILayout.Label("Default Skin background colors");
				defaultSkin.customStyles[0].normal.background = (Texture2D) EditorGUILayoutExt.ObjectField(ListStyleName,
					defaultSkin.customStyles[0].normal.background, typeof(Texture2D), false);
				defaultSkin.customStyles[1].normal.background = (Texture2D) EditorGUILayoutExt.ObjectField(ListAltStyleName,
					defaultSkin.customStyles[1].normal.background, typeof(Texture2D), false);
				defaultSkin.customStyles[2].normal.background =
					(Texture2D) EditorGUILayoutExt.ObjectField(ListStyleName + ConflictSuffix,
						defaultSkin.customStyles[2].normal.background, typeof(Texture2D), false);
				defaultSkin.customStyles[3].normal.background =
					(Texture2D) EditorGUILayoutExt.ObjectField(ListAltStyleName + ConflictSuffix,
						defaultSkin.customStyles[3].normal.background, typeof(Texture2D), false);
				defaultSkin.customStyles[4].normal.background =
					(Texture2D) EditorGUILayoutExt.ObjectField(FlatButtonStyleName,
						defaultSkin.customStyles[4].normal.background, typeof(Texture2D), false);
			} else { GUILayout.Label("<color=red>Oops! No Light Skin found!</color>"); }

			var darkSkin = AssetDatabase.LoadAssetAtPath(DefaultPath + "/" + DarkGuiSkinFilename, typeof(GUISkin)) as GUISkin;
			if (darkSkin) {
				if (darkSkin.customStyles.Length < 5) {
					var newStyles = new GUIStyle[5];
					darkSkin.customStyles.CopyTo(newStyles, 0);
					darkSkin.customStyles = newStyles;
				}
				for (var i = 0; i < darkSkin.customStyles.Length; i++)
					if (darkSkin.customStyles[i] == null)
						darkSkin.customStyles[i] = new GUIStyle();
				darkSkin.customStyles[0].name = ListStyleName;
				darkSkin.customStyles[1].name = ListAltStyleName;
				darkSkin.customStyles[2].name = ListStyleName + ConflictSuffix;
				darkSkin.customStyles[3].name = ListAltStyleName + ConflictSuffix;
				darkSkin.customStyles[4].name = FlatButtonStyleName;
				GUILayout.Label("Dark Skin background colors");
				darkSkin.customStyles[0].normal.background = (Texture2D) EditorGUILayoutExt.ObjectField(ListStyleName,
					darkSkin.customStyles[0].normal.background, typeof(Texture2D), false);
				darkSkin.customStyles[1].normal.background = (Texture2D) EditorGUILayoutExt.ObjectField(ListAltStyleName,
					darkSkin.customStyles[1].normal.background, typeof(Texture2D), false);
				darkSkin.customStyles[2].normal.background =
					(Texture2D) EditorGUILayoutExt.ObjectField(ListStyleName + ConflictSuffix,
						darkSkin.customStyles[2].normal.background, typeof(Texture2D), false);
				darkSkin.customStyles[3].normal.background =
					(Texture2D) EditorGUILayoutExt.ObjectField(ListAltStyleName + ConflictSuffix,
						darkSkin.customStyles[3].normal.background, typeof(Texture2D), false);
				darkSkin.customStyles[4].normal.background =
					(Texture2D) EditorGUILayoutExt.ObjectField(FlatButtonStyleName,
						darkSkin.customStyles[4].normal.background, typeof(Texture2D), false);
			} else { GUILayout.Label("<color=red>Oops! No Dark Skin found!</color>"); }
			EditorGUILayout.EndScrollView();
		}
	}
}
