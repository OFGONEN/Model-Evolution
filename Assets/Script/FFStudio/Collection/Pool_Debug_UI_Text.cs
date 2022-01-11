/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "pool_debug_ui_text", menuName = "FF/Data/Pool/Debug UI Text" ) ]
	public class Pool_Debug_UI_Text : ComponentPool< Debug_UI_Text >
	{
		private static Pool_Debug_UI_Text instance;

		public static Pool_Debug_UI_Text Instance => instance;

		public override void InitPool( Transform parent, bool active )
		{
			instance = this;

			base.InitPool( parent, active );
		}
	}
}