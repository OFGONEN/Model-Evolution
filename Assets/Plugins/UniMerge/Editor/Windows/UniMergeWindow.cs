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
//UniMergeWindow class

#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
#define Unity3
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
#define Unity4_0To4_2
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UniMerge.Editor.Helpers;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UniMerge.Editor.Windows {
	public enum RefreshType { Updating, Comparing, Deleting, Copying, Expanding, Preparing }

	public abstract class UniMergeWindow : EditorWindow {
		internal const int MaxFrameTime = 50;
		const int RowHeight = 28;

		const int ScrollbarWidth = 15;
		const int MaxDrawWindow = 400;
		const int ProgressBarHeight = 15;

#if Unity3
		const int BasePaddingTop = 0;
		const int BasePaddingBot = 0;
#else
		const int BasePaddingTop = 3;
		const int BasePaddingBot = -7;
#endif

		public static int frameTimeTicks = MaxFrameTime * 10000; //In "ticks" which are 100ns

		protected const string RowHeightKey = "RowHeight";
		static readonly int[] RowPaddings = { 10, 5, 0 };
		static readonly float[] RowHeights = { 56.9f, 46.9f, 37 };

		static readonly HashSet<UniMergeWindow> Windows = new HashSet<UniMergeWindow>();

		public static bool blockRefresh;
		static bool displayWarning;
		static bool skinSetUp;

		public RefreshType updateType { private get; set; }
		public int updateCount, totalUpdateNum;

		public bool drawAbort;

		protected int selectedRowHeight;
		protected float colWidth;
		float scrollbarWidth;

		IEnumerator _update;

		internal IEnumerator update {
			set {
				ListenForModifications(false);
				_update = value;
			}
		}

		protected abstract int headerSize { get; }

		float lastWinHeight;
		bool cancelRefresh;
		int rowPadding = 10;
		float rowHeight;

		/// <summary>
		/// The index of the first row to be drawn
		/// (this is a float so we can use it as a scrollbar value)
		/// </summary>
		protected float objectDrawOffset;
		/// <summary>
		/// The total number of rows that can be drawn
		/// This will vary as rows are expanded/collapsed
		/// </summary>
		int objectDrawCount;
		/// <summary>
		/// What row we are currently drawing
		/// Used for ScrollCheck
		/// </summary>
		int objectDrawCursor;
		/// <summary>
		/// Stored value for when scrollbar is introduced
		/// </summary>
		int objectDrawOffsetHold;
		/// <summary>
		/// Number of rows that are currently visible
		/// </summary>
		int objectDrawWindow;

		bool stateModified;
#if !(Unity3 || Unity4_0To4_2)
		bool updateJustFinished;
#endif

		public Helper selected;

		//timing variables
		readonly Stopwatch frameTimer = new Stopwatch();

		public float columnWidth { get { return colWidth; } }

		protected virtual void OnEnable() {
			if (Windows.Count == 0) {
#if UNITY_5 || UNITY_5_3_OR_NEWER
				Application.logMessageReceived += HandleLog;
#else
				Application.RegisterLogCallback(HandleLog);
#endif
			}

#if !(Unity3 || Unity4_0To4_2)
			if (Windows.Add(this)) {
				Undo.undoRedoPerformed += OnUndoPerformed;
				ListenForModifications(true);
			}
#else
			Windows.Add(this);
#endif

			skinSetUp = false;
			objectDrawOffset = 0;

#if !Unity3
			var scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
			if (!string.IsNullOrEmpty(scriptPath))
				UniMergeConfig.DefaultPath = scriptPath.Substring(0, scriptPath.IndexOf("Editor", StringComparison.Ordinal) - 1);
#endif
		}

		protected virtual void OnDisable() {
#if !(Unity3 || Unity4_0To4_2)
			if (Windows.Remove(this)) {
				Undo.undoRedoPerformed -= OnUndoPerformed;
				ListenForModifications(false);
			}
#else
			Windows.Remove(this);
#endif

			if (Windows.Count == 0) {
#if UNITY_5 || UNITY_5_3_OR_NEWER
				Application.logMessageReceived -= HandleLog;
#else
				Application.RegisterLogCallback(null);
#endif
			}
		}

		void OnUndoPerformed() {
			stateModified = true;
		}

		void ListenForModifications(bool listen) {
#if !(Unity3 || Unity4_0To4_2)
			if (listen) {
				Undo.postprocessModifications += OnPostprocessModifications;
			} else {
				Undo.postprocessModifications -= OnPostprocessModifications;
			}
#endif
		}

#if !(Unity3 || Unity4_0To4_2)
		UndoPropertyModification[] OnPostprocessModifications(UndoPropertyModification[] modifications) {
			if (_update != null)
				return modifications;

			if (updateJustFinished) {
				updateJustFinished = false;
				return modifications;
			}

			stateModified = true;

			return modifications;
		}
#endif

		protected abstract IEnumerator Refresh();

		// ReSharper disable once UnusedMember.Local
		void OnDestroy() { EditorPrefs.SetInt(RowHeightKey, selectedRowHeight); }

		static void SetUpSkin(GUISkin builtinSkin) {
			skinSetUp = true;
			//Set up skin. We add the styles from the custom skin because there are a bunch (467!) of built in custom styles
			var guiSkinToUse = UniMergeConfig.DefaultGuiSkinFilename;
#if UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 //Alternate detection of dark skin
			if(UnityEditorInternal.InternalEditorUtility.HasPro() && EditorPrefs.GetInt("UserSkin") == 1)
				guiSkinToUse = UniMergeConfig.DarkGuiSkinFilename;
#else
			if (EditorGUIUtility.isProSkin)
				guiSkinToUse = UniMergeConfig.DarkGuiSkinFilename;
#endif
			var usedSkin =
				AssetDatabase.LoadAssetAtPath(UniMergeConfig.DefaultPath + "/" + guiSkinToUse, typeof(GUISkin)) as GUISkin;

			if (usedSkin) {
				//GUISkin builtinSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
				var customStyles = new List<GUIStyle>(builtinSkin.customStyles);
				//Clear styles from last enable, or for light/dark switch
				for (var i = 0; i < builtinSkin.customStyles.Length; i++)
					if (builtinSkin.customStyles[i].name == UniMergeConfig.ListStyleName
						|| builtinSkin.customStyles[i].name == UniMergeConfig.ListAltStyleName
						|| builtinSkin.customStyles[i].name == UniMergeConfig.ListStyleName + UniMergeConfig.ConflictSuffix
						|| builtinSkin.customStyles[i].name == UniMergeConfig.ListAltStyleName + UniMergeConfig.ConflictSuffix)
						customStyles.Remove(builtinSkin.customStyles[i]);

				customStyles.AddRange(usedSkin.customStyles);
				builtinSkin.customStyles = customStyles.ToArray();
			} else { Debug.LogWarning("Can't find editor skin"); }
		}

		protected bool InitGUI() {
			drawAbort = false;

			if (!skinSetUp)
				SetUpSkin(GUI.skin);

			//Ctrl + w to close
			var current = Event.current;
			if (current.Equals(Event.KeyboardEvent("^w"))) {
				Close();
				current.Use();
				GUIUtility.ExitGUI();
			}

			if (current.type == EventType.ScrollWheel && objectDrawWindow < objectDrawCount) {
				if (current.delta.y > 0)
					objectDrawOffset++;
				else
					objectDrawOffset--;

				Repaint();
				return true;
			}

#if Unity3
			EditorGUIUtility.LookLikeControls();
#endif

			//Adjust colWidth as the window resizes
			colWidth = (position.width - UniMergeConfig.DoubleMidWidth - UniMergeConfig.Margin - scrollbarWidth) * 0.5f;

			if (selected != null && selected.invalid)
				selected = null;

			ProcessInput();

			return false;
		}

		protected void ProgressBar() {
			if (_update != null) {
				var pbar = new Rect(0, position.height - ProgressBarHeight * 2, position.width, ProgressBarHeight);
				EditorGUI.ProgressBar(pbar, (float) updateCount / totalUpdateNum, string.Format("{0} {1}/{2}", updateType, updateCount, totalUpdateNum));
				var cancel = new Rect(0, position.height - ProgressBarHeight, position.width, ProgressBarHeight);
				if (GUI.Button(cancel, "Cancel")) {
					cancelRefresh = true;
					GUIUtility.ExitGUI();
				}
			}
		}

		protected void CustomScroll(Action drawContent) {
			objectDrawCursor = 0;
			objectDrawCount = GetDrawCount();

			var lessCount = objectDrawCount - objectDrawWindow;
			if (lessCount < 0)
				lessCount = 0;

			if (lessCount > 0)
				lessCount++;

			if (objectDrawOffset < 0)
				objectDrawOffset = 0;

			if (objectDrawOffset >= lessCount)
				objectDrawOffset = lessCount;

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.Space(0); // Suppresses error: You cannot call GetLast immediately after beginning a group.
			if (lessCount > 0) {
				var headerHeight = GUILayoutUtility.GetLastRect().y;
				var rect = new Rect(position.width - ProgressBarHeight, headerHeight, ProgressBarHeight,
					position.height - headerHeight);
				if (_update != null)
					rect.height -= ProgressBarHeight * 2;

				scrollbarWidth = ScrollbarWidth;
				objectDrawOffset = GUI.VerticalScrollbar(rect, objectDrawOffset, 1, 0, lessCount);
			} else { scrollbarWidth = 0;}

			drawContent();

			GUILayout.EndVertical();
			if (lessCount > 0)
				GUILayoutUtility.GetRect(ProgressBarHeight, ProgressBarHeight, 0, int.MaxValue);

			GUILayout.EndHorizontal();
		}

		protected abstract int GetDrawCount();

		void Update() {
			/*
			 * Ad-hoc editor window coroutine:  Function returns and IEnumerator, and the Update function calls MoveNext
			 * Refresh will only run when the ObjectMerge window is focused
			 */
			if (cancelRefresh) {
				ResetDrawWindow();
				blockRefresh = false;
				CancelUpdate();
			}

			if (_update != null) {
				var hasNext = true;
				var oldUpdate = _update;
				while (hasNext) {
					if (YieldIfNeeded())
						break;

					hasNext = _update.MoveNext();
				}

				//Check if update is equal to the old one in case we start a new coroutine right as the last one ends
				if (!hasNext && _update == oldUpdate) {
					ResetDrawWindow();
					CancelUpdate();
				}

				Repaint();
			}

			if (stateModified) {
				stateModified = false;
				updateType = RefreshType.Updating;
				update = Refresh();
			}

			var winHeight = position.height;
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (lastWinHeight != winHeight)
				ResetDrawWindow();

			objectDrawOffsetHold = (int) objectDrawOffset;
			lastWinHeight = winHeight;

			cancelRefresh = false;
			displayWarning = true;

			frameTimer.Reset();
			frameTimer.Start();
		}

		void CancelUpdate() {
#if !(Unity3 || Unity4_0To4_2)
			updateJustFinished = true;
#endif
			_update = null;
			ListenForModifications(true);
		}

		/// <summary>
		/// Check if we can draw objects yet, and increment counter
		/// </summary>
		/// <returns></returns>
		public bool ScrollCheck() {
			var check = objectDrawCursor >= objectDrawOffset;

			if (objectDrawCursor >= objectDrawOffset + objectDrawWindow) {
				drawAbort = true;
				check = false;
			}

			if (check) {
				var lastRect = GUILayoutUtility.GetLastRect();
				var height = position.height - 22;
				if (_update != null)
					height -= ProgressBarHeight * 2;

				if (lastRect.y + lastRect.height >= height) {
					if (GUI.GetNameOfFocusedControl() != "UM_Slider")
						objectDrawWindow = objectDrawCursor - (int)objectDrawOffset;

					objectDrawOffset = objectDrawOffsetHold;
					drawAbort = true;
					check = false;
					Repaint();
					GUIUtility.ExitGUI();
				}
			}

			objectDrawCursor++;
			return check;
		}

		public void StartRow(bool same, bool selected) {
			EditorGUILayout.BeginVertical(selected ? GetAltStyle(same) : GetStyle(same));
			//Top padding
			GUILayout.Space(rowPadding + BasePaddingTop);
			// Force rows to be the same magical height, 28px, to fix scroll window issues
			// Non-uniform row heights are not supported
			EditorGUILayout.BeginHorizontal(GUILayout.Height(RowHeight));
		}

		public void EndRow(bool same) {
			EditorGUILayout.EndHorizontal();
			//Bottom padding
			GUILayout.Space(rowPadding + BasePaddingBot);

			GUILayout.Label(string.Empty, GetAltStyle(same));
			EditorGUILayout.EndVertical();
		}

		static string GetStyle(bool same) {
			return same ? UniMergeConfig.ListStyleName : UniMergeConfig.ListStyleConflictName;
		}

		static string GetAltStyle(bool same) {
			return same ? UniMergeConfig.ListAltStyleName : UniMergeConfig.ListAltStyleConflictName;
		}

		protected void DrawRowHeight() {
			var lastRowHeight = selectedRowHeight;
			selectedRowHeight = EditorGUILayout.Popup("Row height: ", selectedRowHeight, new[] { "Large", "Medium", "Small" });
			rowPadding = RowPaddings[selectedRowHeight];
			rowHeight = RowHeights[selectedRowHeight];

			if (selectedRowHeight != lastRowHeight)
				ResetDrawWindow();
		}

		bool YieldIfNeeded() {
			return frameTimer.ElapsedTicks > frameTimeTicks;
		}

		static void HandleLog(string logString, string stackTrace, LogType type) {
			//Totally hack solution, but it works.  This situation happens a lot in conflict resolution.  You have a prefab with git markup, this error spams the console, UniMerge crashes the editor.
			if (logString.Contains("seems to have merge conflicts. Please open it in a text editor and fix the merge.")) {
				if (displayWarning) {
					//I can't get this to stop displaying twice for some reason.
					EditorUtility.DisplayDialog("Merge canceled for your own good",
						"It appears that you have a prefab in your scene with merge conflicts. Unity spits out a"
						+ " warning about this at every step of the merge which makes it take years.  Resolve your"
						+ " prefab conflicts before resolving scene conflicts.", "OK, fine");
					displayWarning = false;
					foreach (var window in Windows) {
						var objectMerge = window as ObjectMerge;
						if (!objectMerge)
							continue;

						objectMerge.SetGameObjects(null, null);
					}
				}

				foreach (var window in Windows) {
					window.CancelUpdate();
					window.cancelRefresh = true;
				}

				GUIUtility.ExitGUI();
			}
		}

		internal bool IsUpdating() { return _update != null; }
		internal bool UpdateMoveNext() { return _update.MoveNext(); }
		void ResetDrawWindow() { objectDrawWindow = MaxDrawWindow; }

		protected abstract void SelectNextRow(bool expandChildren);
		public abstract void SelectPreviousRow(bool collapseChildren);
		protected abstract void SelectNextDifference();
		protected abstract void SelectPreviousDifference();
		protected abstract void ToggleSelectedComponentSection();
		protected abstract void SelectNextRowWithChildren(bool expandComponents);
		protected abstract void SelectParentRow(bool collapseComponents);
		protected abstract void ScrollToRow(Helper row);
		protected abstract void SelectRow(int row);
		protected void ScrollToRow(int row) {
			if (row >= 0 && row < objectDrawOffset) {
				objectDrawOffset = row;
				return;
			}

			if (row >= objectDrawOffset + objectDrawWindow - 1)
				objectDrawOffset = row - objectDrawWindow + 1;
		}

		void ProcessInput() {
			var current = Event.current;

			if (current.isKey && current.type == EventType.KeyDown) {
				switch (current.keyCode) {
					case KeyCode.H:
						if (current.control) {
							ToggleSelectedComponentSection();
							EndProcessInput(current);
						}
						break;
					case KeyCode.UpArrow:
						if (current.alt)
							SelectPreviousDifference();
						else
							SelectPreviousRow(current.shift);

						if (selected != null)
							ScrollToRow(selected);

						EndProcessInput(current);
						break;
					case KeyCode.DownArrow:
						if (current.alt)
							SelectNextDifference();
						else
							SelectNextRow(current.shift);

						if (selected != null)
							ScrollToRow(selected);

						EndProcessInput(current);
						break;
					case KeyCode.LeftArrow:
					case KeyCode.RightArrow:
						if (selected != null) {
							var right = current.keyCode == KeyCode.RightArrow;
							if (current.alt) {
								selected.Transfer(!right);
							} else {
								var helper = selected as GameObjectHelper;
								var shift = current.shift;
								var attributesSelected = helper != null && helper.attributesSelected;
								if (attributesSelected && helper.showAttributes != right) {
									helper.showAttributes = right;
								} else if (helper != null && !helper.hasChildren && helper.showComponents != right) {
									helper.showComponents = right;
									helper.attributesSelected = false;
								} else if (current.control) {
									selected.SetExpandedRecursively(right);
								} else if ((selected.showChildren || attributesSelected && helper.showAttributes) && right) {
									SelectNextRowWithChildren(shift);
								} else if (attributesSelected && !right) {
									helper.attributesSelected = false;
								} else if (!selected.showChildren && !right) {
									SelectParentRow(shift);
								} else if (helper == null || !helper.attributesSelected) {
									selected.showChildren = right;
								}
							}

							selected.InvalidateDrawCount();
							ScrollToRow(selected);
						} else {
							SelectNextRow(false);
						}

						EndProcessInput(current);
						break;
					case KeyCode.PageUp:
						objectDrawOffset -= objectDrawWindow;
						if (objectDrawOffset < 0)
							objectDrawOffset = 0;

						EndProcessInput(current);
						break;
					case KeyCode.PageDown:
						objectDrawOffset += objectDrawWindow;
						if (objectDrawOffset > objectDrawCount)
							objectDrawOffset = 0;

						EndProcessInput(current);
						break;
				}
			}

			if (current.isMouse &&  current.type == EventType.MouseDown) {
				var height = current.mousePosition.y - headerSize;
				if (height > 0) {
					SelectRow((int)(height / rowHeight));
					Repaint();
				}
			}
		}

		void EndProcessInput(Event current) {
			current.Use();
			Repaint();
			GUIUtility.ExitGUI();
		}
	}
}
