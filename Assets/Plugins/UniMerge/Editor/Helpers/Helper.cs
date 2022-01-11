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
//Helper class

using System;
using System.Collections;
using UniMerge.Editor.Windows;
using UnityEngine;

namespace UniMerge.Editor.Helpers {
	public abstract class Helper {
		protected const float EmptyRowSpace = 8;

		public readonly Helper parent;
		public bool showChildren { get; set; }
		public abstract bool hasChildren { get; }
		public abstract Helper this[int i] { get; }
		public abstract int count { get; }
		protected readonly UniMergeWindow window;

		protected readonly ObjectMerge objectMerge;
		protected readonly SceneMerge sceneMerge;

		protected int drawCount;
		protected bool drawCountDirty = true;

		public bool Same { get; protected set; }
		public abstract bool invalid { get; }

		static readonly GUIContent LeftButton = new GUIContent("<", "Copy theirs (properties and children) to mine");
		static readonly GUIContent LeftDeleteButton = new GUIContent("X", "Delete mine");
		static readonly GUIContent RightButton = new GUIContent(">", "Copy mine (properties and children) to theirs");
		static readonly GUIContent RightDeleteButton = new GUIContent("X", "Delete theirs");

		static readonly GUILayoutOption Width = GUILayout.Width(UniMergeConfig.MidWidth);
		static readonly GUILayoutOption DoubleWidth = GUILayout.Width(UniMergeConfig.DoubleMidWidth);
		protected abstract bool hasMine { get; }
		protected abstract bool hasTheirs { get; }
		public bool hasBoth { get { return hasMine && hasTheirs; } }

		protected Helper(Helper parent, UniMergeWindow window, ObjectMerge objectMerge, SceneMerge sceneMerge) {
			this.parent = parent;
			this.window = window;
			this.objectMerge = objectMerge;
			this.sceneMerge = sceneMerge;
		}

		public IEnumerator BubbleRefresh() {
			window.drawAbort = true;
			if (parent != null) {
				var enumerator = parent.BubbleRefresh();
				while (enumerator.MoveNext()) { yield return null; }
			} else {
				window.updateType = RefreshType.Updating;
				var enumerator = Refresh();
				while (enumerator.MoveNext()) { yield return null; }
			}
		}

		public abstract IEnumerator Refresh();

		public void InvalidateDrawCount() {
			drawCountDirty = true;
			if (parent != null)
				parent.InvalidateDrawCount();
		}

		protected static void DrawMidButtons(Action toMine, Action toTheirs) {
			DrawMidButtons(true, true, toMine, toTheirs, null, null);
		}

		protected static void DrawMidButtons(bool hasMine, bool hasTheirs, Action toMine, Action toTheirs,
			Action delMine, Action delTheirs) {
			DrawMidButtons(hasMine, hasTheirs, hasMine, hasTheirs, toMine, toTheirs, delMine, delTheirs);
		}

		protected static void DrawMidButtons(bool hasMine, bool hasTheirs, bool hasMyParent, bool hasTheirParent,
			Action toMine, Action toTheirs, Action delMine, Action delTheirs) {
			const float spaceWidth = UniMergeConfig.MidWidth + 4;
			GUILayout.BeginVertical(DoubleWidth);
#if Unity3
			GUILayout.Space(2);
#endif
			GUILayout.BeginHorizontal();
			if (hasTheirs) {
				if (hasMyParent) {
					if (GUILayout.Button(LeftButton, Width))
						toMine.Invoke();
				} else {
					GUILayout.Space(spaceWidth);
				}
			} else {
				if (GUILayout.Button(LeftDeleteButton, Width))
					delMine.Invoke();
			}
			if (hasMine) {
				if (hasTheirParent) {
					if (GUILayout.Button(RightButton, Width))
						toTheirs.Invoke();
				} else {
					GUILayout.Space(spaceWidth);
				}
			} else {
				if (GUILayout.Button(RightDeleteButton, Width))
					delTheirs.Invoke();
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		public abstract void SetExpandedRecursively(bool expanded);
		public abstract void Transfer(bool toMine);
	}
}
