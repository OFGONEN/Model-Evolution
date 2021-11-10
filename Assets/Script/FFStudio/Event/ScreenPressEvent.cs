/* Created by and for usage of FF Studios (202/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[CreateAssetMenu( fileName = "input_press_event", menuName = "FF/Event/Input/ScreenPressEvent" )]
public class ScreenPressEvent : GameEvent
{
	#region Fields
	public bool isPressedDown = false;
	public Vector2 screenPosition = Vector2.zero;
	#endregion
}
